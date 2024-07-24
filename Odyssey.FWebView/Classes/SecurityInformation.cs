using System.Collections.Generic;

namespace Odyssey.FWebView.Classes
{
    public class CertificateSecurityState
    {
        public List<string> Certificate { get; set; }
        public bool CertificateHasSha1Signature { get; set; }
        public bool CertificateHasWeakSignature { get; set; }
        public string Cipher { get; set; }
        public string Issuer { get; set; }
        public string KeyExchange { get; set; }
        public string KeyExchangeGroup { get; set; }
        public bool ModernSSL { get; set; }
        public bool ObsoleteSslCipher { get; set; }
        public bool ObsoleteSslKeyExchange { get; set; }
        public bool ObsoleteSslProtocol { get; set; }
        public bool ObsoleteSslSignature { get; set; }
        public string Protocol { get; set; }
        public string SubjectName { get; set; }
        public int ValidFrom { get; set; }
        public int ValidTo { get; set; }
    }

    public class VisibleSecurityState
    {
        public CertificateSecurityState CertificateSecurityState { get; set; }
        public string SecurityState { get; set; }
        public List<object> SecurityStateIssueIds { get; set; }
    }

    public class SecurityInformation
    {
        public VisibleSecurityState VisibleSecurityState { get; set; }

    }
}
