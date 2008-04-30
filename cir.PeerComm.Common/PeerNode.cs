using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using cir.PeerComm.Security;

namespace cir.PeerComm
{
    /// <summary>
    /// Represents the basic information about a node on the peer mesh
    /// </summary>
    [Serializable]
    public struct PeerNode : IPeerNode
    {
        /// <summary>
        /// Represents the basic information about a node on the peer mesh
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        public PeerNode(string Name, string Description, RSAParameters PublicKey)
        {
            _Name = Name;
            _Description = Description;
            _ID = Guid.NewGuid();
            _PublicKey = PublicKey;
        }

        private string _Name;

        /// <summary>
        /// A friendly name for the peer
        /// e.g. LittleTimmy
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Description;

        /// <summary>
        /// Some text telling us a little bit about the peer
        /// e.g. Guild Master
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }


        #region IPeer Members

        private Guid _ID;

        /// <summary>
        /// The unique ID for a client
        /// </summary>
        public Guid ID
        {
            get
            { return _ID; }
            set
            { _ID = value; }
        }

        private RSAParameters _PublicKey;

        /// <summary>
        /// The peer's public key used for message signing
        /// </summary>
        public RSAParameters PublicKey
        {
            get
            { return _PublicKey; }
            set
            { _PublicKey = value; }
        }

        #endregion
    }
}
