using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cir.PeerComm
{
    /// <summary>
    /// What a message should look like.
    /// Feel free to add to this interface to suit your needs but 
    /// the only field that isn't required is the MessageText field.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// A unique ID for a message. (Require)
        /// Must be included
        /// </summary>
        Guid MessageID
        { get; set; }

        /// <summary>
        /// Who the message is intended for. (Required)
        /// If this is a broadcast message then the MessageTo GUID is all zeros.
        /// </summary>
        Guid MessageTo
        { get; set; }

        /// <summary>
        /// Who the message is from. (Required)
        /// </summary>
        Guid MessageFrom
        { get; set; }

        /// <summary>
        /// Text of the message. (Optional)
        /// This is "optional" but you'll want to
        /// have some kind of message content.
        /// </summary>
        string MessageText
        { get; set; }

    }
}
