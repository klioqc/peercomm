using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.PeerResolvers;
using System.Security.Cryptography.X509Certificates;

namespace cir.PeerComm
{
    /// <summary>
    /// Creates a custom peer resolver service on the local machine.
    /// </summary>
    public class CustomPeerResolver
    {
        /// <summary>
        /// This is what does the work of loading the resolver service
        /// </summary>
        private ServiceHost _ResolverServiceHost;

        /// <summary>
        /// Handles Peer connections
        /// </summary>
        private CustomPeerResolverService _ResolverService;
        
        /// <summary>
        /// A reference to the actual service object
        /// </summary>
        public CustomPeerResolverService ResolverService
        {
            get { return _ResolverService; }
            set { _ResolverService = value; }
        }

        private string _ChannelName;
        /// <summary>
        /// The name of the channel the resolver will service
        /// </summary>
        public string ChannelName
        {
            get { return _ChannelName; }
            set { _ChannelName = value; }
        }

        private string _ChannelPassword;
        /// <summary>
        /// The password used to access the channel
        /// </summary>
        public string ChannelPassword
        {
            get { return _ChannelPassword; }
            set { _ChannelPassword = value; }
        }

        private X509Certificate2 _ChannelCert;
        /// <summary>
        /// The certificate used to encrypt the Secure Socket Layer
        /// </summary>
        public X509Certificate2 ChannelCert
        {
            get { return _ChannelCert; }
            set { _ChannelCert = value; }
        }

        private int _RefreshInterval = 2;
        /// <summary>
        /// How long before an inactive peer is marked for removal
        /// </summary>
        public int RefreshInterval
        {
            get { return _RefreshInterval; }
            set { _RefreshInterval = value; }
        }

        private int _CleanupInterval = 5;
        /// <summary>
        /// How often we check for inactive peers
        /// </summary>
        public int CleanupInterval
        {
            get { return _CleanupInterval; }
            set { _CleanupInterval = value; }
        }


        public CustomPeerResolver(string ChannelName)
        {
            _ChannelName = ChannelName;

        }

        public CustomPeerResolver(string ChannelName, string ChannelPassword)
        {
            _ChannelName = ChannelName;
            _ChannelPassword = ChannelPassword;
        }

        public CustomPeerResolver(string ChannelName, string ChannelPassword, X509Certificate2 ChannelCert)
        {
            _ChannelName = ChannelName;
            _ChannelPassword = ChannelPassword;
            _ChannelCert = ChannelCert;
        }

        public void Start()
        {
            // Create a new resolver service
            _ResolverService.RefreshInterval = TimeSpan.FromMinutes(_RefreshInterval);
            _ResolverService.CleanupInterval = TimeSpan.FromMinutes(_CleanupInterval);
            _ResolverService.ControlShape = true;

            #region Config File Free Code

            // Create the base addess for the service
            Uri baseAddress = new Uri("net.tcp://localhost/" + _ChannelName);

            // Create a new service host
            _ResolverServiceHost = new ServiceHost(_ResolverService, baseAddress);

            // Secure the channel
            _ResolverServiceHost.Credentials.ClientCertificate.Certificate = _ChannelCert;
            _ResolverServiceHost.Credentials.ServiceCertificate.Certificate = _ChannelCert;
            _ResolverServiceHost.Credentials.Peer.MeshPassword = _ChannelPassword;
            

            // Create a binding
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
            

            // And finally add an endpoint
            _ResolverServiceHost.AddServiceEndpoint(typeof(IPeerResolverContract), binding, baseAddress);

            #endregion Config File Free Code

            // Open the resolver service 
            _ResolverService.Open();
            _ResolverServiceHost.Open();
        }
    }
}
