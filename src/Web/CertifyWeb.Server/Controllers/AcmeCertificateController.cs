using Certify.ACME.Anvil;
using Certify.ACME.Anvil.Acme;
using Certify.ACME.Anvil.Acme.Resource;
using Certify.Models;
using Certify.Models.Providers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using TestProject1;

namespace CertifyWeb.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AcmeCertificateController(ILogger<AcmeCertificateController> logger) : ControllerBase
    {
        private readonly ILogger<AcmeCertificateController> _logger = logger;

        //private readonly ILogger<AcmeCertificateController> _logger;

        //public AcmeCertificateController(ILogger<AcmeCertificateController> logger)
        //{
        //    _logger = logger;
        //}

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}
        /// <summary>
        /// 申请证书
        /// </summary>
        [HttpPost]
        public async Task RequestCertificaAsync()
        {
            var dnsAPIProvider = await getDnsProviderAsync();
            var acme = await getAcmeAsync();
            var order1 = getOrder();

            var _idnMapping = new IdnMapping();

            //放置通配符证书订单（通配符证书需要 DNS 验证）
            var certificateIdentifiers = new[] { order1.PrimaryDomain }.Concat(order1.Domains).Distinct().Select(f => new Identifier { Type = order1.IdentifierType, Value = f }).ToArray();

            var order = await acme.NewOrder(certificateIdentifiers);
            var orderUri = order.Location;

            var aaaaa = new List<(Func<Challenge>, Func<Certify.Models.Config.ActionResult>)>();
            var orderAuthorizations = await order.Authorizations();
            //生成 DNS TXT 记录的值
            foreach (var authz in orderAuthorizations)
            {
                //var challenges = await authz.Challenges();
                var dnsChallenge = await authz.Dns();
                //var httpChallenge = await authz.Http();
                //var tlsAlpnChallenge = await authz.TlsAlpn();

                var dnsTxt = acme.AccountKey.DnsTxt(dnsChallenge.Token);

                var res2 = await authz.Resource();
                var authzDomain2 = res2?.Identifier.Value;
                //if (res2?.Wildcard == true)
                //{
                //    authzDomain2 = "*." + authzDomain2;
                //}
                //var authIdentifierType2 = res2.Identifier.Type;
                //var allIdentifierChallenges2 = res2.Challenges;
                ////_acme-challenge.your.domain.namednsTxt
                var dnsKey = $"_acme-challenge.{authzDomain2}".Replace("*.", "");

                var txtRecordName = _idnMapping.GetAscii(dnsKey).ToLower().Trim();
                //var a2 = dnsAPIProvider.Test().Result;
                var ss = new Certify.Models.Providers.DnsRecord() { RecordType = "TXT", ZoneId = "9855804ce5224f33a0f38e1fc669fe73", TargetDomainName = order1.PrimaryDomain, RecordName = txtRecordName, RecordValue = dnsTxt };
                var a3 = dnsAPIProvider.CreateRecord(ss).Result;
                //var a4 = dnsAPIProvider.DeleteRecord(ss).Result;

                //var alpnCertKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
                //var alpnCert = acme.AccountKey.TlsAlpnCertificate(dnsChallenge.Token, "www.my-domain.com", alpnCertKey);

                System.Threading.Thread.Sleep(2000);

                var sdfdfd = await dnsChallenge.Validate();
                //var dfsdf = await dnsChallenge.Resource();

                aaaaa.Add((() => dnsChallenge.Resource().Result, () => dnsAPIProvider.DeleteRecord(ss).Result));
            }
            //foreach (var authContext in orderAuthorizations)
            //{
            //    IdentifierType authIdentifierType = IdentifierType.Dns;
            //    try
            //    {
            //        var res2 = await authContext.Resource();

            //        var authzDomain2 = res2?.Identifier.Value;

            //        if (res2?.Wildcard == true)
            //        {
            //            authzDomain2 = "*." + authzDomain2;
            //        }

            //        var authIdentifierType2 = res2.Identifier.Type;
            //        var allIdentifierChallenges2 = res2.Challenges;



            //    }
            //    catch
            //    {
            //        //log.Error("Failed to fetch auth challenge details from ACME API.");
            //        break;
            //    }

            //}
            Challenge[] challenges;
            while (true)
            {
                System.Threading.Thread.Sleep(5000);
                challenges = aaaaa.AsQueryable().Select(f => f.Item1()).ToArray();
                if (challenges.All(res => res.Status == ChallengeStatus.Valid || res.Status == ChallengeStatus.Invalid))
                    break;
            }
            aaaaa.ForEach(f => f.Item2());
            if (challenges.Any(res => res.Status == ChallengeStatus.Invalid))
            {
                var err = challenges.Select(f => f.Error).ToArray();
                return;
            }

            var privateKey = KeyFactory.NewKey(KeyAlgorithm.RS256, 2048);
            var ssd = await order.Finalize(
             new CsrInfo
             {
                 //CountryName = "CA",
                 //State = "State",
                 //Locality = "City",
                 //Organization = "Dept",
                 CommonName = _idnMapping.GetAscii(order1.PrimaryDomain)
             }, privateKey);

            //var csr = new Certes.Pkcs.CertificationRequestBuilder();
            //csr.AddName($"C=CA, ST=State, L=City, O=Dept, CN=*.example.com");

            //var dfds = await order.Finalize(csr.Generate());


            var certChain = await order.Download();
            //var certChain2 = await order.Download("ISRG X1 Root");


            //var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certChain.Certificate.ToDer());
            var cert = X509CertificateLoader.LoadCertificate(certChain.Certificate.ToDer());
            var certFriendlyName = $"[Certify] {cert.GetEffectiveDateString()} to {cert.GetExpirationDateString()}";

            //var cert222 = new CertificateInfo(certChain, privateKey);

            var certPem11 = certChain.ToPem();
            //var certPem22 = certChain.Certificate.ToPem();
            //var ddd = certChain.Certificate.ToDer();

            var privkeyPem = privateKey.ToPem();
            var pfx = certChain.ToPfx(privateKey);
            var pfxBytes = pfx.Build(certFriendlyName, "`1q2w3e4r");//pfx文件


            System.IO.File.WriteAllBytes(order1.PrimaryDomain + ".pfx", pfxBytes);
            System.IO.File.WriteAllText(order1.PrimaryDomain + ".cer", certPem11);
            System.IO.File.WriteAllText("privkey.txt", privkeyPem);


            ////authz = (await order.Authorizations()).First();
            //var keyAuthz = httpChallenge.KeyAuthz;
            //var token = httpChallenge.Token;
            //await httpChallenge.Validate();

            //var res = await authz.Resource();
            //while (res.Status != AuthorizationStatus.Valid && res.Status != AuthorizationStatus.Invalid)
            //{
            //    res = await authz.Resource();
            //}



            //var cert = await order.Generate(new CsrInfo
            //{
            //    CountryName = "CA",
            //    State = "Ontario",
            //    Locality = "Toronto",
            //    Organization = "Certes",
            //    OrganizationUnit = "Dev",
            //    CommonName = "www.certes-ci.dymetis.com",
            //}, privateKey, null);

            //var certPem = cert.ToPem();
            //var pfxBuilder = cert.ToPfx(privateKey);

            //var pfx = pfxBuilder.Build("my-cert", "abcd1234");

            //吊销证书
            //await acme.RevokeCertificate(certChain.Certificate.ToDer(), RevocationReason.KeyCompromise, privateKey);

        }
        /// <summary>
        /// 获取Acme商户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public CertificateAuthority[] GetCertificateAuthorities() => Certify.Models.CertificateAuthority.CoreCertificateAuthorities.ToArray();
        [HttpGet]
        public async Task ddd()
        {
            var sss =await PluginManager.GetChallengeAPIProviders();
            var fdf =await getDnsProviderAsync();
        }
        private async Task<IDnsProvider> getDnsProviderAsync()
        {
            var dnsAPIProvider = await PluginManager.GetDnsProvider("DNS01.API.Aliyun",
                new System.Collections.Generic.Dictionary<string, string> {
                            {"accesskeyid","xx" },
                            {"accesskeysecret","xx" }
                },
                new System.Collections.Generic.Dictionary<string, string> { }
            );
            return dnsAPIProvider;
        }
        private async Task<AcmeContext> getAcmeAsync()
        {
            //创建新的 ACME 帐户
            //var _certificateAuthorities = Certify.Models.CertificateAuthority.CoreCertificateAuthorities;
            ////Let's Encrypt
            //var acme = new AcmeContext(WellKnownServers.LetsEncryptV2);
            var acme = new AcmeContext(new Uri("https://acme-v02.api.letsencrypt.org/directory"));
            var account = await acme.NewAccount("mccj@qq.com", true);
            //ZeroSSL
            //var acme = new AcmeContext(new Uri("https://acme.zerossl.com/v2/DV90"));
            //var account = await acme.NewAccount("mccj@qq.com", true, "xx", "xx","");

            //var acme = new AcmeContext(new Uri("https://acme.ssl.com/sslcom-dv-rsa"));
            //var account = await acme.NewAccount("mccj@qq.com", true, "xx", "xx");

            //var acme = new AcmeContext(new Uri("https://api.buypass.com/acme/directory"));
            //var account = await acme.NewAccount("mccj@qq.com", true, "xx", "xx");

            //var tos = await acme.TermsOfService();
            //var account1 = await acme.Account();
            //var accountInfo = await account.Resource();

            // Save the account key for later use
            var pemKey = acme.AccountKey.ToPem();
            //var der = acme.AccountKey.ToDer();



            //            //使用现有的 ACME 帐户
            //            //// Load the saved account key
            //            var pemKey = @"-----BEGIN EC PRIVATE KEY-----
            //-----END EC PRIVATE KEY-----
            //";
            //            var accountKey = KeyFactory.FromPem(pemKey);
            //            var acme = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, accountKey);
            //            var account = await acme.Account();

            return acme;
        }
        private OrderInfo getOrder()
        {
            var authzDomain = "cluster.ink";
            return new OrderInfo
            {
                IdentifierType = IdentifierType.Dns,
                PrimaryDomain = authzDomain,
                Domains = [
                    authzDomain,
                    "*." + authzDomain,
                    //"k8s." + authzDomain ,
                    "*.k8s." + authzDomain ,
                    //"prd." + authzDomain ,
                    "*.prd.k8s." + authzDomain ,
                    //"dev." + authzDomain ,
                    "*.dev.k8s." + authzDomain ,
                    //"app." + authzDomain ,
                    "*.app.k8s." + authzDomain ,
                    //"dbs." + authzDomain ,
                    "*.dbs.k8s." + authzDomain ,
                    //"test." + authzDomain ,
                    "*.test.k8s." + authzDomain
                ],
                //KeyType = ""
            };


        }
        public class OrderInfo
        {
            public string PrimaryDomain { get; set; }
            public string[] Domains { get; set; }
            public IdentifierType IdentifierType { get; set; }
            //public string KeyType { get; set; }
            public string 备注 { get; set; }
        }
    }
}
