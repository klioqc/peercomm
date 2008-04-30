using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace cir.PeerComm
{
    /// <summary>
    /// Describes the minimum information needed by the PeerComm system to identify a Peer, do not alter.
    /// </summary>
    public interface IPeerNode
    {
        /// <summary>
        /// The unique ID for a client
        /// </summary>
        Guid ID
        { get; set; }

        /// <summary>
        /// The peer's public key used for message signing
        /// </summary>
        RSAParameters PublicKey
        { get; set; }

    }
}