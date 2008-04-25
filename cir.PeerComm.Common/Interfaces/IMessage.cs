using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cir.PeerComm
{
    /// <summary>
    /// Modify this interface to suit your message design, this is just an example
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
        /// A checksum to verify a message is from who it says it's from.
        /// Must be included
        /// </summary>
        string MessageChecksum
        { get; set; }

        Guid MessageTo
        { get; set; }

        Guid MessageFrom
        { get; set; }

        string MessageText
        { get; set; }

    }
}
