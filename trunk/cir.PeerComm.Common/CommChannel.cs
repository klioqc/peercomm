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
        /// The ID of the Peer that created this object
        /// </summary>
        private Guid _CreatorID;

        /// <summary>
        /// Create a new instance of the CommNode class with security
        /// </summary>
        /// <param name="PeerNode"></param>
        public CommChannel(Guid ID)
        {
            _CreatorID = ID;
        }
        
        /// <summary>
        /// Send a message to connected clients
        /// </summary>
        /// <param name="PeerNode"></param>
        /// <param name="Msg"></param>
        public void SendMessage(Message Msg, byte[] Signature)
        {
            // Check to make sure the message isn't from this client
            if (Msg.MessageFrom != _CreatorID)
            {
                if (MessageReceived != null)
                {
                    foreach (MessageReceivedHandler callback in MessageReceived.GetInvocationList())
                    {
                        callback.BeginInvoke(Msg, Signature, null, null);
                    }
                }
            }
        }
 

        public void Connect(PeerNode PeerNode)
        {
            // Check to make sure the message isn't from this client
            if (Msg.MessageFrom != _CreatorID)
            {
                if (MessageReceived != null)
                {
                    foreach (MessageReceivedHandler callback in MessageReceived.GetInvocationList())
                    {
                        callback.BeginInvoke(Msg, Signature, null, null);
                    }
                }
            }
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
