﻿namespace Certify.Models
{
    public enum CertificateCleanupMode
    {
        None = 0,
        /// <summary>
        /// Clean up [Certify] expired certificates
        /// </summary>
        AfterExpiry = 1,
        /// <summary>
        /// Clean up [Certify] renewed certificates
        /// </summary>
        AfterRenewal = 2,
        /// <summary>
        /// Clean up all [Certify] certificates not currently managed
        /// </summary>
        FullCleanup = 3
    }

    public static class RenewalIntervalModes
    {
        /// <summary>
        /// Renew certs N days after last renewal
        /// </summary>
        public const string DaysAfterLastRenewal = "DaysAfterLastRenewal";

        /// <summary>
        /// Renew certs N days before expiry
        /// </summary>
        public const string DaysBeforeExpiry = "DaysBeforeExpiry";

        /// <summary>
        /// Renews after n% of certificate lifetime has elapsed
        /// </summary>
        public const string PercentageLifetime = "PercentageLifetime";
    }

    /// <summary>
    /// Note the settings specified here are mapped to CoreAppSettings
    /// </summary>
    public class Preferences : BindableBase
    {
        public bool EnableAppTelematics { get; set; } = true;

        public bool IgnoreStoppedSites { get; set; }

        public bool EnableValidationProxyAPI { get; set; } = true;

        public bool EnableEFS { get; set; }

        public bool EnableDNSValidationChecks { get; set; }

        public string? RenewalIntervalMode { get; set; } = RenewalIntervalModes.PercentageLifetime;

        public int RenewalIntervalDays { get; set; }

        public int MaxRenewalRequests { get; set; }

        public string? InstanceId { get; set; }

        public bool IsInstanceRegistered { get; set; }

        public string? Language { get; set; }

        public bool EnableHttpChallengeServer { get; set; } = true;

        public bool EnableCertificateCleanup { get; set; } = true;

        public CertificateCleanupMode? CertificateCleanupMode { get; set; }

        public string? DefaultCertificateStore { get; set; }

        public bool EnableStatusReporting { get; set; } = true;

        /// <summary>
        /// ID of default CA
        /// </summary>
        public string? DefaultCertificateAuthority { get; set; }

        /// <summary>
        /// Id of default credentials (password) to use for private keys etc
        /// </summary>
        public string? DefaultKeyCredentials { get; set; }

        /// <summary>
        /// If true, the app will decide which Certificate Authority to choose from the list of supported providers.
        /// The preferred provider will be chosen first, with fallback to any other supported (and configured) providers if a failure occurs.
        /// </summary>
        public bool EnableAutomaticCAFailover { get; set; }

        /// <summary>
        /// If true, PFX build favours older key store algorithms compatible with older OpenSSL etc
        /// </summary>
        public bool UseModernPFXAlgs { get; set; }

        /// <summary>
        /// If true, will allow plugins to load from appdata
        /// </summary>
        public bool IncludeExternalPlugins { get; set; }

        public string[] FeatureFlags { get; set; } = System.Array.Empty<string>();

        /// <summary>
        /// Server to use for Ntp time diagnostics
        /// </summary>
        public string? NtpServer { get; set; }

        /// <summary>
        /// If enabled, certificate manager plugins are used to check for ACME certificates managed outside of Certify on same machine
        /// </summary>
        public bool EnableExternalCertManagers { get; set; }

        /// <summary>
        /// Id of the data store conenction configuration to use. 0 is the default scheme using local SQLite
        /// </summary>
        public string ConfigDataStoreConnectionId { get; set; } = "0";

        /// <summary>
        /// If set, defines the default key type used for private keys
        /// </summary>
        public string? DefaultKeyType { get; set; }

        public bool EnableParallelRenewals { get; set; }

        /// <summary>
        /// If set, customizes the ACME retry interval for operations such as polling order status where Retry After not supported by CA
        /// </summary>
        public int DefaultACMERetryInterval { get; set; }

        /// <summary>
        /// If true, system CA roots etc are loaded during chain build to help validate chain
        /// </summary>
        public bool EnableIssuerCache { get; set; }
    }

    public static class FeatureFlags
    {
        /// <summary>
        /// Enable import/export UI
        /// </summary>
        public const string IMPORT_EXPORT = "IMPORT_EXPORT";

        /// <summary>
        /// Enable options for PFX pwd (global and per item credentials)
        /// </summary>
        public const string PRIVKEY_PWD = "PRIVKEY_PWD";

        /// <summary>
        /// Enable editor for custom Certificate Authorities
        /// </summary>
        public const string CA_EDITOR = "CA_EDITOR";

        /// <summary>
        /// Enable options for auto CA Failover
        /// </summary>
        public const string CA_FAILOVER = "CA_FAILOVER";

        /// <summary>
        /// Enable options for external cert managers
        /// </summary>
        public const string EXTERNAL_CERT_MANAGERS = "EXTERNAL_CERT_MANAGERS";

        /// <summary>
        /// Enable server connection UI
        /// </summary>
        public const string SERVER_CONNECTIONS = "SERVER_CONNECTIONS";

        /// <summary>
        /// Enable data store UI
        /// </summary>
        public const string DATA_STORES = "DATA_STORES";
    }
}
