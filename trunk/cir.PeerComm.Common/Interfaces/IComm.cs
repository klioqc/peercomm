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
        /// Broadcasts a message to all peers on the network
        /// 
        /// You can specify a message is intended for only one recipient
        /// but all members will still receive the message so be aware
        /// this is NOT a secure method of sending a message.
        /// </summary>
        /// <param name="PeerFrom"></param>
        /// <param name="ToNode"></param>
        /// <param name="Message"></param>
        [OperationContract(IsOneWay = true)]
        void SendMessage(IMessage Message, byte[] MessageSignature);
    }

}
