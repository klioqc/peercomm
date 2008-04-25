using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.PeerResolvers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;



using System.Diagnostics;

namespace cir.PeerComm
{
    /// <summary>
    /// A simple WCF PeerChannel Peer that allows for custom node and message types
    /// </summary>
    /// <typeparam name="Object"></typeparam>
    /// <typeparam name="Object"></typeparam>
    internal class CommChannel : IComm
    {

        public delegate void PeerNodeConnectedDelegate(IPeer Peer);
        public delegate void PeerNodeDisconnectedDelegate(IPeer Peer);

        /// <summary>
        /// Used to verify a message isn't spoofed by a rogue client.
        /// 
        /// The certs already in use make sure the communication is from an authorized client
        /// but this makes sure a client doesn't lie about who it is.  We could use certs for this
        /// too but it's not that complicated so this should work fine.
        /// </summary>
        private Guid _PrivateGuid = Guid.NewGuid();
        
        /// <summary>
        /// The Public GUID (Peer.ID) is concatinated to the _PrivateGuid then MD5'd to give this value
        /// If 
        /// </summary>
        private string MD5Hash = "";

        /// <summary>
        /// Stores information about the Peer that created this communications node
        /// </summary>
        private IPeer _Peer;


        /// <summary>
        /// This is the communication channel used to send messages from the client
        /// </summary>
        ICommDuplexChannel _ClientChannel = null;

        /// <summary>
        /// This does all the work of generating the proxies for communication
        /// </summary>
        DuplexChannelFactory<ICommDuplexChannel> _ChannelFactory;

        /// <summary>
        /// 
        /// </summary>
        private NetPeerTcpBinding _CommBinding = new NetPeerTcpBinding();

        /// <summary>
        /// If the Peer is online or not.
        /// </summary>
        private bool _Online;

        /// <summary>
        /// Builds a NetPeerTcpBinding for the peer comm channel
        /// If a custom resolver is specified and enabled in the config file 
        /// then that is used instead of the default (and easier) PNRP.
        /// </summary>
        private NetPeerTcpBinding buildCommBinding()
        {
            NetPeerTcpBinding binding = new NetPeerTcpBinding();
            _CommBinding.Security.Mode = SecurityMode.TransportWithMessageCredential;
            _CommBinding.Security.Transport.CredentialType = PeerTransportCredentialType.Password;

            if (Properties.Settings.Default.UseCustomResolver)
            {
                //Uri baseAddress = new Uri("net.tcp://" + Properties.Settings.Default.CustomResolverURI + "/" + Properties.Settings.Default.CustomerResolverName);
                //CustomResolver resolver = new CustomResolver();
                _CommBinding.Resolver.Mode = PeerResolverMode.Custom;
                //_CommBinding.Resolver.Custom.Address = new EndpointAddress(baseAddress);
                _CommBinding.Resolver.Custom.Binding = new NetTcpBinding(SecurityMode.Message);
            }

            return binding;
        }

        /// <summary>
        /// Create a new instance of the CommNode class with no security
        /// </summary>
        /// <param name="Peer"></param>
        public CommChannel(IPeer Peer)
        {
            initialize(Peer, null);
        }

        /// <summary>
        /// Create a new instance of the CommNode class with security
        /// </summary>
        /// <param name="Peer"></param>
        public CommChannel(IPeer Peer, string ChannelName, string ChannelPassword, X509Certificate2 ChannelCert)
        {
            //initialize(Peer, null);
        }


        /// <summary>
        /// Create a new instance of the CommNode class using a custom peer resolver instead of PNRP
        /// </summary>
        /// <param name="Peer"></param>
        public CommChannel(IPeer Peer, bool UseCustomePeerResolver)
        {
            //initialize(Peer, null);
        }


        private void initialize(IPeer Peer, CustomPeerResolver Resolver)
        {
            _Peer = Peer;

            //buildCommBinding

            // This creates the references for other clients to call this client's IComm methods
            InstanceContext instanceContext = new InstanceContext(this);

            // The channel factory will do all the work of creating the proxy for this class so we don't have to
            _ChannelFactory = new DuplexChannelFactory<ICommDuplexChannel>(instanceContext, _CommBinding, "net.p2p://cirCommMesh/PeerComm");

            // Retrieve the peer node associated with the participant and register for online/offline events
            _ClientChannel = _ChannelFactory.CreateChannel();

            // Register this peer node with the online and offline events
            IOnlineStatus ostat = _ClientChannel.GetProperty<IOnlineStatus>();
            ostat.Online += new EventHandler(OnOnline);
            ostat.Offline += new EventHandler(OnOffline);

            try
            {
                _ClientChannel.Open();
            }
            catch (CommunicationException ce)
            {
                throw new Exception("Could not connect to resolver service.", ce);
            }

            // Announce self to other participants
            _ClientChannel.Connect(Peer);

        }

       
        /// <summary>
        /// Send a message to 
        /// </summary>
        /// <param name="Peer"></param>
        /// <param name="Message"></param>
        public void SendMessage(IMessage Message, byte[] MessageSignature)
        {
        }
 

        public void Connect(IPeer Peer)
        {

        }

        public void Disconnect(IPeer Peer)
        {
            // Leave the mesh
            _ClientChannel.Disconnect(Peer);
            _ClientChannel.Close();
            _ChannelFactory.Close();

        }

        
        static void OnOnline(object sender, EventArgs e)
        {
            Console.WriteLine("**  Online");
        }

        static void OnOffline(object sender, EventArgs e)
        {
            Console.WriteLine("**  Offline");
        }


    }
}
