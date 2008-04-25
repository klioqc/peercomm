using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;

namespace cir.PeerComm.Security
{
    /// <summary>
    /// Provides methods for signing and verifying signed signatures.
    /// </summary>
    public class DigitalSignature
    {
        #region Properties

        /// <summary>
        /// The crypto provider used by this class
        /// </summary>
        private RSACryptoServiceProvider _CryptoProvider;

        /// <summary>
        /// Gets the crypto provider used by this class
        /// </summary>
        public RSACryptoServiceProvider CryptoProvider
        {
            get 
            {
                return _CryptoProvider; 
            }
        }

        public RSAParameters _PublicKey;

        /// <summary>
        /// Gets your public key, used verify message signatures.
        /// </summary>
        public RSAParameters PublicKey
        {
            get { return _PublicKey; }
        }
        
        public RSAParameters _PrivateKey;

        /// <summary>
        /// Gets the private key for the Crypto Provider used by this class.
        /// Remember that you don't EVER want an external program to have access to this.
        /// </summary>
        public RSAParameters PrivateKey
        {
            get { return _PrivateKey; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Create a new digital signator using the given key size.
        /// </summary>
        /// <param name="KeySize"></param>
        public DigitalSignature(int KeySize)
        {
            InitializeCrytoServiceProvider(KeySize);
        }

        /// <summary>
        /// Create a new digital signator using the smallest key size (384).
        /// </summary>
        public DigitalSignature()
        {
            InitializeCrytoServiceProvider();
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Creates a new RSA based Crypto Service Provider using the smallest key size (384).
        /// </summary>
        /// <param name="KeySize"></param>
        /// <returns></returns>
        public void InitializeCrytoServiceProvider()
        { InitializeCrytoServiceProvider(384); }

        /// <summary>
        /// Creates a new RSA based Crypto Service Provider using the given key size.
        /// </summary>
        /// <param name="KeySize">Valid sizes are 384 to 16384</param>
        public void InitializeCrytoServiceProvider(int KeySize)
        {
            // Bound checks
            if (KeySize < 384) KeySize = 384;
            if (KeySize > 16384) KeySize = 16384;

            _CryptoProvider = new RSACryptoServiceProvider(KeySize);

            // Export WITHOUT the private key data
            _PublicKey = _CryptoProvider.ExportParameters(false);

            // Export WITH the private key data
            _PrivateKey = _CryptoProvider.ExportParameters(true);
        }

        /// <summary>
        /// Creates a fixed size message signature using a hash of your message and your private key.
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="PrivateKey"></param>
        /// <returns></returns>
        public byte[] SignMessage(string Message)
        {
            byte[] messageBytes = ASCIIEncoding.ASCII.GetBytes(Message);
            // Creates a signature using a hash of your message
            return CryptoProvider.SignData(messageBytes, new SHA1CryptoServiceProvider());
        }

        
        /// <summary>
        /// Verify a message is valid and hasn't been tampered with.
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Signature"></param>
        /// <param name="PublicKey"></param>
        /// <returns></returns>
        public bool VerifySignature(string Message, byte[] Signature, RSAParameters PublicKey)
        {

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.ImportParameters(PublicKey);
            byte[] messageBytes = ASCIIEncoding.ASCII.GetBytes(Message);

            // Verifies the signature based on a hash of your original message
            return rsaProvider.VerifyData(messageBytes, new SHA1CryptoServiceProvider(), Signature);
        }

        #endregion Public Methods

    }
}
