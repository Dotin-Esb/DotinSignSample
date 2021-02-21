using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Jose;

namespace DotinSignSample
{
    public static class Utility
    {
        public static string GenerateSign(string pichakReqBody,string callerTerminalName, string callerBranchCode, string callerBranchUserName, string customerAuthStatus, string certificateThumbPrint)
        {
          
            var payload = customerAuthStatus +
                          callerTerminalName +
                          callerBranchCode +
                          callerBranchUserName +
                          pichakReqBody;

            return GenerateJwt(certificateThumbPrint, payload, true, detachPayload: true);
        }

        private static string GenerateJwt(string certificateFingerPrint, string payload, bool addCertificateHeader = false, Dictionary<string, object> extraHeaders = null, bool detachPayload = false)
        {
            var certCollectionStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            certCollectionStore.Open(OpenFlags.ReadOnly);
            var collection = certCollectionStore.Certificates.Find(X509FindType.FindByThumbprint, certificateFingerPrint, false);
            if (collection.Count == 0)
            {
                certCollectionStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certCollectionStore.Open(OpenFlags.ReadOnly);
                certCollectionStore.Certificates.Find(X509FindType.FindByThumbprint, certificateFingerPrint, false);
                collection = certCollectionStore.Certificates.Find(X509FindType.FindByThumbprint, certificateFingerPrint, false);
            }
            var certificate = collection[0];
            certCollectionStore.Close();

            var certificateString = Convert.ToBase64String(certificate.Export(X509ContentType.Cert), Base64FormattingOptions.None);

            if (extraHeaders == null)
                extraHeaders = new Dictionary<string, object>();

            if (addCertificateHeader)
                extraHeaders.Add("x5c", new[] { certificateString });

            var options = new JwtOptions { DetachPayload = detachPayload };
            var token = JWT.Encode(payload, certificate.GetRSAPrivateKey(), JwsAlgorithm.RS256, extraHeaders, options: options);

            return token;
        }
    }
}
