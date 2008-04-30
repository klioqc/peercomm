using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cir.PeerComm
{
    /// <summary>
    /// The minimum requirements for a message object, do not alter.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// A unique ID for a message.
        /// Must be included
        /// </summary>
        Guid MessageID
        { get; set; }

        /// <summary>
        /// Who the message is intended for. 
        /// If this is a broadcast message then the MessageTo GUID is all zeros.
        /// </summary>
        Guid MessageTo
        { get; set; }

        /// <summary>
        /// Who the message is from.
        /// </summary>
        Guid MessageFrom
        { get; set; }

    }
}