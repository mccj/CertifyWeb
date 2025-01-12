using Certify.Models;
using Certify.Models.Config;
using Certify.Models.Plugins;
using Certify.Models.Providers;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace TestProject1
{
    public class PluginManager
    {
        PluginManager() { }
        static PluginManager()
        {
            CurrentInstance = new PluginManager();
            CurrentInstance.LoadPlugins(false);
        }
        public static PluginManager CurrentInstance { get; private set; }
        public static async Task<List<ChallengeProviderDefinition>> GetChallengeAPIProviders()
        {
            var result = PluginManager.CurrentInstance.DnsProviderProviders.SelectMany(pp => pp.GetProviders(pp.GetType())).ToList();
            return await Task.FromResult(result);
        }
        public static async Task<IDnsProvider> GetDnsProvider(string providerType, Dictionary<string, string> credentials, Dictionary<string, string> parameters, ILog log = null)
        {
            IDnsProvider dnsAPIProvider = null;

            if (!string.IsNullOrEmpty(providerType))
            {
                var providerPlugins = PluginManager.CurrentInstance.DnsProviderProviders;
                foreach (var providerPlugin in providerPlugins)
                {
                    dnsAPIProvider = providerPlugin.GetProvider(providerPlugin.GetType(), providerType);
                    if (dnsAPIProvider != null)
                    {
                        break;
                    }
                }
            }
            else
            {
                return null;
            }

            if (dnsAPIProvider == null)
            {
                // we don't have the requested provider available, plugin probably didn't load or ID is wrong
                if (providerType == "DNS01.API.MSDNS")
                {
                    // We saved earlier that the MSDNS provider failed to load. It's now explicitly being requested, so log that failure.
                    log?.Error("Failed to create MS DNS API Provider. Check Microsoft.Management.Infrastructure is available and install latest compatible Windows Management Framework: https://docs.microsoft.com/en-us/powershell/wmf/overview");
                    return null;
                }
                else
                {
                    log?.Error($"Cannot create requested DNS API Provider. Plugin did not load or provider ID is invalid: {providerType}");
                    return null;
                }
            }
            else
            {
                await dnsAPIProvider.InitProvider(credentials, parameters, log);
            }

            return dnsAPIProvider;
        }

        //private readonly static IdnMapping _idnMapping = new IdnMapping();
        //public static IDnsProviderProviderPlugin[] DnsProviderProviders { get; set; } = [
        //    new Certify.Providers.DNS.AWSRoute53.DnsProviderAWSRoute53Provider()
        //    ];
        //public static async Task<IDnsProvider> GetDnsProvider(string providerType, Dictionary<string, string> credentials, Dictionary<string, string> parameters, ILog log = null)
        //{
        //    var dnsAPIProvider = new DnsProviderAliyun();
        //    var a1 = await dnsAPIProvider.InitProvider(credentials, parameters, log);
        //    return dnsAPIProvider;
        //    //IDnsProvider dnsAPIProvider = null;

        //    //if (!string.IsNullOrEmpty(providerType))
        //    //{
        //    //    List<IDnsProviderProviderPlugin> providerPlugins = DnsProviderProviders;
        //    //    foreach (IDnsProviderProviderPlugin providerPlugin in providerPlugins)
        //    //    {
        //    //        dnsAPIProvider = providerPlugin.GetProvider(providerPlugin.GetType(), providerType);
        //    //        if (dnsAPIProvider != null)
        //    //        {
        //    //            break;
        //    //        }
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    return null;
        //    //}

        //    //if (dnsAPIProvider == null)
        //    //{
        //    //    // we don't have the requested provider available, plugin probably didn't load or ID is wrong
        //    //    if (providerType == "DNS01.API.MSDNS")
        //    //    {
        //    //        // We saved earlier that the MSDNS provider failed to load. It's now explicitly being requested, so log that failure.
        //    //        log?.Error("Failed to create MS DNS API Provider. Check Microsoft.Management.Infrastructure is available and install latest compatible Windows Management Framework: https://docs.microsoft.com/en-us/powershell/wmf/overview");
        //    //        return null;
        //    //    }
        //    //    else
        //    //    {
        //    //        log?.Error($"Cannot create requested DNS API Provider. Plugin did not load or provider ID is invalid: {providerType}");
        //    //        return null;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    await dnsAPIProvider.InitProvider(credentials, parameters, log);
        //    //}

        //    //return dnsAPIProvider;
        //}

        public List<IDnsProviderProviderPlugin> DnsProviderProviders { get; set; }

        public bool EnableExternalPlugins { get; set; } = false;
        public List<PluginLoadResult> PluginLoadResults { get; private set; } = new List<PluginLoadResult>();

        public void LoadPlugins(bool usePluginSubfolder = true)
        {
            DnsProviderProviders = new List<IDnsProviderProviderPlugin>();

            //// load core providers as plugins
            //var builtInProvider = new BuiltinDnsProviderProvider();//(IDnsProviderProviderPlugin)Activator.CreateInstance(Type.GetType("Certify.Core.Management.Challenges.ChallengeProviders+BuiltinDnsProviderProvider, Certify.Core"));
            //DnsProviderProviders.Add(builtInProvider);

            ////var poshAcmeProvider = (IDnsProviderProviderPlugin)Activator.CreateInstance(Type.GetType("Certify.Core.Management.Challenges.DNS.DnsProviderPoshACME+PoshACMEDnsProviderProvider, Certify.Shared.Extensions"));
            //var poshAcmeProvider = new Certify.Core.Management.Challenges.DNS.DnsProviderPoshACME.PoshACMEDnsProviderProvider();
            //DnsProviderProviders.Add(poshAcmeProvider);

            var otherProviders = LoadPlugins<IDnsProviderProviderPlugin>("Plugin.DNS.*.dll", usePluginSubfolder: usePluginSubfolder);
            DnsProviderProviders.AddRange(otherProviders);

            if (EnableExternalPlugins)
            {
                var customPlugins = LoadPlugins<IDnsProviderProviderPlugin>("*.dll", loadFromAppData: true);
                if (customPlugins.Any())
                {
                    DnsProviderProviders.AddRange(customPlugins);
                }
            }
        }
        private string GetPluginFolderPath(bool usePluginSubfolder = true, bool useAppData = false)
        {
            if (!useAppData)
            {
                var executableLocation = AppContext.BaseDirectory;
                if (string.IsNullOrEmpty(executableLocation))
                {
                    throw new ArgumentNullException("GetPluginFolderPath: Executing assembly location is null");
                }

                if (usePluginSubfolder)
                {
                    var path = Path.Combine(executableLocation, "plugins");
                    return path;
                }
                else
                {
                    return Path.GetDirectoryName(executableLocation);
                }
            }
            else
            {
                return EnvironmentUtil.CreateAppDataPath("plugins");
            }
        }

        private T LoadPlugin<T>(string dllFileName, string pluginFolder = null)
        {
            var interfaceType = typeof(T);
            var pluginPath = string.Empty;

            try
            {
                pluginPath = pluginFolder != null ? Path.Combine(pluginFolder, dllFileName) : Path.Combine(GetPluginFolderPath(), dllFileName);

                if (!File.Exists(pluginPath))
                {
                    pluginPath = Path.Combine(GetPluginFolderPath(usePluginSubfolder: false), dllFileName);
                }

                if (File.Exists(pluginPath))
                {
                    // https://stackoverflow.com/questions/10732933/can-i-use-activator-createinstance-with-an-interface
                    var pluginAssembly = Assembly.LoadFrom(pluginPath);

                    var exportedTypes = pluginAssembly.GetExportedTypes();

                    var pluginType = pluginAssembly.GetTypes()
                        .Where(type => type.GetInterfaces()
                        .Any(inter => inter.IsAssignableFrom(interfaceType)))
                        .FirstOrDefault();

                    if (pluginType != null)
                    {
                        var obj = (T)Activator.CreateInstance(pluginType);

                        return obj;
                    }
                    else
                    {
                        //_log?.Debug($"Plugin Load Skipped [{interfaceType}] File does not contain a matching interface: {dllFileName} in {pluginPath}");
                    }
                }
                else
                {
                    //_log?.Warning($"Plugin Load Failed [{interfaceType}] File does not exist: {dllFileName} in {pluginPath}");
                }
            }
            catch (ReflectionTypeLoadException ex)
            {

                //_log?.Warning($"Plugin Load Failed [{interfaceType}] :: {dllFileName} [Reflection or Loader Error] in {pluginPath}");

                //_log.Error(ex.ToString());
                //foreach (var loaderEx in ex.LoaderExceptions)
                //{
                //    _log.Error(loaderEx.ToString());
                //}
            }
            catch (Exception exp)
            {
                //_log?.Error(exp.ToString());
            }

            return default;
        }
        private List<T> LoadPlugins<T>(string fileMatch, bool loadFromAppData = false, bool usePluginSubfolder = true)
        {
            var plugins = new List<T>();

            var pluginDir = new DirectoryInfo(GetPluginFolderPath(usePluginSubfolder));

            if (loadFromAppData)
            {
                pluginDir = new DirectoryInfo(GetPluginFolderPath(useAppData: true));
            }

            if (!pluginDir.Exists)
            {
                return new List<T> { };
            }

            var pluginAssemblyFiles = pluginDir.GetFiles(fileMatch);

            var discoveredPlugins = pluginAssemblyFiles.Select(assem =>
            {

                try
                {
                    var result = LoadPlugin<T>(assem.Name, pluginDir.FullName);

                    if (result != null)
                    {
                        PluginLoadResults.Add(new PluginLoadResult(assem.Name, $"Loaded plugin: {assem.Name}", true));
                    }
                    else
                    {
                        PluginLoadResults.Add(new PluginLoadResult(assem.Name, $"Failed to load plugin: {assem.Name}", false));
                    }

                    return result;
                }
                catch (Exception exp)
                {
                    // failed to load plugin
                    PluginLoadResults.Add(new PluginLoadResult(assem.Name, $"Failed to load plugin: {assem.Name} {exp}", false));
                    return default;
                }
            })
            .Where(p => p != null)
            .ToList();

            plugins.AddRange(discoveredPlugins);

            return plugins;
        }
    }

    public class PluginLoadResult : Certify.Models.Config.ActionResult
    {
        public string PluginName { get; set; }

        public PluginLoadResult(string name, string msg, bool isSuccess)
        {
            PluginName = name;
            Message = msg;
            IsSuccess = isSuccess;
        }
    }
    public class BuiltinDnsProviderProvider : IDnsProviderProviderPlugin
    {
        private static List<ChallengeProviderDefinition> _providers;

        static BuiltinDnsProviderProvider()
        {
            _providers = new List<ChallengeProviderDefinition>()
                {
                    // IIS
                    new ChallengeProviderDefinition
                    {
                        Id = "HTTP01.IIS.Local",
                        ChallengeType = SupportedChallengeTypes.CHALLENGE_TYPE_HTTP,
                        Title = "Local IIS Server",
                        Description = "Validates via standard http website bindings on port 80",
                        HandlerType = ChallengeHandlerType.INTERNAL
                    },

                    // Fake challenge type for UN/PW authentication
                    new ChallengeProviderDefinition
                    {
                        Id = StandardAuthTypes.STANDARD_AUTH_GENERIC,
                        ChallengeType = "",
                        Title = "Username and Password",
                        Description = "Standard username and password credentials",
                        HandlerType = ChallengeHandlerType.INTERNAL,
                        ProviderParameters= new List<ProviderParameter>
                        {
                           new ProviderParameter{ Key="username",Name="Username", IsRequired=true, IsPassword=false, IsCredential=true },
                           new ProviderParameter{ Key="password",Name="Password", IsRequired=true, IsPassword=true, IsCredential=true },
                        }
                    },

                    // Fake challenge type for password-only authentication
                    new ChallengeProviderDefinition
                    {
                        Id = StandardAuthTypes.STANDARD_AUTH_PASSWORD,
                        ChallengeType = "",
                        Title = "Password",
                        Description = "Standard Password credential",
                        HandlerType = ChallengeHandlerType.INTERNAL,
                        ProviderParameters= new List<ProviderParameter>
                        {
                           new ProviderParameter{ Key="password",Name="Password", IsRequired=true, IsPassword=true, IsCredential=true },
                        }
                    },

                      // Fake challenge type for api token authentication
                    new ChallengeProviderDefinition
                    {
                        Id = StandardAuthTypes.STANDARD_AUTH_API_TOKEN,
                        ChallengeType = "",
                        Title = "API Key or Token",
                        Description = "Generic API Token or Key",
                        HandlerType = ChallengeHandlerType.INTERNAL,
                        ProviderParameters= new List<ProviderParameter>
                        {
                           new ProviderParameter{ Key="api_token",Name="API Key or Token", IsRequired=true, IsPassword=true, IsCredential=true },
                        }
                    },

                    // Fake challenge type for Windows network credentials
                    new ChallengeProviderDefinition
                    {
                        Id = StandardAuthTypes.STANDARD_AUTH_WINDOWS,
                        ChallengeType = "",
                        Title = "Windows Credentials (Network)",
                        Description = "Windows username and password credentials",
                        HandlerType = ChallengeHandlerType.INTERNAL,
                        ProviderParameters= new List<ProviderParameter>
                        {
                           new ProviderParameter{ Key="domain",Name="Domain", IsRequired=false, IsPassword=false, IsCredential=true },
                           new ProviderParameter{ Key="username",Name="Username", IsRequired=true, IsPassword=false, IsCredential=true },
                           new ProviderParameter{ Key="password",Name="Password", IsRequired=true, IsPassword=true, IsCredential=true },
                        }
                    },

                    // Fake challenge type for Windows impersonation credentials
                    new ChallengeProviderDefinition
                    {
                        Id = StandardAuthTypes.STANDARD_AUTH_LOCAL_AS_USER,
                        ChallengeType = "",
                        Title = "Windows Credentials (Local)",
                        Description = "Windows username and password credentials",
                        HandlerType = ChallengeHandlerType.INTERNAL,
                        ProviderParameters= new List<ProviderParameter>
                        {
                           new ProviderParameter{ Key="domain",Name="Domain", IsRequired=false, IsPassword=false, IsCredential=true, Description="(optional)" },
                           new ProviderParameter{ Key="username",Name="Username", IsRequired=true, IsPassword=false, IsCredential=true },
                           new ProviderParameter{ Key="password",Name="Password", IsRequired=true, IsPassword=true, IsCredential=true },
                        }
                    },

                    // Fake challenge type for SSH UN/PW or UN/key credentials
                    new ChallengeProviderDefinition
                    {
                        Id = StandardAuthTypes.STANDARD_AUTH_SSH,
                        ChallengeType = "",
                        Title = "SSH Credentials",
                        Description = "SSH username, password and private key credentials",
                        HandlerType = ChallengeHandlerType.INTERNAL,
                        ProviderParameters= new List<ProviderParameter>
                        {
                           new ProviderParameter{ Key="username",Name="Username", IsRequired=true, IsPassword=false, IsCredential=true },
                           new ProviderParameter{ Key="password",Name="Password", IsRequired=false, IsPassword=true, IsCredential=true, Description="Optional password" },
                           new ProviderParameter{ Key="privatekey",Name="Private Key File Path", IsRequired=false, IsPassword=false, IsCredential=true, Description="Optional path private key file" },
                           new ProviderParameter{ Key="key_passphrase",Name="Private Key Passphrase", IsRequired=false, IsPassword=true, IsCredential=true , Description="Optional key passphrase"},
                        }
                    },

                    // Fake challenge type for Azure AD OAuth client credentials
                    new ChallengeProviderDefinition
                    {
                        Id = "ExternalAuth.Azure.ClientSecret",
                        Title = "Azure AD Application Client Secret",
                        Description = "Azure AD Application user and client secret",
                        HelpUrl="https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal",
                        ProviderParameters = new List<ProviderParameter>{
                            new ProviderParameter{Key="tenantid", Name="Directory (tenant) Id", IsRequired=true, IsCredential=true },
                            new ProviderParameter{Key="clientid", Name="Application (client) Id", IsRequired=true, IsCredential=true },
                            new ProviderParameter{Key="secret",Name="Client Secret", IsRequired=true , IsPassword=true}
                        },
                        ChallengeType = "",
                        HandlerType = ChallengeHandlerType.INTERNAL
                    },

                    // DNS by pausing and e-mailing a manual request
                    DnsProviderManual.Definition,

                    // DNS by using a PowerShell script
                    DnsProviderScripting.Definition,

                    // ISSUE: Apache Libcloud's Python provider has no definitions.
                };
        }

        public IDnsProvider GetProvider(Type pluginType, string id)
        {
            if (id == DnsProviderManual.Definition.Id)
            {
                return new DnsProviderManual();
            }
            else if (id == DnsProviderScripting.Definition.Id)
            {
                return new DnsProviderScripting();
            }
            else
            {
                return null;
            }
        }

        public List<ChallengeProviderDefinition> GetProviders(Type pluginType)
        {
            return _providers.ToList(); // Return a copy so it can't be inadvertently mutated
        }
    }
    public class DnsProviderManual : IDnsProvider
    {
        int IDnsProvider.PropagationDelaySeconds => Definition.PropagationDelaySeconds;

        string IDnsProvider.ProviderId => Definition.Id;

        string IDnsProvider.ProviderTitle => Definition.Title;

        string IDnsProvider.ProviderDescription => Definition.Description;

        string IDnsProvider.ProviderHelpUrl => Definition.HelpUrl;

        bool IDnsProvider.IsTestModeSupported => Definition.IsTestModeSupported;

        List<ProviderParameter> IDnsProvider.ProviderParameters => Definition.ProviderParameters;

        private ILog _log;

        public static ChallengeProviderDefinition Definition => new ChallengeProviderDefinition
        {
            Id = "DNS01.Manual",
            Title = "(Update DNS Manually)",
            Description = "When a DNS update is required, wait for manual changes.",
            HelpUrl = "https://docs.certifytheweb.com/docs/dns/validation",
            PropagationDelaySeconds = -1,
            ProviderParameters = new List<ProviderParameter>() { new ProviderParameter { Description = "Email address to prompt changes", IsRequired = false, Key = "email", Name = "Email to Notify (optional)", IsCredential = false } },
            ChallengeType = Certify.Models.SupportedChallengeTypes.CHALLENGE_TYPE_DNS,
            Config = "Provider=Certify.Providers.DNS.Manual",
            HandlerType = ChallengeHandlerType.MANUAL,
            IsTestModeSupported = false
        };

        public DnsProviderManual()
        {
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        public async Task<ActionResult> CreateRecord(DnsRecord request) => new ActionResult
        {
            IsSuccess = true,
            Message = $"Please login to your DNS control panel for the domain '{request.TargetDomainName}' and create a new TXT record named: \r\n\t{request.RecordName} \r\nwith the value:\r\n\t{request.RecordValue}"
        };

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        public async Task<ActionResult> DeleteRecord(DnsRecord request) => new ActionResult
        {
            IsSuccess = true,
            Message = $"Please login to your DNS control panel for the domain '{request.TargetDomainName}' and delete the TXT record named '{request.RecordName}'."
        };

        Task<List<DnsZone>> IDnsProvider.GetZones() => Task.FromResult(new List<DnsZone>());

        Task<bool> IDnsProvider.InitProvider(Dictionary<string, string> credentials, Dictionary<string, string> parameters, ILog log)
        {
            _log = log;

            return Task.FromResult(true);
        }

        async Task<ActionResult> IDnsProvider.Test() => await Task.FromResult(new ActionResult { IsSuccess = true, Message = "The user will manually update DNS as required." });
    }
    public class DnsProviderScripting : IDnsProvider
    {
        private ILog _log;

        int IDnsProvider.PropagationDelaySeconds => (_customPropagationDelay != null ? (int)_customPropagationDelay : Definition.PropagationDelaySeconds);

        string IDnsProvider.ProviderId => Definition.Id;

        string IDnsProvider.ProviderTitle => Definition.Title;

        string IDnsProvider.ProviderDescription => Definition.Description;

        string IDnsProvider.ProviderHelpUrl => Definition.HelpUrl;

        public bool IsTestModeSupported => Definition.IsTestModeSupported;

        List<ProviderParameter> IDnsProvider.ProviderParameters => Definition.ProviderParameters;

        private string _createScriptPath = "";
        private string _deleteScriptPath = "";
        private int? _customPropagationDelay = null;

        public static ChallengeProviderDefinition Definition => new ChallengeProviderDefinition
        {
            Id = "DNS01.Scripting",
            Title = "(Use Custom Script)",
            Description = "Validates DNS challenges via a user provided custom script",
            HelpUrl = "https://docs.certifytheweb.com/docs/dns/providers/scripting",
            PropagationDelaySeconds = 60,
            ProviderParameters = new List<ProviderParameter>{
                        new ProviderParameter{ Key="createscriptpath", Name="Create Script Path", IsRequired=true , IsCredential=false},
                        new ProviderParameter{ Key="deletescriptpath", Name="Delete Script Path", IsRequired=false, IsCredential=false },
                        new ProviderParameter{ Key="propagationdelay",Name="Propagation Delay Seconds (optional)", IsRequired=false, IsPassword=false, Value="60", IsCredential=false },
                        new ProviderParameter{ Key="zoneid",Name="Dns Zone Id (optional)", IsRequired=false, IsPassword=false, IsCredential=false }
                    },
            ChallengeType = Certify.Models.SupportedChallengeTypes.CHALLENGE_TYPE_DNS,
            Config = "Provider=Certify.Providers.DNS.Scripting",
            HandlerType = ChallengeHandlerType.CUSTOM_SCRIPT
        };

        public DnsProviderScripting()
        {
        }

        public async Task<ActionResult> CreateRecord(DnsRecord request)
        {
            if (!string.IsNullOrEmpty(_createScriptPath))
            {
                // standard parameters are the subject domain/subdomain, full txt record name to
                // create, txt record value, zone id
                var parameters = $"{request.TargetDomainName} {request.RecordName} {request.RecordValue} {request.ZoneId}";
                return await RunScript(_createScriptPath, parameters);
            }
            else
            {
                return new ActionResult { IsSuccess = false, Message = "Dns Scripting: No Create Script Path provided." };
            }
        }

        public async Task<ActionResult> DeleteRecord(DnsRecord request)
        {
            if (!string.IsNullOrEmpty(_deleteScriptPath))
            {
                // standard parameters are the subject domain/subdomain, full txt record name to
                // create, txt record value, zone id
                var parameters = $"{request.TargetDomainName} {request.RecordName} {request.RecordValue} {request.ZoneId}";
                return await RunScript(_deleteScriptPath, parameters);
            }
            else
            {
                return new ActionResult { IsSuccess = true, Message = "Dns Scripting: No Delete Script Path provided (skipped delete)." };
            }
        }

        Task<List<DnsZone>> IDnsProvider.GetZones() => Task.FromResult(new List<DnsZone>());

        Task<bool> IDnsProvider.InitProvider(Dictionary<string, string> credentials, Dictionary<string, string> parameters, ILog log)
        {
            _log = log;

            if (parameters?.ContainsKey("createscriptpath") == true)
            {
                _createScriptPath = parameters["createscriptpath"];
            }

            if (parameters?.ContainsKey("deletescriptpath") == true)
            {
                _deleteScriptPath = parameters["deletescriptpath"];
            }

            if (parameters?.ContainsKey("propagationdelay") == true)
            {
                if (int.TryParse(parameters["propagationdelay"], out var customPropDelay))
                {
                    _customPropagationDelay = customPropDelay;
                }
            }

            return Task.FromResult(true);
        }

        Task<ActionResult> IDnsProvider.Test() => Task.FromResult(new ActionResult
        {
            IsSuccess = true,
            Message = "Test skipped for scripted DNS. No test available."
        });

        private async Task<ActionResult> RunScript(string script, string parameters)
        {
            var _log = new StringBuilder();
            // https://stackoverflow.com/questions/5519328/executing-batch-file-in-c-sharp and
            // attempting to have some argument compat with https://github.com/PKISharp/win-acme/blob/master/letsencrypt-win-simple/Plugins/ValidationPlugins/Dns/Script.cs

            var scriptProcessInfo = new ProcessStartInfo(Environment.ExpandEnvironmentVariables(script))
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (!string.IsNullOrWhiteSpace(parameters))
            {
                scriptProcessInfo.Arguments = parameters;
            }
            else
            {
                _log.AppendLine($"{Definition.Title}: Running DNS script [{script} {parameters}]");
            }

            try
            {
                var process = new Process { StartInfo = scriptProcessInfo };

                var logMessages = new StringBuilder();
                var errorLineCount = 0;

                // capture output streams and add to log
                process.OutputDataReceived += (obj, args) =>
                {
                    if (args.Data != null)
                    {
                        logMessages.AppendLine(args.Data);
                    }
                };

                process.ErrorDataReceived += (obj, args) =>
                {
                    // allow up to 4K lines of errors, after that we're probably in some sort of loop and there's no point running out of ram for the log
                    if (errorLineCount < 4000)
                    {
                        if (!string.IsNullOrWhiteSpace(args.Data))
                        {
                            logMessages.AppendLine($"Error: {args.Data}");
                            errorLineCount++;
                        }
                    }
                };

                try
                {
                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit((60 + Definition.PropagationDelaySeconds) * 1000);
                }
                catch (Exception exp)
                {
                    _log.AppendLine("Error Running Script: " + exp.ToString());
                }

                // append output to main log
                _log.Append(logMessages.ToString());

                if (!process.HasExited)
                {
                    //process still running, kill task
                    process.CloseMainWindow();

                    _log.AppendLine("Warning: Script ran but took too long to exit and was closed.");
                }
                else if (process.ExitCode != 0)
                {
                    _log.AppendLine("Warning: Script exited with the following ExitCode: " + process.ExitCode);
                }

                return await Task.FromResult(new ActionResult { IsSuccess = true, Message = _log.ToString() });
            }
            catch (Exception exp)
            {
                _log.AppendLine("Error: " + exp.ToString());
                return await Task.FromResult(new ActionResult { IsSuccess = false, Message = _log.ToString() });
            }
        }
    }
}