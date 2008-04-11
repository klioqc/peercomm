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
    /// Note: I really wanted to use generics with this for the Client and Message types
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
        /// <param name="NodeName">A friendly name for the node</param>
        /// /// <param name="NodeID">A globally unique ID number for the node</param>
        [OperationContract(IsOneWay = true)]
        void Connect(Object Client);

        /// <summary>
        /// Node disconnects from the peer mesh.
        /// This is used for signalling other nodes that
        /// a node has left the network.
        /// </summary>
        /// <param name="NodeID"></param>
        [OperationContract(IsOneWay = true)]
        void Disconnect(Object Client);

        /// <summary>
        /// Node sends a message to all of the other peers on the network.
        /// </summary>
        /// <param name="NodeID"></param>
        /// <param name="Message"></param>
        [OperationContract(IsOneWay = true)]
        void SendMessage(Object ClientFrom, Object Message);
        
        /// <summary>
        /// Node sends a message to a specific peer on the network.
        /// In reality is still sends the message to all the nodes 
        /// but only the appropriate node will receive a message event.
        /// </summary>
        /// <param name="NodeID"></param>
        /// <param name="Message"></param>
        [OperationContract(IsOneWay = true)]
        void SendMessage(Object ClientFrom, Object ToNode, Object Message);
    }

}
