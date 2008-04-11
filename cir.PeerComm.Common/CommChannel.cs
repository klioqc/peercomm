using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.PeerResolvers;
using System.Security.Cryptography.X509Certificates;

using System.Diagnostics;

namespace cir.PeerComm
{
    /// <summary>
    /// A simple WCF PeerChannel client that allows for custom node and message types
    /// </summary>
    /// <typeparam name="Object"></typeparam>
    /// <typeparam name="Object"></typeparam>
    public class CommNode : IComm
    {

        public delegate void PeerNodeConnectedDelegate(Object Client);
        public delegate void PeerNodeDisconnectedDelegate(Object Client); 


        /// <summary>
        /// Stores information about the client that created this communications node
        /// </summary>
        private Object _Client;

        /// <summary>
        /// 
        /// </summary>
        private NetPeerTcpBinding _CommBinding = new NetPeerTcpBinding();

        /// <summary>
        /// If the client is online or not.
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
        /// <param name="Client"></param>
        public CommNode(Object Client)
        {
            initialize(Client, null);
        }

        /// <summary>
        /// Create a new instance of the CommNode class with no security
        /// </summary>
        /// <param name="Client"></param>
        public CommNode(Object Client, string ChannelName, string ChannelPassword, X509Certificate2 ChannelCert)
        {
            //initialize(Client, null);
        }


        /// <summary>
        /// Create a new instance of the CommNode class 
        /// </summary>
        /// <param name="Client"></param>
        public CommNode(Object Client, bool UseCustomePeerResolver)
        {
            initialize(Client, null);
        }


        private void initialize(Object Client, CustomPeerResolver Resolver)
        {
            _Client = Client;

            //buildCommBinding

            // Handles messages on the callback interface
            InstanceContext instanceContext = new InstanceContext(this);

            // Create a channel factory to do all the work of creating the proxy for this class
            DuplexChannelFactory<ICommDuplexChannel> factory = new DuplexChannelFactory<ICommDuplexChannel>(instanceContext, _CommBinding, "net.p2p://cirCommMesh/PeerComm");

            // Retrieve the PeerNode associated with the participant and register for online/offline events
            ICommDuplexChannel participant = factory.CreateChannel();

            // Register this client node with the online and offline events
            IOnlineStatus ostat = participant.GetProperty<IOnlineStatus>();
            ostat.Online += new EventHandler(OnOnline);
            ostat.Offline += new EventHandler(OnOffline);
            

            try
            {
                participant.Open();
            }
            catch (CommunicationException ce)
            {
                Debug.WriteLine(ce.ToString());
                Console.WriteLine(ce.ToString());
                Console.WriteLine("Could not find resolver.  If you are using a custom resolver, please ensure");
                Console.WriteLine("that the service is running before executing this sample.  Refer to the readme");
                Console.WriteLine("for more details.");
                Console.ReadLine();
                return;
            }


            Console.WriteLine("{0} is ready", Client);
            Console.WriteLine("Type chat messages after going Online");
            Console.WriteLine("Press q<ENTER> to terminate this instance.");

            // Announce self to other participants
            participant.Connect(Client);

            // loop until the user quits
            while (true)
            {
                string line = Console.ReadLine();
                if (line == "q") break;
                participant.SendMessage(Client, line);
            }
            // Leave the mesh
            participant.Disconnect(Client);
            participant.Close();
            factory.Close();

        }

       
        public void SendMessage(Object Client, Object Message)
        {

        }

        public void Connect(Object Client)
        {
        }

        public void Disconnect(Object Client)
        {

        }

        
        static void OnOnline(object sender, EventArgs e)
        {
            Console.WriteLine("**  Online");
        }

        static void OnOffline(object sender, EventArgs e)
        {
            Console.WriteLine("**  Offline");
        }

        #region IComm<Object,Object> Members


        public void SendMessage(Object ClientFrom, Object ToNode, Object Message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
