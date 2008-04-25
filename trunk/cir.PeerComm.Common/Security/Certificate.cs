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
using System.Security.Cryptography.X509Certificates;

namespace cir.PeerComm.Security
{
    /// <summary>
    /// This class provides methods for loading and generating certificates
    /// </summary>
    public class Certificate
    {

        public enum KeyTypes
        { 
            exchange = 1,
            signature = 2
        }

        /// <summary>
        /// Creates an X509 Certificate that is self-signed.
        /// </summary>
        /// <param name="CompanyName"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool CreateCertificate(string CompanyName, string FileName, KeyTypes KeyType, string Password)
        {

            
            // Build the file name and command line parameters
            string makeCertExe = Directory.GetCurrentDirectory() + "\\assembly\\makecert.exe";
            string pvk2pfxExe = Directory.GetCurrentDirectory() + "\\assembly\\pvk2pfx.exe";

            string commandLine = "-r -n \"CN=" + CompanyName + "\" -sv " + FileName + ".pvk -sky " + KeyType.ToString() + " " + FileName + ".cer";

            // Generate the certificate and private key files
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = commandLine;
            startInfo.FileName = makeCertExe;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            Process shell = Process.Start(startInfo);
            shell.WaitForExit();

            // Convert the cert and private key files to an all in one pfx file
            commandLine = "-pvk " + FileName + ".pvk -scp " + FileName + ".cer -po " + Password + " -pfx " + FileName + ".pfx";

            startInfo = new ProcessStartInfo();
            startInfo.Arguments = commandLine;
            startInfo.FileName = pvk2pfxExe;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            shell = Process.Start(startInfo);
            shell.WaitForExit();

            File.Delete(FileName + ".cer");

            File.Delete(FileName + ".pvk");

            // Check for an error
            if (shell.ExitCode == -1)
            { return false; }

            return true;
        }

        /// <summary>
        /// Read in a certificate
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static X509Certificate2 ReadCert(string FileName)
        {
            // Read in the certificate
            X509Certificate2 cert = null;
            try
            {
                byte[] rawCert = File.ReadAllBytes(FileName);
                cert = new X509Certificate2(rawCert);
            }
            catch { }

            return cert;
        }

        /// <summary>
        /// Creates an empty DLL to store the certificate in
        /// Not used but I wanted to learn how to do it so kept the code.
        /// </summary>
        /// <param name="CertFileName"></param>
        public static void createCertSatelliteDLL(string CertFileName, byte[] SecurityCert)
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
