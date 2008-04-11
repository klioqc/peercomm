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

namespace cir.PeerComm
{
    /// <summary>
    /// This class provides methods for loading and generating certificates
    /// </summary>
    public class Certificate
    {

        /// <summary>
        /// Creates an X509 Certificate that is self-signed.
        /// </summary>
        /// <param name="CompanyName"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool CreateCertificate(string CompanyName, string FileName)
        {
            // Build the file name and command line parameters
            string makeCertExe = Directory.GetCurrentDirectory() + "\\assembly\\makecert.exe";
            string commandLine = "-r -pe -n \"CN=" + CompanyName + "\" -sky exchange \"" + FileName + "\"";

            // Generate the certificate
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = commandLine;
            startInfo.FileName = makeCertExe;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            Process shell = Process.Start(startInfo);
            shell.WaitForExit();

            // Check for an error
            if (shell.ExitCode == -1)
            { return false; }

            return true;
        }

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
        
        }

        public static void createCertSatelliteDLL(string CertFileName)
        {
            
            string assemblyFileName = "cir.PeerComm.Cert.dll";
            
            // Find out where we are
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            
            // 
            AppDomain appDomain = Thread.GetDomain();
            
            // 
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "cir.PeerComm.Cert";
            assemblyName.CodeBase = basePath;
            
            // Create the assembly reference
            AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, basePath);
            // Embed the cert
            //assemblyBuilder.DefineResource(

            //embed the resources
            ModuleBuilder modualeBuilder = assemblyBuilder.DefineDynamicModule("EmptyModule", "cir.PeerComm.Cert.dll");
            IResourceWriter resourceWriter = modualeBuilder.DefineResource("cir.PeerComm.Cert.resources", "Description", ResourceAttributes.Public);
            
            resourceWriter.AddResource("resName","My (dynamic) resource value.");
            resourceWriter.AddResource("resName2","My (dynamic) second resource value.");

            // Now actually save the assembly
            assemblyBuilder.Save(assemblyFileName);
        }

    }
}
