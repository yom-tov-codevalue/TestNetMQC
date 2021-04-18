using System;
using NetMQ;

namespace Shared
{
    public static class CertificatesManager
    {
        private static readonly NetMQCertificate _serverCert;

        static CertificatesManager()
        {
            var serverKey = new byte[32];
            var rnd = new Random();
            rnd.NextBytes(serverKey);
            _serverCert = NetMQCertificate.CreateFromSecretKey(serverKey);
        }

        public static NetMQCertificate GetServerCertificate() => _serverCert;
    }
}
