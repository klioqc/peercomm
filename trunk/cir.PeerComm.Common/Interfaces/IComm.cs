using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.PeerResolvers;

using System.Diagnostics;

namespace cir.PeerComm
{
    /// <summary>
    /// Describes what can be done by the peer nodes that connect to the service.
    /// 
    /// Note: I really wanted to use generics with this for the Peer and Message types
    ///       but you can't use generics in a service contract declaration. Because of this
    ///       you have to declare the types somewhere and that negates the purpose of generics.
    /// </summary>
    [ServiceContract(Namespace = "http://www.fiveinchfish.com", CallbackContract = typeof(IComm))] 
    public interface IComm
    {
        /// <summary>
        /// Called when a node connects to the peer mesh
        /// This is used for signaling other nodes that
        /// a node has joined the network.
        /// </summary>
        /// <param name="Peer"></param>
        [OperationContract(IsOneWay = true)]
        void Connect(IPeer Peer);

         /// <summary>
        /// Peer disconnects from the peer mesh.
        /// This is used for signalling other peers that
        /// a node has left the network.
        /// </summary>
        /// <param name="Peer"></param>
        [OperationContract(IsOneWay = true)]
        void Disconnect(IPeer Peer);

        /// <summary>
        /// Peer sends a message to all of the other peers on the network.
        /// </summary>
        /// <param name="PeerFrom"></param>
        /// <param name="Message"></param>
        [OperationContract(IsOneWay = true)]
        void SendMessage(IPeer PeerFrom, IMessage Message);

        /// <summary>
        /// Sends a message to a specific peer on the network.
        /// In reality is still sends the message to all the nodes 
        /// but only the appropriate node will receive a message event.
        /// </summary>
        /// <param name="PeerFrom"></param>
        /// <param name="ToNode"></param>
        /// <param name="Message"></param>
        [OperationContract(IsOneWay = true)]
        void SendMessage(IPeer PeerFrom, IPeer PeerTo, IMessage Message);
    }

}
