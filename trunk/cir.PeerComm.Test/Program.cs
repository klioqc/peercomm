using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


using System.Diagnostics;

// http://channel9.msdn.com/wiki/default.aspx/SecurityWiki.ValidateRSASigCode

namespace cir.PeerComm.Test
{
    class Program
    {
        // In RSA signatures a message is hashed with a predefined cryptographic hashing algorithm
        // and the digest or hash is signed with the private keypair
        public static byte[] SignMessage(byte[] message, RSAParameters privkey)
        {
            byte[] signature;
            // Create an RSA Crypto provider, by default creating a new RSACryptoServiceProvider
            // causes a public / private key pair to be generated.
            RSACryptoServiceProvider myRsaProvider = new RSACryptoServiceProvider();
            // Import our private key so we can perform the signing operation
            myRsaProvider.ImportParameters(privkey);
            signature = myRsaProvider.SignData(message, new SHA1CryptoServiceProvider());
            return signature;
        }


        // In RSA signatures a message is verified by performing a hash of the original message
        // and performing a public key operation (exponentiation)
        public static bool VerifyMessageSignature(byte[] message, byte[] sig, RSAParameters pubkey)
        {
            bool isValidSig = false;
            RSACryptoServiceProvider myRsaProvider = new RSACryptoServiceProvider();
            // Import the sender's public key
            myRsaProvider.ImportParameters(pubkey);


            isValidSig = myRsaProvider.VerifyData(message, new SHA1CryptoServiceProvider(), sig);
            return isValidSig;
        }

        static void Main(string[] args)
        {
            // Create a test keypair with a strong keysize of 2048 bits. In a production application 
            // we would load this value from a DPAPI protected registry value
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(384);
            
            // The ExportParameters method below takes a boolean, which determines whether
            // to export the private key data within the parameters. When exporting parameters
            // ensure that only the public key parameter is shared with the remote party:
            RSAParameters rsaPubKeyParams = rsaProvider.ExportParameters(false);

            // We'll also export the private key for our signing operations
            RSAParameters rsaPrivKeyParams = rsaProvider.ExportParameters(true);

			//Create a UnicodeEncoder to convert between byte array and string.
			//UnicodeEncoding byteConverter = new UnicodeEncoding();

            for (int i = 0; i < 10000; i++)
            { 
                
            }

            
            string message1 = "the quick brown fox jumped over the lazy dog";
            string message2 = "the quick brown dog jumped over the lazy fox";


            byte[] sig = SignMessage(ASCIIEncoding.ASCII.GetBytes(message1), rsaPrivKeyParams);
            Console.Out.WriteLine("MSG: " + message1);
            Console.Out.WriteLine("\nRSA Sig: " + Convert.ToBase64String(sig));


            Console.Out.WriteLine("\nTest Case #1:");
            Console.Out.WriteLine("Validating signature for MSG: " + message1);
            if (VerifyMessageSignature(ASCIIEncoding.ASCII.GetBytes(message1), sig, rsaPubKeyParams)) {
                Console.Out.WriteLine("Valid signature.");
            } else {
                Console.Out.WriteLine("Invalid signature.");
            }


            Console.Out.WriteLine("\nTest Case #2:");
            Console.Out.WriteLine("Validating signature for MSG: " + message2);
            if (VerifyMessageSignature(ASCIIEncoding.ASCII.GetBytes(message2), sig, rsaPubKeyParams))
            {
                Console.Out.WriteLine("Valid signature.");
            }
            else
            {
                Console.Out.WriteLine("Invalid signature.");
            }

            Console.ReadLine();
        }

    }
}
