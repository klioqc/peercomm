using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace cir.PeerComm
{
    /// <summary>
    /// Describes what a Peer object will look like, modify this to your needs as this is only an example
    /// The only fields required for proper operation of the PeerComm system are ID and PublicKey.
    /// </summary>
    public interface IPeer
    {
        /// <summary>
        /// The unique ID for a client (required)
        /// </summary>
        Guid ID
        { get; set; }

        /// <summary>
        /// The peer's public key used for message signing (required)
        /// </summary>
        RSAParameters PublicKey
        { get; set; }

        /// <summary>
        /// A friendly name for the peer (optional)
        /// e.g. LittleTimmy
        /// </summary>
        string Name
        { get; set; }

        /// <summary>
        /// Some text telling us a little bit about the peer (optional)
        /// e.g. Guild Master
        /// </summary>
        string Description
        { get; set; }

    }
}
