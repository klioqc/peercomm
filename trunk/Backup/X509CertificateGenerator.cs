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
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;

namespace Org.Mentalis.Security.Tools {
    /// <summary>
    /// Generates self-signed X509 certificates.
    /// </summary>
    public static class X509CertificateGenerator {
        /// <summary>
        /// Creates and returns a self-signed X509 certificate.
        /// </summary>
        /// <param name="key">The key of the certificate.</param>
        /// <param name="issuerName">The encoded common name of the issuer.</param>
        /// <returns>An <see cref="X509Certificate"/> instance.</returns>
        /// <exception cref="CryptographicException">An error occurs while creating the self-signed certificate.</exception>
        public static X509Certificate Create(RSACryptoServiceProvider key, string issuerName) {
            return Create(key, issuerName, null);
        }
        /// <summary>
        /// Creates and returns a self-signed X509 certificate.
        /// </summary>
        /// <param name="key">The key of the certificate.</param>
        /// <param name="issuerName">The encoded common name of the issuer.</param>
        /// <param name="settings">The settings of the X509 certificate.</param>
        /// <returns>An <see cref="X509Certificate"/> instance.</returns>
        /// <exception cref="CryptographicException">An error occurs while creating the self-signed certificate.</exception>
        public static X509Certificate Create(RSACryptoServiceProvider key, string issuerName, CertificateSettings settings) {
            if (settings == null)
                settings = new CertificateSettings();

            CspKeyContainerInfo container = key.CspKeyContainerInfo;
            IntPtr certHandle = IntPtr.Zero;
            byte[] issuerBlob = null;
            uint pcbEncoded = 0;
            if (CertStrToName(X509_ASN_ENCODING, issuerName, CERT_X500_NAME_STR, IntPtr.Zero, null, ref pcbEncoded, IntPtr.Zero)) {
                issuerBlob = new byte[pcbEncoded];
                CertStrToName(X509_ASN_ENCODING, issuerName, CERT_X500_NAME_STR, IntPtr.Zero, issuerBlob, ref pcbEncoded, IntPtr.Zero);
            } else {
                throw new CryptographicException("Cannot encode the issuer name! Please check whether the string is in a valid format.");
            }

            List<IntPtr> extensionList = new List<IntPtr>();
            CERT_NAME_BLOB subject = new CERT_NAME_BLOB();
            try {
                subject.pbData = Marshal.AllocHGlobal(issuerBlob.Length);
                Marshal.Copy(issuerBlob, 0, subject.pbData, issuerBlob.Length);
                subject.cbData = issuerBlob.Length;

                SYSTEMTIME startDate = ConvertDateTime(settings.StartDate);
                SYSTEMTIME endDate = ConvertDateTime(settings.EndDate);

                CRYPT_KEY_PROV_INFO providerInfo = new CRYPT_KEY_PROV_INFO();
                providerInfo.pwszContainerName = container.KeyContainerName;
                providerInfo.pwszProvName = container.ProviderName;
                providerInfo.dwProvType = container.ProviderType;
                providerInfo.dwFlags = (int)0;
                providerInfo.cProvParam = 0;
                providerInfo.rgProvParam = IntPtr.Zero;
                providerInfo.dwKeySpec = (int)container.KeyNumber;

                // convert the list of extensions to an unmanaged structure
                // the .NET marshallers don't handle recursive arrays too well,
                // so we do it manually :-/
                extensionList = ConvertExtensions(settings.Extensions);

                certHandle = CertCreateSelfSignCertificate(IntPtr.Zero, ref subject, settings.Flags, ref providerInfo, IntPtr.Zero, ref startDate, ref endDate, extensionList[0]);
                if (certHandle == IntPtr.Zero) {
                    throw new CryptographicException("Couldn't create unsigned certificate");
                }

                return new X509Certificate2(certHandle);
            } finally {
                if (subject.pbData != IntPtr.Zero)
                    Marshal.FreeHGlobal(subject.pbData);
                if (extensionList != null && extensionList.Count > 0) {
                    foreach (IntPtr ptr in extensionList) {
                        if (ptr != IntPtr.Zero)
                            Marshal.FreeHGlobal(ptr);
                    }
                }
            }
        }

        // returns a list of IntPtr's; the first IntPtr is the pointer to the CERT_EXTENSIONS
        // strucuture; the other pointers are pointers that have to be released in order
        // to avoid leaking memory
        private unsafe static List<IntPtr> ConvertExtensions(X509ExtensionCollection extensions) {
            List<IntPtr> ret = new List<IntPtr>();
            if (extensions == null && extensions.Count == 0) {
                ret.Add(IntPtr.Zero);
                return ret;
            }

            int extensionStructSize = Marshal.SizeOf(typeof(CERT_EXTENSION));
            // create the pointer to the CERT_EXTENSIONS object
            CERT_EXTENSIONS extensionsStruct = new CERT_EXTENSIONS();
            IntPtr extensionsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CERT_EXTENSIONS)));
            ret.Add(extensionsPtr);
            extensionsStruct.cExtension = extensions.Count;
            extensionsStruct.rgExtension = Marshal.AllocHGlobal(extensionStructSize * extensions.Count); ;
            ret.Add(extensionsStruct.rgExtension);
            Marshal.StructureToPtr(extensionsStruct, extensionsPtr, false);

            // create the array of CERT_EXTENSION objects
            CERT_EXTENSION extensionStruct = new CERT_EXTENSION();
            byte* workPointer = (byte*)extensionsStruct.rgExtension.ToPointer();
            foreach(X509Extension ext in extensions) {
                // initialize the extension structure
                extensionStruct.pszObjId = Marshal.StringToHGlobalAnsi(ext.Oid.Value);
                ret.Add(extensionStruct.pszObjId);
                extensionStruct.fCritical = ext.Critical ? 1 : 0;
                byte[] rawData = ext.RawData;
                extensionStruct.cbData = rawData.Length;
                extensionStruct.pbData = Marshal.AllocHGlobal(rawData.Length); ;
                Marshal.Copy(rawData, 0, extensionStruct.pbData, rawData.Length);
                ret.Add(extensionStruct.pbData);
                // copy it to unmanaged memory
                Marshal.StructureToPtr(extensionStruct, new IntPtr(workPointer), false);
                workPointer += extensionStructSize;
            }
            // everything successfully created; return
            return ret;
        }

        private static SYSTEMTIME ConvertDateTime(DateTime input) {
            SYSTEMTIME ret = new SYSTEMTIME();
            ret.Milliseconds = (short)input.Millisecond;
            ret.Second = (short)input.Second;
            ret.Minute = (short)input.Minute;
            ret.Hour = (short)input.Hour;
            ret.Day = (short)input.Day;
            ret.DayOfWeek = (short)input.DayOfWeek;
            ret.Month = (short)input.Month;
            ret.Year = (short)input.Year;
            return ret;
        }

        [DllImport("crypt32.dll", SetLastError = true)]
        private static extern IntPtr CertCreateSelfSignCertificate(
            IntPtr hProv,
            ref CERT_NAME_BLOB pSubjectIssuerBlob,
            uint dwFlagsm,
            ref CRYPT_KEY_PROV_INFO pKeyProvInfo,
            IntPtr pSignatureAlgorithm,
            ref SYSTEMTIME pStartTime,
            ref SYSTEMTIME pEndTime,
            IntPtr other);

        [DllImport("crypt32.dll", SetLastError = true)]
        private static extern bool CertStrToName(
            uint dwCertEncodingType,
            String pszX500,
            uint dwStrType,
            IntPtr pvReserved,
            [In, Out] byte[] pbEncoded,
            ref uint pcbEncoded,
            IntPtr other);

        [StructLayout(LayoutKind.Sequential)]
        private struct CERT_NAME_BLOB {
            public int cbData;
            public IntPtr pbData;
        }

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

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME {
            [MarshalAs(UnmanagedType.U2)]
            public short Year;
            [MarshalAs(UnmanagedType.U2)]
            public short Month;
            [MarshalAs(UnmanagedType.U2)]
            public short DayOfWeek;
            [MarshalAs(UnmanagedType.U2)]
            public short Day;
            [MarshalAs(UnmanagedType.U2)]
            public short Hour;
            [MarshalAs(UnmanagedType.U2)]
            public short Minute;
            [MarshalAs(UnmanagedType.U2)]
            public short Second;
            [MarshalAs(UnmanagedType.U2)]
            public short Milliseconds;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CERT_EXTENSION {
            public IntPtr pszObjId;
            public int fCritical;
            public int cbData;
            public IntPtr pbData;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CERT_EXTENSIONS {
            public int cExtension;
            public IntPtr rgExtension;
        }

        private const uint X509_ASN_ENCODING = 0x00000001;
        private const uint CERT_X500_NAME_STR = 3;
    }
}