﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Certify.Models.Config;
using Certify.Models.Providers;

namespace Certify.Models.Plugins
{

    public enum ACMECompatibilityMode
    {
        /// <summary>
        /// ACME provider should follow compatibility requirements for current Let's Encrypt service
        /// </summary>
        Standard = 1,
        /// <summary>
        /// ACME provider follows compatibility requirements for alternative ACME APIs which may include deviations from spec or different behaviours
        /// </summary>
        AltProvider1 = 2
    }

    public interface IACMEClientProvider
    {
        string GetProviderName();

        string GetAcmeBaseURI();

        Task<bool> InitProvider(ILog log = null, AccountDetails account = null);

        Task<Uri> GetAcmeTermsOfService();

        Task<string> GetAcmeAccountStatus();

        Task<ActionResult<AccountDetails>> AddNewAccountAndAcceptTOS(ILog log, string email, string eabKeyId, string eabKey, string eabKeyAlg);

        Task<bool> DeactivateAccount(ILog log);

        Task<ActionResult<AccountDetails>> UpdateAccount(ILog log, string email, bool termsAgreed);

        Task<PendingOrder> BeginCertificateOrder(ILog log, CertRequestConfig config, string orderUri = null);

        Task<StatusMessage> SubmitChallenge(ILog log, string challengeType, PendingAuthorization pendingAuthorization);

        Task<PendingAuthorization> CheckValidationCompleted(ILog log, string challengeType, PendingAuthorization pendingAuthorization);

        Task<ProcessStepResult> CompleteCertificateRequest(ILog log, CertRequestConfig config, string orderId, string pwd, string preferredChain);

        Task<StatusMessage> RevokeCertificate(ILog log, ManagedCertificate managedCertificate);

        Task<bool> ChangeAccountKey(ILog log);
    }

    public class PendingOrder
    {
        public PendingOrder() { }

        /// <summary>
        /// if failure message is provider a default failed pending order object is created
        /// </summary>
        /// <param name="failureMessage"></param>
        public PendingOrder(string failureMessage)
        {

            Authorizations = new List<PendingAuthorization> {
                                        new PendingAuthorization{
                                            IsFailure = true,
                                            AuthorizationError = failureMessage
                                        }
                                    };
        }

        public List<PendingAuthorization> Authorizations { get; set; }
        public string OrderUri { get; set; }
        public bool IsPendingAuthorizations { get; set; } = true;
    }
}
