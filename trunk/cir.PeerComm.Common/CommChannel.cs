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
    /// A simple WCF PeerChannel PeerNode that allows for custom node and message types.
    /// It can be confusing at first but the methods of this class are not called directly.
    /// Instead they are called through a channel object and then the methods inside this class
    /// are fired on every client connected to the mesh, including the client that made the call.
    /// </summary>
    /// <typeparam name="Object"></typeparam>
    /// <typeparam name="Object"></typeparam>
    internal class CommChannel : IComm , IDisposable
    {

        // Handlers for message events
        public delegate void PeerNodeConnectedHandler(PeerNode Peer);
        public delegate void PeerNodeDisconnectedHandler(PeerNode Peer);
        public delegate void MessageReceivedHandler(Message Msg, byte[] Signature);

        /// <summary>
        /// Fires when a peer on the mesh sends a message
        /// </summary>
        public event MessageReceivedHandler MessageReceived;

        /// <summary>
        /// Fires when a peer connects to the mesh
        /// </summary>
        public event PeerNodeConnectedHandler PeerNodeConnected;

        /// <summary>
        /// Fires when a peer disconnects from the mesh
        /// </summary>
        public event PeerNodeDisconnectedHandler PeerNodeDisconnected;

        /// <summary>
        /// Stores information about the PeerNode that created this communications node
        /// </summary>
        private PeerNode _ThisPeer;


        /// <summary>
        /// Create a new instance of the CommNode class with no security
        /// </summary>
        /// <param name="PeerNode"></param>
        public CommChannel(PeerNode Peer)
        {
            //initialize(PeerNode, null);
        }

        /// <summary>
        /// Create a new instance of the CommNode class with security
        /// </summary>
        /// <param name="PeerNode"></param>
        public CommChannel(PeerNode Peer, string ChannelName, string ChannelPassword, X509Certificate2 ChannelCert)
        {
            //initialize(PeerNode, null);
        }


        /// <summary>
        /// Create a new instance of the CommNode class using a custom peer resolver instead of PNRP
        /// </summary>
        /// <param name="PeerNode"></param>
        public CommChannel(PeerNode Peer, bool UseCustomePeerResolver)
        {
            //initialize(PeerNode, null);
        }
        
        /// <summary>
        /// Send a message to connected clients
        /// </summary>
        /// <param name="PeerNode"></param>
        /// <param name="Msg"></param>
        public void SendMessage(Message Msg, byte[] MessageSignature)
        {
            // Check to make sure the message isn't from me
            // And it's either to everyone or me specifically
            if (Msg.MessageFrom != _ThisPeer.ID && (Msg.MessageTo == _BroadcastGuid || Msg.MessageTo == _ThisPeer.ID))
            { 
                
            }
        }
 

        public void Connect(PeerNode PeerNode)
        {
            // Check to see if there is already a peer with that GUID
                // Reject

            // See if the GUID matches our GUID
                // Add to list

        }

        public void Disconnect(PeerNode PeerNode)
        {
            // Leave the mesh
            //_ClientChannel.Disconnect(PeerNode);
            //_ClientChannel.Close();
            //_ChannelFactory.Close();

        }




        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
