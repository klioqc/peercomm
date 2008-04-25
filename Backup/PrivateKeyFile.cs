/*
 *   Mentalis.org Security Tools
 * 
 *     Copyright 2002-2007, The Mentalis.org Team
 *     All rights reserved.
 *     http://www.mentalis.org/
 *
 *
 *   Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions
 *   are met:
 *
 *     - Redistributions of source code must retain the above copyright
 *        notice, this list of conditions and the following disclaimer. 
 *
 *     - Neither the name of the Mentalis.org Team, nor the names of its
 *        contributors may be used to endorse or promote products derived
 *        from this software without specific prior written permission. 
 *
 *   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 *   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 *   FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 *   THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 *   INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 *   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 *   SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 *   HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 *   STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 *   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 *   OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;

namespace Org.Mentalis.Security.Tools {
    /// <summary>
    /// The PrivateKeyFile class represents a Private Key file (also known as a PVK
    /// file). It can load PVK files and associate them with their corresponding
    /// X509 certificates.
    /// </summary>
    public class PrivateKeyFile : IDisposable {
        /// <summary>
        /// Initializes a new PrivateKeyFile instance.
        /// </summary>
        /// <param name="pvkFile">The file to load.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pvkFile"/> is a null reference -or- a password is required.</exception>
        /// <exception cref="FileNotFoundException">The specified file could not be found.</exception>
        /// <exception cref="CryptographicException">An error occurs while decrypting the file.</exception>
        /// <exception cref="InvalidDataException">The file is invalid.</exception>
        public PrivateKeyFile(string pvkFile) : this(pvkFile, null) { }
        /// <summary>
        /// Initializes a new PrivateKeyFile instance.
        /// </summary>
        /// <param name="pvkFile">The file to load.</param>
        /// <param name="password">The password, if any.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pvkFile"/> is a null reference -or- a password is required and <paramref name="password"/> is a null reference.</exception>
        /// <exception cref="FileNotFoundException">The specified file could not be found.</exception>
        /// <exception cref="CryptographicException">An error occurs while decrypting the file.</exception>
        /// <exception cref="InvalidDataException">The file is invalid.</exception>
        public PrivateKeyFile(string pvkFile, string password) {
            if (pvkFile == null)
                throw new ArgumentNullException("pvkFile");
            if (!File.Exists(pvkFile))
                throw new FileNotFoundException("The PVK file could not be found.");
            using (BinaryReader reader = new BinaryReader(File.OpenRead(pvkFile))) { 
                if (reader.ReadUInt32() != 0xB0B5F11E)
                    throw new CryptographicException("The specified file is not a valid private key file.");
                reader.ReadInt32(); // skip 4 bytes
                _keyType = reader.ReadInt32();
                int isEncrypted = reader.ReadInt32();
                int saltLength = reader.ReadInt32();
                int keyLen = reader.ReadInt32();
                byte[] salt = reader.ReadBytes(saltLength);
                if (salt.Length != saltLength)
                    throw new InvalidDataException("Unexpected end-of-file found.");
                _blob = reader.ReadBytes(keyLen);
                if (_blob.Length != keyLen)
                    throw new InvalidDataException("Unexpected end-of-file found.");
                if (isEncrypted != 0) { // decrypt private key
                    if (password == null) {
                        throw new ArgumentNullException("password");
                    }
                    byte[] pass = null, key = null, pkb = null;
                    try {
                        pass = Encoding.ASCII.GetBytes(password);
                        key = new byte[salt.Length + password.Length];
                        Array.Copy(salt, 0, key, 0, salt.Length);
                        Array.Copy(pass, 0, key, salt.Length, pass.Length);
                        pkb = TryDecrypt(_blob, 8, _blob.Length - 8, key, 16);
                        if (pkb == null) { // decryption failed, try with an export key
                            pkb = TryDecrypt(_blob, 8, _blob.Length - 8, key, 5);
                            if (pkb == null) {
                                throw new CryptographicException("The PVK file could not be decrypted. [wrong password?]");
                            }
                        }
                        Array.Copy(pkb, 0, _blob, 8, pkb.Length);
                    } finally {
                        if (pkb != null)
                            Array.Clear(pkb, 0, pkb.Length);
                        if (pass != null)
                            Array.Clear(pass, 0, pass.Length);
                        if (key != null)
                            Array.Clear(key, 0, key.Length);
                    }
                }
            }
        }
        // return null of a decryption error occurs
        private byte[] TryDecrypt(byte[] buffer, int offset, int length, byte[] password, int keyLen) {
            byte[] key = new byte[16];
            Array.Copy(SHA1.Create().ComputeHash(password, 0, password.Length), 0, key, 0, keyLen);
            byte[] ret = (new ARCFourManagedTransform(key)).TransformFinalBlock(buffer, offset, length);
            if (ret[0] != 0x52 || ret[1] != 0x53 || ret[2] != 0x41 || ret[3] != 0x32) // first four bytes must be 'RSA2'
                return null;
            return ret;
        }

        /// <summary>
        /// Associates this private key instance with the corresponding X509 certificate.
        /// </summary>
        /// <param name="certificate">An <see cref="X509Certificate"/> instance to associate the key with.</param>
        /// <exception cref="CryptographicException">An error occurs while associating the private key with the certificate.</exception>
        /// <remarks>The key will be imported as an exportable key.</remarks>
        public void AssociateWith(X509Certificate certificate) {
            AssociateWith(certificate, true);
        }
        /// <summary>
        /// Associates this private key instance with the corresponding X509 certificate.
        /// </summary>
        /// <param name="certificate">An <see cref="X509Certificate"/> instance to associate the key with.</param>
        /// <param name="exportable"><b>true</b> if the key should be marked as exportable, <b>false</b> otherwise.</param>
        /// <exception cref="CryptographicException">An error occurs while associating the private key with the certificate.</exception>
        public void AssociateWith(X509Certificate certificate, bool exportable) {
            if (certificate == null)
                throw new ArgumentNullException("certificate");

            IntPtr hKey = IntPtr.Zero;
            int flags = 0;
            if (exportable)
                flags = CRYPT_EXPORTABLE;
            using (CryptoProviderHandle provider = new CryptoProviderHandle()) {
                if (CryptImportKey(provider.Handle, _blob, _blob.Length, IntPtr.Zero, flags, ref hKey) == 0)
                    throw new CryptographicException("Could not import the private key from the PVK file.");
                CRYPT_KEY_PROV_INFO kpi = new CRYPT_KEY_PROV_INFO();
                kpi.pwszContainerName = provider.Container;
                kpi.pwszProvName = null;
                kpi.dwProvType = provider.ProviderType;
                kpi.dwFlags = 0;
                kpi.cProvParam = 0;
                kpi.rgProvParam = IntPtr.Zero;
                kpi.dwKeySpec = _keyType;
                if (CertSetCertificateContextProperty(certificate.Handle, CERT_KEY_PROV_INFO_PROP_ID, 0, ref kpi) == 0)
                    throw new CryptographicException("Could not associate the private key with the certificate.");
                CryptDestroyKey(hKey);
            }
        }

        /// <summary>
        /// Releases all managed and unmanaged resources.
        /// </summary>
        public void Dispose() {
            if (_blob != null) {
                Array.Clear(_blob, 0, _blob.Length);
                _blob = null;
                GC.SuppressFinalize(this);
            }
        }
        /// <summary>
        /// Releases all managed and unmanaged resources.
        /// </summary>
        ~PrivateKeyFile() {
            Dispose();
        }

        private byte[] _blob;
        private int _keyType;

        [DllImport(@"advapi32.dll", SetLastError = true)]
        private static extern int CryptImportKey(IntPtr hProv, byte[] pbData, int dwDataLen, IntPtr hPubKey, int dwFlags, ref IntPtr phKey);

        [DllImport(@"crypt32.dll", EntryPoint = "CertSetCertificateContextProperty")]
        private static extern int CertSetCertificateContextProperty(IntPtr pCertContext, int dwPropId, int dwFlags, ref CRYPT_KEY_PROV_INFO pvData);

        [DllImport(@"advapi32.dll")]
        private static extern int CryptDestroyKey(IntPtr hKey);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CRYPT_KEY_PROV_INFO {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwszContainerName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pwszProvName;
            public int dwProvType;
            public int dwFlags;
            public int cProvParam;
            public IntPtr rgProvParam;
            public int dwKeySpec;
        }

        private const int CRYPT_EXPORTABLE = 0x00000001;
        private const int CERT_KEY_PROV_INFO_PROP_ID = 2;
    }
}
