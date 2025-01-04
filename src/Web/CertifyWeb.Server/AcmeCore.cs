using Certify.Models.Plugins;
using Certify.Models.Providers;
using Certify.Providers.DNS.Aliyun;

namespace TestProject1
{
    public class AcmeCore
    {
        //private readonly static IdnMapping _idnMapping = new IdnMapping();
        public static List<IDnsProviderProviderPlugin> DnsProviderProviders { get; set; } = new List<IDnsProviderProviderPlugin>();
        public static async Task<IDnsProvider> GetDnsProvider(string providerType, Dictionary<string, string> credentials, Dictionary<string, string> parameters, ILog log = null)
        {
            var dnsAPIProvider = new DnsProviderAliyun();
            var a1 = await dnsAPIProvider.InitProvider(credentials, parameters, log);
            return dnsAPIProvider;
            //IDnsProvider dnsAPIProvider = null;

            //if (!string.IsNullOrEmpty(providerType))
            //{
            //    List<IDnsProviderProviderPlugin> providerPlugins = DnsProviderProviders;
            //    foreach (IDnsProviderProviderPlugin providerPlugin in providerPlugins)
            //    {
            //        dnsAPIProvider = providerPlugin.GetProvider(providerPlugin.GetType(), providerType);
            //        if (dnsAPIProvider != null)
            //        {
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    return null;
            //}

            //if (dnsAPIProvider == null)
            //{
            //    // we don't have the requested provider available, plugin probably didn't load or ID is wrong
            //    if (providerType == "DNS01.API.MSDNS")
            //    {
            //        // We saved earlier that the MSDNS provider failed to load. It's now explicitly being requested, so log that failure.
            //        log?.Error("Failed to create MS DNS API Provider. Check Microsoft.Management.Infrastructure is available and install latest compatible Windows Management Framework: https://docs.microsoft.com/en-us/powershell/wmf/overview");
            //        return null;
            //    }
            //    else
            //    {
            //        log?.Error($"Cannot create requested DNS API Provider. Plugin did not load or provider ID is invalid: {providerType}");
            //        return null;
            //    }
            //}
            //else
            //{
            //    await dnsAPIProvider.InitProvider(credentials, parameters, log);
            //}

            //return dnsAPIProvider;
        }

    }
}