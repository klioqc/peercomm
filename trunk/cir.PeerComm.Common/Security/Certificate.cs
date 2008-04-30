using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Reflection.Emit;
using System.Resources;
using System.Threading;

using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;


namespace cir.PeerComm.Security
{
    /// <summary>
    /// This class provides methods for loading and generating certificates
    /// If you don't tell the class a cert name it assumes you are dealing
    /// with the peer network's mesh certificate.
    /// </summary>
    public class Certificate
    {
        private const string _MeshCertFileName = ".\\Assembly\\MeshCert.cer";
        private const int _DefaultKeySize = 1024;

        private static X509Certificate2 _Cert;

        /// <summary>
        /// This certificate is used by the default mesh channel.
        /// If you want to make multiple channels you'll just need to 
        /// keep track of the cert yourself.  This is really just for
        /// convenience in a one mesh enviroment.
        /// </summary>
        public static X509Certificate2 MeshCertificate
        {
            get
            {
                // Try to load the cert, if it doesn't exist create it
                if (_Cert == null)
                {
                    _Cert = LoadCert(_MeshCertFileName);
                    if (_Cert == null)
                    { CreateAndSave(_MeshCertFileName, _DefaultKeySize); }
                }

                return _Cert;
            }
        }


        /// <summary>
        /// Create and save a self-signed certificate.
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="KeySize"></param>
        public static void CreateAndSave(string FileName, int KeySize)
        {
            _Cert = CreateCertificate(Guid.NewGuid().ToString(), KeySize);
            SaveCert(_Cert, FileName);
        }

        /// <summary>
        /// Make a new self-signed certificate using the given keysize.
        /// </summary>
        /// <param name="CompanyName"></param>
        /// <param name="KeySize"></param>
        /// <returns></returns>
        public static X509Certificate2 CreateCertificate(string CompanyName, int KeySize)
        {
            DigitalSignature sig = new DigitalSignature(KeySize);

            Org.Mentalis.Security.Tools.CertificateSettings certSettings = new Org.Mentalis.Security.Tools.CertificateSettings();
            certSettings.EndDate = DateTime.Now.AddYears(10);

            // Why they don't wrap the company name for you is beyond me.  Dumb, just plain dumb.
            string companyName = "CN={" + CompanyName.Replace(",", " ") +"}";
            X509Certificate cert = Org.Mentalis.Security.Tools.X509CertificateGenerator.Create(sig.CryptoProvider, companyName , certSettings);
           
            return new X509Certificate2(cert);

        }

        /// <summary>
        /// Save a certificate object to file
        /// </summary>
        /// <param name="Cert"></param>
        /// <param name="FileName"></param>
        public static void SaveCert(X509Certificate2 Cert, string FileName)
        {
            try
            {
                File.WriteAllBytes(FileName, Cert.RawData);
            }
            catch { }

            return;
        }


        /// <summary>
        /// Read in a certificate
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static X509Certificate2 LoadCert(string FileName)
        {
            // Read in the certificate
            X509Certificate2 cert = null;

            if (File.Exists(FileName))
            {
                try
                {
                    byte[] rawCert = File.ReadAllBytes(FileName);
                    cert = new X509Certificate2(rawCert);
                }
                catch { }
            }
            return cert;
        }

        /// <summary>
        /// Creates an empty DLL to store the certificate in
        /// Not used but I wanted to learn how to do it so kept the code.
        /// </summary>
        /// <param name="CertFileName"></param>
        private void createCertSatelliteDLL(string CertFileName, byte[] SecurityCert)
        {
            
            string assemblyFileName = "cir.PeerComm.Cert.dll";
            
            // Find out where we are
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            
            // ... and what domain we're in
            AppDomain appDomain = Thread.GetDomain();
            
            // Create a new assembly to store the cert
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "cir.PeerComm.Cert";
            assemblyName.CodeBase = basePath;
            
            // Create the assembly reference
            AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, basePath);

            // Create a dummy module so we can use that to create a resources file
            ModuleBuilder modualeBuilder = assemblyBuilder.DefineDynamicModule("EmptyModule", "cir.PeerComm.Cert.dll");
            IResourceWriter resourceWriter = modualeBuilder.DefineResource("cir.PeerComm.Cert.resources", "Description", ResourceAttributes.Public);

            // And finally actually embed the cert
            resourceWriter.AddResource("SecurityCert", SecurityCert);

            // Now actually save the assembly
            assemblyBuilder.Save(assemblyFileName);
        }


    }
}
