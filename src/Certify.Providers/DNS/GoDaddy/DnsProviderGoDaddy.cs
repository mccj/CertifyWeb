﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Certify.Models.Config;
using Certify.Models.Plugins;
using Certify.Models.Providers;
using Newtonsoft.Json;

namespace Certify.Providers.DNS.GoDaddy
{
    public class DnsProviderGoDaddyProvider : PluginProviderBase<IDnsProvider, ChallengeProviderDefinition>, IDnsProviderProviderPlugin { }

    internal class Zone
    {
        public string Domain { get; set; }
        public string DomainId { get; set; }
    }

    internal class DnsRecordGoDaddy
    {
        public string data { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public int ttl { get; set; }
    }

    internal class DnsResultGoDaddy
    {
        public DnsRecord[] Result { get; set; }
    }

    /// <summary>
    /// GoDaddy DNS API Provider contributed by https://github.com/alphaz18
    /// </summary>
    public class DnsProviderGoDaddy : DnsProviderBase, IDnsProvider
    {
        private ILog _log;
        private HttpClient _client;
        private string _authKey;
        private string _authSecret;
        private const string _baseUri = "https://api.godaddy.com/v1/";
        private const string _listZonesUri = _baseUri + "domains?limit=1000";
        private const string _createRecordUri = _baseUri + "domains/{0}/records";
        private const string _listRecordsUri = _baseUri + "domains/{0}/records/{1}";
        private const string _deleteRecordUri = _baseUri + "domains/{0}/records/{1}/{2}";
        private const string _updateRecordUri = _baseUri + "domains/{0}/records/{1}/{2}";

        private int? _customPropagationDelay = null;
        public int PropagationDelaySeconds => (_customPropagationDelay != null ? (int)_customPropagationDelay : Definition.PropagationDelaySeconds);

        public string ProviderId => Definition.Id;

        public string ProviderTitle => Definition.Title;

        public string ProviderDescription => Definition.Description;

        public string ProviderHelpUrl => Definition.HelpUrl;

        public bool IsTestModeSupported => Definition.IsTestModeSupported;

        public List<ProviderParameter> ProviderParameters => Definition.ProviderParameters;

        public static ChallengeProviderDefinition Definition => new ChallengeProviderDefinition
        {
            Id = "DNS01.API.GoDaddy",
            Title = "GoDaddy DNS API",
            Description = "Validates via GoDaddy DNS APIs using credentials",
            HelpUrl = "https://docs.certifytheweb.com/docs/dns/providers/godaddy",
            PropagationDelaySeconds = 120,
            ProviderParameters = new List<ProviderParameter>{
                        new ProviderParameter{ Key="authkey", Name="Auth Key", IsRequired=true },
                        new ProviderParameter{ Key="authsecret", Name="Auth Secret", IsRequired=true },
                        new ProviderParameter{ Key="zoneid", Name="DNS Zone Id", IsRequired=true, IsPassword=false, IsCredential=false },
                        new ProviderParameter{ Key="propagationdelay", Name="Propagation Delay Seconds", IsRequired=false, IsPassword=false, Value="120", IsCredential=false }
                    },
            ChallengeType = Models.SupportedChallengeTypes.CHALLENGE_TYPE_DNS,
            Config = "Provider=Certify.Providers.DNS.GoDaddy",
            HandlerType = ChallengeHandlerType.INTERNAL
        };

        public DnsProviderGoDaddy()
        {
        }

        public async Task<ActionResult> Test()
        {
            // test connection and credentials
            try
            {
                var zones = await GetZones();
                if (zones != null && zones.Any())
                {
                    return new ActionResult { IsSuccess = true, Message = "Test Completed OK." };
                }
                else
                {
                    return new ActionResult { IsSuccess = true, Message = "Test completed, but no zones returned." };
                }
            }
            catch (Exception exp)
            {
                return new ActionResult { IsSuccess = false, Message = $"Test Failed: {exp.Message}" };
            }
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Add("Authorization", $"sso-key {_authKey}:{_authSecret}");

            return request;
        }

        private async Task<List<DnsRecord>> GetDnsRecords(string tldName)
        {
            var records = new List<DnsRecord>();
            var request = CreateRequest(HttpMethod.Get, $"{string.Format(_listRecordsUri, tldName, "TXT")}");

            var result = await _client.SendAsync(request);

            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                var dnsResult = JsonConvert.DeserializeObject<DnsRecordGoDaddy[]>(content);

                records.AddRange(dnsResult.Select(x => new DnsRecord
                {
                    RecordId = x.name,
                    RecordName = x.name,
                    RecordType = x.type,
                    RecordValue = x.data,
                    Data = x
                }));
            }
            else
            {
                throw new Exception($"Could not get DNS records for zone {tldName}. Result: {result.StatusCode} - {await result.Content.ReadAsStringAsync()}");
            }

            return records;
        }

        private async Task<ActionResult> AddDnsRecord(string zoneName, string recordname, string value)
        {
            var request = CreateRequest(new HttpMethod("PATCH"), string.Format(_createRecordUri, zoneName));

            var rec = new DnsRecordGoDaddy
            {
                type = "TXT",
                name = recordname,
                data = value,
                ttl = 600
            };

            var recarr = new object[] { rec };

            request.Content = new StringContent(
                JsonConvert.SerializeObject(recarr)
                );

            request.Content.Headers.ContentType.MediaType = "application/json";

            var result = await _client.SendAsync(request);

            if (!result.IsSuccessStatusCode)
            {
                return new ActionResult
                {
                    IsSuccess = false,
                    Message = $"Could not add dns record {recordname} to zone {zoneName}. Result: {result.StatusCode} - {await result.Content.ReadAsStringAsync()}"
                };
            }
            else
            {
                return new ActionResult
                {
                    IsSuccess = true,
                    Message = "DNS record added."
                };
            }
        }

        private async Task<ActionResult> UpdateDnsRecord(string zoneName, DnsRecord record, string value)
        {
            await Test();

            var request = CreateRequest(HttpMethod.Put, string.Format(_updateRecordUri, zoneName, record.RecordType, record.RecordName));

            request.Content = new StringContent(
                JsonConvert.SerializeObject(new object[] { new
                        {
                            data = value,
                            ttl = 600
                        }
                    })
                );

            request.Content.Headers.ContentType.MediaType = "application/json";

            var result = await _client.SendAsync(request);

            if (!result.IsSuccessStatusCode)
            {
                return new ActionResult
                {
                    IsSuccess = false,
                    Message = $"Could not update dns record {record.RecordName} to zone {zoneName}. Result: {result.StatusCode} - {await result.Content.ReadAsStringAsync()}"
                };
            }
            else
            {
                return new ActionResult { IsSuccess = true, Message = "DNS record updated" };
            }
        }

        public async Task<ActionResult> CreateRecord(DnsRecord request)
        {
            //TODO: check if record already exists and update instead
            var root = await DetermineZoneDomainRoot(request.RecordName, request.ZoneId);
            var recordName = NormaliseRecordName(root, request.RecordName);
            return await AddDnsRecord(root.RootDomain, recordName, request.RecordValue);
        }

        public async Task<ActionResult> DeleteRecord(DnsRecord request)
        {
            // grab all the txt records for the zone as a json array, remove the txt record in
            // question, and send an update command.

            var root = await DetermineZoneDomainRoot(request.RecordName, request.ZoneId);
            var recordName = NormaliseRecordName(root, request.RecordName);

            var domainrecords = await GetDnsRecords(root.RootDomain);

            if (!domainrecords.Any())
            {
                return new ActionResult { IsSuccess = true, Message = "DNS record delete: nothing to do." };
            }

            var recordsToRemove = domainrecords.Where(x => x.RecordName + "." + root.RootDomain == request.RecordName).ToList();
            if (!recordsToRemove.Any())
            {
                return new ActionResult { IsSuccess = true, Message = "DNS record does not exist, nothing to delete." };
            }

            // API now supports a delete method
            var req = CreateRequest(HttpMethod.Delete, string.Format(_deleteRecordUri, root.RootDomain, "TXT", recordName));
            var result = await _client.SendAsync(req);

            if (result.IsSuccessStatusCode)
            {
                return new ActionResult { IsSuccess = true, Message = "DNS record deleted." };
            }
            else
            {
                return new ActionResult
                {
                    IsSuccess = false,
                    Message = $"Could not delete record {request.RecordName}. Result: {result.StatusCode} - {await result.Content.ReadAsStringAsync()}"
                };
            }
        }

        public override async Task<List<DnsZone>> GetZones()
        {
            var zones = new List<DnsZone>();

            var request = CreateRequest(HttpMethod.Get, $"{_listZonesUri}");

            var result = await _client.SendAsync(request);

            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                var zonesResult = JsonConvert.DeserializeObject<IEnumerable<Zone>>(content).ToList();

                foreach (var zone in zonesResult)
                {
                    // DomainId is not used by the GoDaddy API, so we use the domain as the ID
                    zones.Add(new DnsZone { ZoneId = zone.Domain, Name = zone.Domain });
                }
            }
            else
            {
                return new List<DnsZone>();
            }

            return zones;
        }

        public async Task<bool> InitProvider(Dictionary<string, string> credentials, Dictionary<string, string> parameters, ILog log = null)
        {
            _log = log;

            _authKey = credentials["authkey"];
            _authSecret = credentials["authsecret"];

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (parameters?.ContainsKey("propagationdelay") == true)
            {
                if (int.TryParse(parameters["propagationdelay"], out int customPropDelay))
                {
                    _customPropagationDelay = customPropDelay;
                }
            }

            return await Task.FromResult(true);
        }
    }
}
