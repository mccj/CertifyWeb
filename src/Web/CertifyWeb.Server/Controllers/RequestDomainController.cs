using Certify.ACME.Anvil;
using Certify.ACME.Anvil.Acme;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using SqlSugar;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TestProject1;

namespace CertifyWeb.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RequestDomainController(IServiceProvider serviceProvider) : ControllerBase
    {
        private readonly ILogger<RequestDomainController> _logger = serviceProvider.GetRequiredService<ILogger<RequestDomainController>>();
        protected internal readonly SimpleClient<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainInfoRecord> _domainInfoRepository = serviceProvider.GetRequiredService<SimpleClient<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainInfoRecord>>();
        protected internal readonly SimpleClient<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.AcmeInfoRecord> _acmeInfoRepository = serviceProvider.GetRequiredService<SimpleClient<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.AcmeInfoRecord>>();
        protected internal readonly SimpleClient<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DnsInfoRecord> _dnsInfoRepository = serviceProvider.GetRequiredService<SimpleClient<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DnsInfoRecord>>();
        protected internal readonly SimpleClient<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainOrderRecord> _domainOrderRepository = serviceProvider.GetRequiredService<SimpleClient<OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainOrderRecord>>();

        ///// <summary>
        ///// 自动配置dns申请证书
        ///// </summary>
        //[HttpPost]
        //public async Task GetDomainOrderAsync(long id)
        //{
        //    var domainInfoRecord = _domainInfoRepository.GetById(id);

        //    //放置通配符证书订单（通配符证书需要 DNS 验证）
        //    var certificateIdentifiers = new[] { domainInfoRecord.PrimaryDomain?.Trim() }.Concat(domainInfoRecord.OtherDomains?.Split(["\r", "\n"], StringSplitOptions.RemoveEmptyEntries)?.Select(f => f.Trim()) ?? []).Distinct().ToArray();

        //    await CreateDomainOrderAsync(domainInfoRecord.AcmeId, certificateIdentifiers, "");
        //}
        /// <summary>
        /// 自动配置dns申请证书
        /// </summary>
        [HttpPost]
        public async Task CreateDomainOrderAsync(long acmeId, string domains, string? description)
        {
            var acmeInfoRecord = _acmeInfoRepository.GetById(acmeId);

            var acme = new AcmeContext(new Uri(acmeInfoRecord.DirectoryUri), PluginManager.PemKey, badNonceRetryCount: 10);

            //登入账号
            var acmeAccount = await acme.NewAccount(acmeInfoRecord.Email, true, acmeInfoRecord.EabKeyId, acmeInfoRecord.EabHmacKey, acmeInfoRecord.EabKeyAlg);

            var order = await acme.NewOrder(domains?.Split(["\r", "\n"], StringSplitOptions.RemoveEmptyEntries));
            var orderResource = await order.Resource();

            var oid = await _domainOrderRepository.AsQueryable().Where(f => f.OrderUrl.ToLower() == order.Location.AbsoluteUri.ToLower()).Select(f => f.Id).FirstAsync();
            var domainRecord = new OneNetIotService.Server.EntityAutoGenerator.AutoGenerator.DomainOrderRecord
            {
                Id = oid,
                AcmeTitle = acmeInfoRecord.AcmeTitle,
                Domains = string.Join(",", orderResource.Identifiers.Select(f => f.Value)),
                DirectoryUri = acmeInfoRecord.DirectoryUri,
                OrderUrl = order.Location.AbsoluteUri,
                OrderExpires = orderResource.Expires,
                OrderStatus = orderResource.Status,
                Description = description,
            };
            if (oid > 0)
                await _domainOrderRepository.UpdateAsync(domainRecord);
            else
                await _domainOrderRepository.InsertReturnSnowflakeIdAsync(domainRecord);
        }
        /// <summary>
        /// 获取证书授权
        /// </summary>
        [HttpPost]
        public async Task<object[]> GetDomainOrderAuthorizationsAsync(long id)
        {
            var domainOrderRecord = _domainOrderRepository.GetById(id);

            var acme = new AcmeContext(new Uri(domainOrderRecord.DirectoryUri), PluginManager.PemKey, badNonceRetryCount: 10);

            var order = acme.Order(new Uri(domainOrderRecord.OrderUrl));
            //var orderResource = await order.Resource();

            var authz = await order.Authorizations();
            var result = authz.Select(f =>
            {
                var resource = f.Resource().Result;
                var dnsAuthz = f.Dns().Result;
                var httpAuthz = f.Http().Result;
                var tlsAlpnAuthz = f.TlsAlpn().Result;
                return new
                {
                    DirectoryUri = domainOrderRecord.DirectoryUri,
                    AuthUrl = f.Location,
                    Resource = new { resource.Expires, resource.Identifier, resource.Status, resource.Wildcard },
                    DnsAuthz = new { dnsAuthz.Location, dnsAuthz.KeyAuthz, dnsAuthz.Token, dnsAuthz.Type, dnsAuthz.RetryAfter, DnsTxt = acme.AccountKey.DnsTxt(dnsAuthz.Token) },
                    HttpAuthz = httpAuthz,
                    TlsAlpnAuthz = tlsAlpnAuthz
                };
            }).ToArray();
            return result;
        }
        /// <summary>
        /// 验证证书授权
        /// </summary>
        [HttpPost]
        public async Task<object?> GetDomainOrderAuthValidateAsync(string directoryUri, string authUrl)
        {
            var acme = new AcmeContext(new Uri(directoryUri), PluginManager.PemKey, badNonceRetryCount: 10);
            var authz = acme.Authorization(new Uri(authUrl));
            var ssss = await authz.Challenges();
            //var sssdd = ssss.FirstOrDefault().Resource();
            //var sss = await authz.Deactivate();

            var sfs = await authz.Dns();
            var sfs1 = await authz.TlsAlpn();
            var sfs2 = await authz.Http();

            var dsfsf = await sfs.Validate();

            var ss = await sfs.Resource();

            //ChallengeStatus.Invalid
            //AuthorizationStatus
            return ss;
        }
        /// <summary>
        /// 下载证书
        /// </summary>
        [HttpPost]
        public async Task DownloadCertificateAsync(long id)
        {
            var domainOrderRecord = _domainOrderRepository.GetById(id);

            var acme = new AcmeContext(new Uri(domainOrderRecord.DirectoryUri), PluginManager.PemKey, badNonceRetryCount: 10);

            var order = acme.Order(new Uri(domainOrderRecord.OrderUrl));

            var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256, 2048);
            var cert = await order.Generate(new CsrInfo
            {
                CountryName = "CN",//国家
                Locality = "Ningbo",//城市
                State = "Zhejiang",//省
                Organization = "软云智联",//企业
                OrganizationUnit = "研发",//部门
                //RequireOcspMustStaple = false,
                CommonName = domainOrderRecord.Domains//证书拥有者名
            }, privateKey);

            var orderResource = await order.Resource();

            var certificate = cert.ToPem();
            var privkey = privateKey.ToPem();

            domainOrderRecord.PrivateKey = privkey;
            domainOrderRecord.Certificate = certificate;
            domainOrderRecord.OrderStatus = orderResource.Status;

            await _domainOrderRepository.UpdateAsync(domainOrderRecord);

            //var privateKey = KeyFactory.NewKey(KeyAlgorithm.RS256, 2048);
            //var ssd = await order.Finalize(
            // new CsrInfo
            // {
            //     //CountryName = "CA",
            //     //State = "State",
            //     //Locality = "City",
            //     //Organization = "Dept",
            //     CommonName = _idnMapping.GetAscii(order1.PrimaryDomain)
            // }, privateKey);

            //var csr = new Certes.Pkcs.CertificationRequestBuilder();
            //csr.AddName($"C=CA, ST=State, L=City, O=Dept, CN=*.example.com");

            //var dfds = await order.Finalize(csr.Generate());


            //var certChain = await order.Finalize();
            //var certPem11 = certChain.ToPem();
            //var certPem12 = certChain.Certificate.ToPem();

        }
        /// <summary>
        /// 吊销证书
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task RevokeCertAsync(long id)
        {
            var domainOrderRecord = _domainOrderRepository.GetById(id);

            var privateKey = KeyFactory.FromPem(domainOrderRecord.PrivateKey);
            var certChain = new CertificateChain(domainOrderRecord.Certificate);

            var acme = new AcmeContext(new Uri(domainOrderRecord.DirectoryUri), PluginManager.PemKey, badNonceRetryCount: 10);
            //吊销证书
            try
            {
                await acme.RevokeCertificate(certChain.Certificate.ToDer(), Certify.ACME.Anvil.Acme.Resource.RevocationReason.KeyCompromise, privateKey);
            }
            catch (Exception)
            {
            }

            var order = acme.Order(new Uri(domainOrderRecord.OrderUrl));
            var orderResource = await order.Resource();

            domainOrderRecord.OrderStatus = orderResource.Status;

            await _domainOrderRepository.UpdateAsync(domainOrderRecord);
        }
        [HttpGet]
        public async Task<object> GetCertInfoAsync(long id)
        {
            var domainOrderRecord = await _domainOrderRepository.GetByIdAsync(id);
            if (domainOrderRecord == null) throw new Exception("证书不存在");

            var certificate = X509Certificate2.CreateFromPem(domainOrderRecord.Certificate);
            //var certificate = X509CertificateLoader.LoadCertificate(certChain.Certificate.ToDer());
            //var certificate = X509Certificate2.CreateFromPem(domainOrderRecord.Certificate, domainOrderRecord.PrivateKey);
            //var ss = RsaKeysFormatExtensions.PemPublicKeyToXml(domainOrderRecord.PrivateKey);
            //var cert2 = X509CertificateLoader.LoadPkcs12(certChain.Certificate.ToDer(), "123456");
            //var sss1 = certificate.Export(X509ContentType.Cert, "1111");
            //var sss2 = certificate.Export(X509ContentType.Pkcs12, "1111");

            //var rsa = RSA.Create();

            //var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(domainOrderRecord.PrivateKey);

            //rsa.ImportParameters(new RSAParameters() {  Modulus});



            //try
            //{
            //    //rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            //    rsa.ImportRSAPrivateKey(privateKey.ToDer(), out _);
            //}
            //catch (CryptographicException)
            //{
            //    try
            //    {
            //        //rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            //        rsa.ImportPkcs8PrivateKey(privateKey.ToDer(), out _);
            //    }
            //    catch (CryptographicException)
            //    {
            //        ECDsa ecdsa = ECDsa.Create();
            //        //ecdsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            //        ecdsa.ImportPkcs8PrivateKey(privateKey.ToDer(), out _);
            //        var sdd = certificate.CopyWithPrivateKey(ecdsa);
            //    }
            //}
            //var sddd = certificate.CopyWithPrivateKey(rsa);

            return new
            {
                certificate.Version,
                certificate.Thumbprint,
                //SubjectName = certificate.SubjectName.Name,
                certificate.Subject,
                certificate.SerialNumber,
                certificate.NotBefore,
                certificate.NotAfter,
                //IssuerName = certificate.IssuerName.Name,
                certificate.Issuer,
                certificate.Handle,
                certificate.FriendlyName,
                certificate.Archived,
                CertHash = certificate.GetCertHashString(),
                CertFormat = certificate.GetFormat(),
                //ECDiffieHellmanPrivateKey = certificate.GetECDiffieHellmanPrivateKey(),
                //ECDiffieHellmanPublicKey = certificate.GetECDiffieHellmanPublicKey(),
                EffectiveDate = certificate.GetEffectiveDateString(),
                ExpirationDate = certificate.GetExpirationDateString(),
                KeyAlgorithm = certificate.GetKeyAlgorithm(),
                KeyAlgorithmParameters = certificate.GetKeyAlgorithmParametersString(),
                PublicKey = certificate.GetPublicKeyString(),
                PrivateKeyAlgorithm = KeyFactory.FromPem(domainOrderRecord.PrivateKey).Algorithm.ToString(),
                //RawCertData = certificate.GetRawCertDataString(),
                //SerialNumber1 = certificate.GetSerialNumberString(),
            };
        }
        /// <summary>
        /// 下载证书
        /// </summary>
        /// <param name="id"></param>
        /// <param name="certFormat"></param>
        /// <param name="serverType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        public async Task<FileResult> DownloadCertFileAsync(long id, string certFormat, string serverType)
        {
            var domainOrderRecord = await _domainOrderRepository.GetByIdAsync(id);
            if (domainOrderRecord == null) throw new Exception("证书不存在");

            var privateKey = KeyFactory.FromPem(domainOrderRecord.PrivateKey);
            var certChain = new CertificateChain(domainOrderRecord.Certificate);


            var primaryDomain = "www.cluster.ink";
            var fileDownloadName = $"{primaryDomain}_{serverType.ToLower()}.zip";
            if (certFormat == "pfx")
            {
                var certFriendlyName = primaryDomain;
                var password = GenerateRandomPassword(8);
                var pfx = certChain.ToPfx(privateKey);
                var pfxBytes = pfx.Build(certFriendlyName, password, false);//pfx文件
                var pfxlegacyBytes = pfx.Build(certFriendlyName, password, true);//pfx文件

                return GenerateZipFile(zipArchive =>
                {
                    // 添加一个文件到ZIP存档中
                    using (var writer = new StreamWriter(zipArchive.CreateEntry("pfx-password.txt").Open())) writer.Write(password);
                    using (var writer = zipArchive.CreateEntry($"{primaryDomain}.pfx").Open()) writer.Write(pfxBytes);
                    using (var writer = zipArchive.CreateEntry($"{primaryDomain}_legacy.pfx").Open()) writer.Write(pfxlegacyBytes);
                }, fileDownloadName);
            }
            else if (certFormat == "crt/key")
            {
                return GenerateZipFile(zipArchive =>
                {
                    // 添加一个文件到ZIP存档中
                    using (var writer = new StreamWriter(zipArchive.CreateEntry("privkey.key").Open())) writer.Write(domainOrderRecord.PrivateKey);
                    using (var writer = new StreamWriter(zipArchive.CreateEntry($"{primaryDomain}_public.crt").Open())) writer.Write(certChain.Certificate.ToPem());
                    if (certChain.Issuers.Count > 0)
                        using (var writer = new StreamWriter(zipArchive.CreateEntry($"{primaryDomain}_chain.crt").Open())) writer.Write(string.Join("\r\n", certChain.Issuers.Select(f => f.ToPem())));
                }, fileDownloadName);
            }
            else
            {
                return GenerateZipFile(zipArchive =>
                {
                    // 添加一个文件到ZIP存档中
                    using (var writer = new StreamWriter(zipArchive.CreateEntry("privkey.key").Open())) writer.Write(domainOrderRecord.PrivateKey);
                    using (var writer = new StreamWriter(zipArchive.CreateEntry($"{primaryDomain}.cer").Open())) writer.Write(domainOrderRecord.Certificate);
                    using (var writer = zipArchive.CreateEntry($"{primaryDomain}.der").Open()) writer.Write(certChain.Certificate.ToDer());
                }, fileDownloadName);
            }
        }
        /// <summary>
        /// 创建压缩包
        /// </summary>
        /// <param name="action"></param>
        /// <param name="fileDownloadName"></param>
        /// <returns></returns>
        private static FileResult GenerateZipFile(Action<ZipArchive> action, string fileDownloadName)
        {
            // 创建一个内存流来存储ZIP内容
            using (var memoryStream = new MemoryStream())
            {
                // 创建一个ZipArchive对象，指定压缩级别为最佳压缩（Fastest或NoCompression除外）
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // 添加一个文件到ZIP存档中
                    action(zipArchive);
                }

                // 将内存流的内容写入文件（可选）
                return new FileContentResult(memoryStream.ToArray(), "application/zip") { FileDownloadName = fileDownloadName };
            }
        }
        /// <summary>
        /// 创建随机密码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string GenerateRandomPassword(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return RandomNumberGenerator.GetString(validChars, length);
        }
    }
    public static class RsaKeysFormatExtensions
    {
        /// <summary>
        /// XML公钥转成Pem公钥
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <returns></returns>
        public static string XmlPublicKeyToPem(this string xmlPublicKey)
        {
            RSAParameters rsaParam;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())

            {

                rsa.FromXmlString(xmlPublicKey);

                rsaParam = rsa.ExportParameters(false);

            }

            RsaKeyParameters param = new RsaKeyParameters(false, new Org.BouncyCastle.Math.BigInteger(1, rsaParam.Modulus), new Org.BouncyCastle.Math.BigInteger(1, rsaParam.Exponent));

            string pemPublicKeyStr = null;

            using (var ms = new MemoryStream())

            {

                using (var sw = new StreamWriter(ms))

                {

                    var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);

                    pemWriter.WriteObject(param);

                    sw.Flush();

                    byte[] buffer = new byte[ms.Length];

                    ms.Position = 0;

                    ms.Read(buffer, 0, (int)ms.Length);

                    pemPublicKeyStr = Encoding.UTF8.GetString(buffer);

                }

            }

            return pemPublicKeyStr;

        }
        /// <summary>
        /// Pem公钥转成XML公钥
        /// </summary>
        /// <param name="pemPublicKeyStr"></param>
        /// <returns></returns>
        public static string PemPublicKeyToXml(this string pemPublicKeyStr)
        {
            RsaKeyParameters pemPublicKey;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(pemPublicKeyStr)))
            {

                using (var sr = new StreamReader(ms))

                {

                    var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);

                    pemPublicKey = (RsaKeyParameters)pemReader.ReadObject();

                }

            }

            var p = new RSAParameters

            {

                Modulus = pemPublicKey.Modulus.ToByteArrayUnsigned(),

                Exponent = pemPublicKey.Exponent.ToByteArrayUnsigned()

            };

            string xmlPublicKeyStr;

            using (var rsa = new RSACryptoServiceProvider())

            {

                rsa.ImportParameters(p);

                xmlPublicKeyStr = rsa.ToXmlString(false);

            }

            return xmlPublicKeyStr;

        }
        /// <summary>
        /// XML私钥转成PEM私钥
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <returns></returns>
        public static string XmlPrivateKeyToPem(this string xmlPrivateKey)

        {

            RSAParameters rsaParam;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())

            {

                rsa.FromXmlString(xmlPrivateKey);

                rsaParam = rsa.ExportParameters(true);

            }

            var param = new RsaPrivateCrtKeyParameters(

            new Org.BouncyCastle.Math.BigInteger(1, rsaParam.Modulus), new Org.BouncyCastle.Math.BigInteger(1, rsaParam.Exponent), new Org.BouncyCastle.Math.BigInteger(1, rsaParam.D),

            new Org.BouncyCastle.Math.BigInteger(1, rsaParam.P), new Org.BouncyCastle.Math.BigInteger(1, rsaParam.Q), new Org.BouncyCastle.Math.BigInteger(1, rsaParam.DP), new Org.BouncyCastle.Math.BigInteger(1, rsaParam.DQ),

            new Org.BouncyCastle.Math.BigInteger(1, rsaParam.InverseQ));

            string pemPrivateKeyStr = null;

            using (var ms = new MemoryStream())

            {

                using (var sw = new StreamWriter(ms))

                {

                    var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);

                    pemWriter.WriteObject(param);

                    sw.Flush();

                    byte[] buffer = new byte[ms.Length];

                    ms.Position = 0;

                    ms.Read(buffer, 0, (int)ms.Length);

                    pemPrivateKeyStr = Encoding.UTF8.GetString(buffer);

                }

            }

            return pemPrivateKeyStr;

        }

        /// <summary>

        /// Pem私钥转成XML私钥

        /// </summary>

        /// <param name="pemPrivateKeyStr"></param>

        /// <returns></returns>

        public static string PemPrivateKeyToXml(this string pemPrivateKeyStr)

        {

            RsaPrivateCrtKeyParameters pemPrivateKey;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(pemPrivateKeyStr)))

            {

                using (var sr = new StreamReader(ms))

                {

                    var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);

                    var keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();

                    pemPrivateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;

                }

            }

            var p = new RSAParameters

            {

                Modulus = pemPrivateKey.Modulus.ToByteArrayUnsigned(),

                Exponent = pemPrivateKey.PublicExponent.ToByteArrayUnsigned(),

                D = pemPrivateKey.Exponent.ToByteArrayUnsigned(),

                P = pemPrivateKey.P.ToByteArrayUnsigned(),

                Q = pemPrivateKey.Q.ToByteArrayUnsigned(),

                DP = pemPrivateKey.DP.ToByteArrayUnsigned(),

                DQ = pemPrivateKey.DQ.ToByteArrayUnsigned(),

                InverseQ = pemPrivateKey.QInv.ToByteArrayUnsigned(),

            };

            string xmlPrivateKeyStr;

            using (var rsa = new RSACryptoServiceProvider())

            {

                rsa.ImportParameters(p);

                xmlPrivateKeyStr = rsa.ToXmlString(true);

            }

            return xmlPrivateKeyStr;

        }
        #region 解析.net 生成的Pem
        /// <summary>
        /// 将Pkcs1pPem格式公钥(1024 or 2048)转换为RSAParameters
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        private static RSAParameters ConvertFromPublicKey(this string publicKey)
        {

            if (string.IsNullOrEmpty(publicKey))
            {
                throw new ArgumentNullException("publicKey", "This arg cann't be empty.");
            }
            byte[] keyData = Convert.FromBase64String(publicKey);
            bool keySize1024 = (keyData.Length == 162);
            bool keySize2048 = (keyData.Length == 294);
            if (!(keySize1024 || keySize2048))
            {
                throw new ArgumentException("pem file content is incorrect, Only support the key size is 1024 or 2048");
            }
            byte[] pemModulus = (keySize1024 ? new byte[128] : new byte[256]);
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, (keySize1024 ? 29 : 33), pemModulus, 0, (keySize1024 ? 128 : 256));
            Array.Copy(keyData, (keySize1024 ? 159 : 291), pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            return para;
        }
        /// <summary>
        /// 将Pkcs1pPem格式私钥(1024 or 2048)转换为RSAParameters
        /// </summary>
        /// <param name="privateKey">pem私钥内容</param>
        /// <returns>转换得到的RSAParamenters</returns>
        public static RSAParameters ConvertFromPrivateKey(this string privateKey)
        {
            if (string.IsNullOrEmpty(privateKey))
            {
                throw new ArgumentNullException("privateKey", "This arg cann't be empty.");
            }
            byte[] keyData = Convert.FromBase64String(privateKey);


            bool keySize1024 = (keyData.Length == 609 || keyData.Length == 610);
            bool keySize2048 = (keyData.Length == 1190 || keyData.Length == 1191 || keyData.Length == 1192);

            if (!(keySize1024 || keySize2048))
            {
                throw new ArgumentException("pem file content is incorrect, Only support the key size is 1024 or 2048");
            }

            int index = (keySize1024 ? 11 : 12);
            byte[] pemModulus = (keySize1024 ? new byte[128] : new byte[256]);
            Array.Copy(keyData, index, pemModulus, 0, pemModulus.Length);

            index += pemModulus.Length;
            index += 2;
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, index, pemPublicExponent, 0, 3);

            index += 3;
            index += 4;
            if ((int)keyData[index] == 0)
            {
                index++;
            }
            byte[] pemPrivateExponent = (keySize1024 ? new byte[128] : new byte[256]);
            Array.Copy(keyData, index, pemPrivateExponent, 0, pemPrivateExponent.Length);

            index += pemPrivateExponent.Length;
            index += (keySize1024 ? ((int)keyData[index + 1] == 64 ? 2 : 3) : ((int)keyData[index + 2] == 128 ? 3 : 4));
            byte[] pemPrime1 = (keySize1024 ? new byte[64] : new byte[128]);
            Array.Copy(keyData, index, pemPrime1, 0, pemPrime1.Length);

            index += pemPrime1.Length;
            index += (keySize1024 ? ((int)keyData[index + 1] == 64 ? 2 : 3) : ((int)keyData[index + 2] == 128 ? 3 : 4));
            byte[] pemPrime2 = (keySize1024 ? new byte[64] : new byte[128]);
            Array.Copy(keyData, index, pemPrime2, 0, pemPrime2.Length);

            index += pemPrime2.Length;
            index += (keySize1024 ? ((int)keyData[index + 1] == 64 ? 2 : 3) : ((int)keyData[index + 2] == 128 ? 3 : 4));
            byte[] pemExponent1 = (keySize1024 ? new byte[64] : new byte[128]);
            Array.Copy(keyData, index, pemExponent1, 0, pemExponent1.Length);

            index += pemExponent1.Length;
            index += (keySize1024 ? ((int)keyData[index + 1] == 64 ? 2 : 3) : ((int)keyData[index + 2] == 128 ? 3 : 4));
            byte[] pemExponent2 = (keySize1024 ? new byte[64] : new byte[128]);
            Array.Copy(keyData, index, pemExponent2, 0, pemExponent2.Length);

            index += pemExponent2.Length;
            index += (keySize1024 ? ((int)keyData[index + 1] == 64 ? 2 : 3) : ((int)keyData[index + 2] == 128 ? 3 : 4));
            byte[] pemCoefficient = (keySize1024 ? new byte[64] : new byte[128]);
            Array.Copy(keyData, index, pemCoefficient, 0, pemCoefficient.Length);

            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            para.D = pemPrivateExponent;
            para.P = pemPrime1;
            para.Q = pemPrime2;
            para.DP = pemExponent1;
            para.DQ = pemExponent2;
            para.InverseQ = pemCoefficient;
            return para;
        }
        #endregion
    }
}
