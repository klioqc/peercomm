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

using System.Collections;

using System.Diagnostics;

namespace cir.PeerComm
{
    /// <summary>
    /// Provides all the methods and events for sending and receiving messages
    /// Acts as a wrapper for the CommChannel class so it's not as confusing.
    /// </summary>
    public class Client
    {



        #region Properties

        /// <summary>
        /// This is the peer channel the client talks on
        /// </summary>
        private CommChannel _CommChannel;

        /// <summary>
        /// Remember who we are
        /// </summary>
        private PeerNode _ThisPeer = new PeerNode();

        /// <summary>
        /// Maintain a list of peers we're connected to.
        /// </summary>
        private ArrayList _Peers = new ArrayList();

        /// <summary>
        /// This is the communication channel used to send messages from the client
        /// </summary>
        private ICommDuplexChannel _ClientChannel = null;

        /// <summary>
        /// This does all the work of generating the proxies for communication
        /// </summary>
        private DuplexChannelFactory<ICommDuplexChannel> _ChannelFactory;

        /// <summary>
        /// 
        /// </summary>
        private NetPeerTcpBinding _CommBinding = new NetPeerTcpBinding();

        /// <summary>
        /// If the PeerNode is online or not.
        /// </summary>
        private bool _Online;

        #endregion Properties


        #region Handlers and Events


        // We need to keep track of the event handlers so we can remove them when we are done
        private EventHandler _OnOnlineHandler = new EventHandler(OnOnline);
        private EventHandler _OnOfflineHandler = new EventHandler(OnOffline);

        public delegate void OnlineHandler(bool IsOnline);

        public delegate void MessageReceivedHandler(Message Msg);

        public delegate void BogusMessageReceivedHandler(Message Msg);

        public delegate void PeerCameOnlineHandler(PeerNode Peer);


        public event OnlineHandler Online;

        #endregion Handlers and Events

        private void initialize(PeerNode PeerNode, CustomPeerResolver Resolver)
        {
            _ThisPeer = PeerNode;

            //buildCommBinding

            // This creates the references for other clients to call this client's IComm methods
            InstanceContext instanceContext = new InstanceContext(this);

            // The channel factory will do all the work of creating the proxy for this class so we don't have to
            _ChannelFactory = new DuplexChannelFactory<ICommDuplexChannel>(instanceContext, _CommBinding, "net.p2p://cirCommMesh/PeerComm");

            // Retrieve the peer node associated with the participant and register for online/offline events
            _ClientChannel = _ChannelFactory.CreateChannel();

            // Register this peer node with the online and offline events
            IOnlineStatus ostat = _ClientChannel.GetProperty<IOnlineStatus>();
            EventHandler _OnOnlineHandler = new EventHandler(OnOnline);
            EventHandler _OnOfflineHandler = new EventHandler(OnOffline);

            ostat.Online += _OnOnlineHandler;
            ostat.Offline += _OnOfflineHandler;

            try
            {
                _ClientChannel.Open();
            }
            catch (CommunicationException ce)
            {
                throw new Exception("Could not connect to resolver service.", ce);
            }

            // Announce self to other participants
            _ClientChannel.Connect(PeerNode);

        }

        static void OnOnline(object sender, EventArgs e)
        {
            Console.WriteLine("**  Online");
        }

        static void OnOffline(object sender, EventArgs e)
        {
            Console.WriteLine("**  Offline");
        }

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

    }
}
