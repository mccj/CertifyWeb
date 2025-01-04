﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Certify.Models.Providers
{
    /// <summary>
    /// An example certified server would be an IIS server 
    /// </summary>
    public interface ICertifiedServer : IDisposable
    {
        Task<List<BindingInfo>> GetSiteBindingList(
            bool ignoreStoppedSites,
            string siteId = null
            );

        Task<List<BindingInfo>> GetPrimarySites(bool ignoreStoppedSites);

        Task<SiteInfo> GetSiteById(string siteId);

        Task RemoveHttpsBinding(string siteId, string sni);

        Task<Version> GetServerVersion();

        Task<bool> IsAvailable();

        Task<bool> IsSiteRunning(string id);

        IBindingDeploymentTarget GetDeploymentTarget();

        Task<List<ActionStep>> RunConfigurationDiagnostics(string siteId);
    }
}
