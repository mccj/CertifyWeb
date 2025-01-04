using Certes;
using Certes.Acme.Resource;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Cryptography;
using TestProject1;

namespace CertifyWeb.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AcmeCertificateController : ControllerBase
    {
        //private static readonly string[] Summaries = new[]
        //{
        //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        //};

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
            var _idnMapping = new IdnMapping();
            var acmeCore = new AcmeCore();
            var dnsAPIProvider = await AcmeCore.GetDnsProvider("",
                new System.Collections.Generic.Dictionary<string, string> {
                    {"accesskeyid","9d1iVcZKohGyMxFQ" },
                    {"accesskeysecret","DSw7DhfTFLR86khoBvGtKuO8giKO1r" }
                },
                new System.Collections.Generic.Dictionary<string, string> { }
            );

            var authzDomain = "cluster.ink";

            //创建新的 ACME 帐户
            var _certificateAuthorities = Certify.Models.CertificateAuthority.CoreCertificateAuthorities;
            ////Let's Encrypt
            //var acme = new AcmeContext(WellKnownServers.LetsEncryptV2);
            var acme = new AcmeContext(new Uri("https://acme-v02.api.letsencrypt.org/directory"));
            var account = await acme.NewAccount("mccj@qq.com", true);
            //ZeroSSL
            //var acme = new AcmeContext(new Uri("https://acme.zerossl.com/v2/DV90"));
            //var account = await acme.NewAccount("mccj@qq.com", true, "GHdIvPgCbFnTvT1UInZilg", "4TanG5iwyOuSJ4Uks6YOZu1-IMaSSB2NXw5PeS6KavCxXhFIbdojPg2TcZuITZSFXqNZQ0UZW1iYDBKOvXzEsQ","");

            //var acme = new AcmeContext(new Uri("https://acme.ssl.com/sslcom-dv-rsa"));
            //var account = await acme.NewAccount("mccj@qq.com", true, "f8f7fd2753f1", "erUWeYxn1y-_AY9RnYmbehs8iZRuWRj6W6ja3iPkVJU");

            //var acme = new AcmeContext(new Uri("https://api.buypass.com/acme/directory"));
            //var account = await acme.NewAccount("mccj@qq.com", true, "f8f7fd2753f1", "erUWeYxn1y-_AY9RnYmbehs8iZRuWRj6W6ja3iPkVJU");

            //var tos = await acme.TermsOfService();
            //var account1 = await acme.Account();
            //var accountInfo = await account.Resource();

            // Save the account key for later use
            var pemKey = acme.AccountKey.ToPem();
            //var der = acme.AccountKey.ToDer();



            //            //使用现有的 ACME 帐户
            //            //// Load the saved account key
            //            var pemKey = @"-----BEGIN EC PRIVATE KEY-----
            //MHcCAQEEIGWuCAtg4N0WlJ2UPXooBKdFWIr9v9LroE3iCZZYfZlzoAoGCCqGSM49
            //AwEHoUQDQgAEnI6InAF6yK3SDfwlyKrRalUxWp7DHOLBKwj9aFUQftpr7YAUBM1h
            //RzzSLxhiQGbGr3tGfS5PIFtALSRW2pWipA==
            //-----END EC PRIVATE KEY-----
            //";
            //            var accountKey = KeyFactory.FromPem(pemKey);
            //            var acme = new AcmeContext(WellKnownServers.LetsEncryptStagingV2, accountKey);
            //            var account = await acme.Account();

            //放置通配符证书订单（通配符证书需要 DNS 验证）
            var certificateIdentifiers = new[] {
                new Identifier{ Type= IdentifierType.Dns, Value= authzDomain },
                new Identifier{ Type= IdentifierType.Dns, Value= "*." + authzDomain },
                //new Identifier{ Type= IdentifierType.Dns, Value= "k8s." + authzDomain },
                new Identifier{ Type= IdentifierType.Dns, Value= "*.k8s." + authzDomain },
                //new Identifier{ Type= IdentifierType.Dns, Value= "prd." + authzDomain },
                new Identifier{ Type= IdentifierType.Dns, Value= "*.prd.k8s." + authzDomain },
                //new Identifier{ Type= IdentifierType.Dns, Value= "dev." + authzDomain },
                new Identifier{ Type= IdentifierType.Dns, Value= "*.dev.k8s." + authzDomain },
                //new Identifier{ Type= IdentifierType.Dns, Value= "app." + authzDomain },
                new Identifier{ Type= IdentifierType.Dns, Value= "*.app.k8s." + authzDomain },
                //new Identifier{ Type= IdentifierType.Dns, Value= "dbs." + authzDomain },
                new Identifier{ Type= IdentifierType.Dns, Value= "*.dbs.k8s." + authzDomain },
                //new Identifier{ Type= IdentifierType.Dns, Value= "test." + authzDomain },
                new Identifier{ Type= IdentifierType.Dns, Value= "*.test.k8s." + authzDomain }
            };
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
                var ss = new Certify.Models.Providers.DnsRecord() { RecordType = "TXT", ZoneId = "9855804ce5224f33a0f38e1fc669fe73", TargetDomainName = authzDomain, RecordName = txtRecordName, RecordValue = dnsTxt };
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
                 CommonName = _idnMapping.GetAscii(authzDomain)
             }, privateKey);

            //var csr = new Certes.Pkcs.CertificationRequestBuilder();
            //csr.AddName($"C=CA, ST=State, L=City, O=Dept, CN=*.example.com");

            //var dfds = await order.Finalize(csr.Generate());


            var certChain = await order.Download();
            //var certChain2 = await order.Download("ISRG X1 Root");


            var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certChain.Certificate.ToDer());
            var certFriendlyName = $"[Certify] {cert.GetEffectiveDateString()} to {cert.GetExpirationDateString()}";

            //var cert222 = new CertificateInfo(certChain, privateKey);

            var certPem11 = certChain.ToPem();
            //var certPem22 = certChain.Certificate.ToPem();
            //var ddd = certChain.Certificate.ToDer();

            var privkeyPem = privateKey.ToPem();
            var pfx = certChain.ToPfx(privateKey);
            var pfxBytes = pfx.Build(certFriendlyName, "`1q2w3e4r");//pfx文件


            System.IO.File.WriteAllBytes(authzDomain + ".pfx", pfxBytes);
            System.IO.File.WriteAllText(authzDomain + ".cer", certPem11);
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
    }
}
