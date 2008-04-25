using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace cir.PeerComm.Security
{
    /// <summary>
    /// Provides methods 
    /// </summary>
    internal class DigitalSignature
    {
        /// <summary>
        /// Generates a private key
        /// </summary>
        /// <returns></returns>
        public static byte[] MakePrivateKey()
        {
            // 
            byte[] privateKey = new byte[64];

            // Create a cryptographically strong random number generator
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            
            // Save a random number into the key
            rng.GetBytes(privateKey);

            return privateKey;
        }


        public static string MakeSignature(string Text, X509Certificate2 ClientCert)
        {


            byte[] data = Encoding.Unicode.GetBytes(Text);
            //sha1 crypto service, digital signatures are created from the hash
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(data);

            DSACryptoServiceProvider DSA = (DSACryptoServiceProvider)ClientCert.PrivateKey;

            //Create an DSASignatureFormatter object and pass it the 
            //DSACryptoServiceProvider to transfer the key information.
            //DSAFormatter is used to generate the digital signature
            DSASignatureFormatter DSAFormatter = new DSASignatureFormatter(DSA);
            //Set the hash algorithm to SHA1.
            DSAFormatter.SetHashAlgorithm("SHA1");
            //Create a signature for HashValue and return it.
            byte[] signature = DSAFormatter.CreateSignature(hash);

            StringBuilder tempSig = new StringBuilder();

            foreach (byte sigByte in signature)
            { tempSig.AppendFormat("X2", sigByte); }

            return tempSig.ToString();
        }
    }
}
