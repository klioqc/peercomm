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
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Org.Mentalis.Security.Tools {
    /// <summary>
    /// Encapsulates a CSP handle.
    /// </summary>
    internal class CryptoProviderHandle : IDisposable {
        public CryptoProviderHandle() : this(Guid.NewGuid().ToString()) { }
        public CryptoProviderHandle(string container) {
            int flags, fs = 0, fmk = 0;
            _container = container;

            if (!Environment.UserInteractive && Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 5) {
                fs = CRYPT_SILENT;
                fmk = CRYPT_MACHINE_KEYSET;
            }

            flags = fs | fmk;
            if (CryptAcquireContext(ref _handle, _container, null, this.ProviderType, flags) == 0) {
                if (Marshal.GetLastWin32Error() == NTE_BAD_KEYSET) {
                    CryptAcquireContext(ref _handle, _container, null, this.ProviderType, flags | CRYPT_NEWKEYSET);
                } else if (fmk != 0) {
                    flags = fs;
                    if (CryptAcquireContext(ref _handle, _container, null, this.ProviderType, flags) == 0) {
                        if (Marshal.GetLastWin32Error() == NTE_BAD_KEYSET) {
                            CryptAcquireContext(ref _handle, _container, null, this.ProviderType, flags | CRYPT_NEWKEYSET);
                        }
                    }
                }
            }
            if (_handle == IntPtr.Zero) {
                throw new CryptographicException("Could not acquire a crypto service provider handle.");
            }
        }

        public IntPtr Handle {
            get {
                return _handle;
            }
        }

        public int ProviderType {
            get {
                return PROV_RSA_FULL;
            }
        }

        public string Container {
            get {
                return _container;
            }
        }

        public void Dispose() {
            if (_handle != IntPtr.Zero)
                CryptReleaseContext(_handle, 0);
            GC.SuppressFinalize(this);
        }
        ~CryptoProviderHandle() {
            Dispose();
        }

        private IntPtr _handle;
        private string _container;

        [DllImport(@"advapi32.dll")]
        private static extern int CryptReleaseContext(IntPtr hProv, int dwFlags);

        [DllImport(@"advapi32.dll", EntryPoint = "CryptAcquireContextA", CharSet = CharSet.Ansi, SetLastError = true)] // do not remove SetLastError
        private static extern int CryptAcquireContext(ref IntPtr phProv, string pszContainer, string pszProvider, int dwProvType, int dwFlags);

        private const int CRYPT_SILENT = 0x40;
        private const int CRYPT_MACHINE_KEYSET = 0x00000020;
        private const int PROV_RSA_FULL = 1;
        private const int NTE_BAD_KEYSET = -2146893802;
        private const int CRYPT_NEWKEYSET = 0x00000008;
    }
}