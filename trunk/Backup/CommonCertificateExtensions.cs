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

namespace Org.Mentalis.Security.Tools {
    /// <summary>
    /// This class contains builder methods to generate X509Extension instances
    /// for some of the more common extensions.
    /// </summary>
    public static class CommonCertificateExtensions {
        /// <summary>
        /// Creates a Key Usage extension.
        /// </summary>
        /// <param name="keyUsage">One of the <see cref="KeyUsageExtension"/> values.</param>
        /// <returns>A <see cref="X509Extension"/> instance.</returns>
        public static X509Extension CreateKeyUsageExtension(KeyUsageExtension keyUsage) {
            return CreateKeyUsageExtension(keyUsage, false);
        }
        /// <summary>
        /// Creates a Key Usage extension.
        /// </summary>
        /// <param name="keyUsage">One of the <see cref="KeyUsageExtension"/> values.</param>
        /// <param name="critical"><b>true</b> if the extension is marked as critical, <b>false</b> otherwise.</param>
        /// <returns>A <see cref="X509Extension"/> instance.</returns>
        public static X509Extension CreateKeyUsageExtension(KeyUsageExtension keyUsage, bool critical) {
            BitStringEncodable value = new BitStringEncodable((byte)keyUsage);
            return new X509Extension(new Oid("2.5.29.15", "Key Usage"), value.ToArray(), critical);
        }

        /// <summary>
        /// Creates a Basic Constraints extension.
        /// </summary>
        /// <param name="isCA"><b>true</b> if this certificate represents a certificate authority, <b>false</b> otherwise.</param>
        /// <returns>A <see cref="X509Extension"/> instance.</returns>
        public static X509Extension CreateBasicConstraintsExtension(bool isCA) {
            return CreateBasicConstraintsExtension(isCA, -1, true);
        }
        /// <summary>
        /// Creates a Basic Constraints extension.
        /// </summary>
        /// <param name="isCA"><b>true</b> if this certificate represents a certificate authority, <b>false</b> otherwise.</param>
        /// <param name="pathLength">The maximum length of the validation path.</param>
        /// <param name="critical"><b>true</b> if the extension is marked as critical, <b>false</b> otherwise.</param>
        /// <returns>A <see cref="X509Extension"/> instance.</returns>
        /// <remarks>If <paramref name="pathLength"/> is less than or equal to zero, then the path length is omitted from the extension.</remarks>
        public static X509Extension CreateBasicConstraintsExtension(bool isCA, int pathLength, bool critical) {
            SequenceEncodable value = new SequenceEncodable();
            value.Children.Add(new BooleanEncodable(isCA));
            if (isCA && pathLength >= 0) {
                value.Children.Add(new IntegerEncodable(pathLength));
            }
            return new X509Extension(new Oid("2.5.29.19", "Basic Constraints"), value.ToArray(), critical);
        }

        /// <summary>
        /// Creates a Invalidity Date extension.
        /// </summary>
        /// <param name="invalidityDate">The date from when the certificate became invalid.</param>
        /// <returns>A <see cref="X509Extension"/> instance.</returns>
        public static X509Extension CreateInvalidityDateExtension(DateTime invalidityDate) {
            GeneralizedTimeEncodable value = new GeneralizedTimeEncodable(invalidityDate);
            return new X509Extension(new Oid("2.5.29.24", "Invalidity Date"), value.ToArray(), false);
        }

        /// <summary>
        /// Creates a CRL Number extension.
        /// </summary>
        /// <param name="crlNumber">The number of the CRL.</param>
        /// <returns>A <see cref="X509Extension"/> instance.</returns>
        public static X509Extension CreateCRLNumberExtension(int crlNumber) {
            IntegerEncodable value = new IntegerEncodable(crlNumber);
            return new X509Extension(new Oid("2.5.29.20", "CRL Number"), value.ToArray(), false);
        }

        /// <summary>
        /// Creates a Reason Code extension.
        /// </summary>
        /// <param name="reason">One of the <see cref="ReasonCodeExtension"/> values.</param>
        /// <returns>A <see cref="X509Extension"/> instance.</returns>
        public static X509Extension CreateReasonCodeExtension(ReasonCodeExtension reason) {
            IntegerEncodable value = new IntegerEncodable((int)reason);
            return new X509Extension(new Oid("2.5.29.21", "Reason Code"), value.ToArray(), false);
        }

        private interface IEncodable {
            byte[] ToArray();
            int Length { get; }
        }
        private class GeneralizedTimeEncodable : IEncodable {
            public GeneralizedTimeEncodable(DateTime value) {
                Value = value;
            }
            public byte[] ToArray() {
                byte[] buffer = new byte[_valueBuffer.Length + 2];
                buffer[0] = 0x18;
                buffer[1] = (byte)_valueBuffer.Length;
                Array.Copy(_valueBuffer, 0, buffer, 2, _valueBuffer.Length);
                return buffer;
            }
            public int Length {
                get {
                    return _valueBuffer.Length + 2;
                }
            }
            public DateTime Value {
                get {
                    return _value;
                }
                set {
                    _value = value;
                    _valueBuffer = Encoding.ASCII.GetBytes(_value.ToString("yyyyMMddHHmmss.fff").TrimEnd(new char[] { '0' }).TrimEnd(new char[] { '.' }) + "Z");
                }
            }
            private DateTime _value;
            private byte[] _valueBuffer;
        }
        private class OctetStringEncodable : IEncodable {
            public OctetStringEncodable(byte[] value) {
                _value = value;
            }
            public byte[] ToArray() {
                byte[] buffer = new byte[_value.Length + 2];
                buffer[0] = 0x4;
                buffer[1] = (byte)_value.Length;
                Array.Copy(_value, 0, buffer, 2, _value.Length);
                return buffer;
            }
            public int Length {
                get {
                    return _value.Length + 2;
                }
            }
            public byte[] Value {
                get {
                    return _value;
                }
                set {
                    _value = value;
                }
            }
            private byte[] _value;
        }
        private class BooleanEncodable : IEncodable {
            public BooleanEncodable(bool value) {
                _value = value;
            }
            public bool Value {
                get {
                    return _value;
                }
                set {
                    _value = value;
                }
            }
            public byte[] ToArray() {
                return new byte[] { 0x1, 0x1, (byte)(_value ? 0xFF : 0x0) };
            }
            public int Length {
                get {
                    return 3;
                }
            }
            private bool _value;
        }
        private class BitStringEncodable : IEncodable {
            public BitStringEncodable(byte value) {
                _value = new byte[] { value };
            }
            public BitStringEncodable(byte[] value) {
                _value = value;
            }
            public byte[] Value {
                get {
                    return _value;
                }
                set {
                    _value = value;
                }
            }
            public byte[] ToArray() {
                byte[] buffer = new byte[_value.Length + 3];
                buffer[0] = 0x3;
                buffer[1] = (byte)(_value.Length + 1);
                buffer[2] = 0x0;
                Array.Copy(_value, 0, buffer, 3, _value.Length);
                return buffer;
            }
            public int Length {
                get {
                    return _value.Length + 3;
                }
            }
            private byte[] _value;
        }
        private class SequenceEncodable : IEncodable {
            public SequenceEncodable() {
                _children = new List<IEncodable>();
            }
            public List<IEncodable> Children {
                get {
                    return _children;
                }
            }
            public byte[] ToArray() {
                int length = 0;
                foreach (IEncodable ecd in _children) {
                    length += ecd.Length;
                }
                byte[] buffer = new byte[length + 2];
                buffer[0] = 0x30;
                buffer[1] = (byte)length;
                int offset = 2;
                foreach (IEncodable ecd in _children) {
                    byte[] b = ecd.ToArray();
                    Array.Copy(b, 0, buffer, offset, b.Length);
                    offset += b.Length;
                }
                return buffer;
            }
            public int Length {
                get {
                    int length = 2;
                    foreach (IEncodable ecd in _children) {
                        length += ecd.Length;
                    }
                    return length;
                }
            }
            private List<IEncodable> _children;
        }
        private class IntegerEncodable : IEncodable {
            public IntegerEncodable(int value) {
                _value = BitConverter.GetBytes(value);
            }
            public IntegerEncodable(byte[] value) {
                _value = value;
            }
            public byte[] Value {
                get {
                    return _value;
                }
                set {
                    _value = value;
                }
            }
            public byte[] ToArray() {
                byte[] buffer = new byte[_value.Length + 2];
                buffer[0] = 0x2;
                buffer[1] = (byte)(_value.Length);
                if (BitConverter.IsLittleEndian) {
                    for (int i = 0; i < _value.Length; i++) {
                        // copy reversed
                        buffer[buffer.Length - i - 1] = _value[i];
                    }
                } else {
                    Array.Copy(_value, 0, buffer, 2, _value.Length);
                }
                return buffer;
            }
            public int Length {
                get {
                    return _value.Length + 2;
                }
            }
            private byte[] _value;
        }
    }

    /// <summary>
    /// Defines the different certificate key usage schemes.
    /// </summary>
    [Flags]
    public enum KeyUsageExtension : byte {
        /// <summary>Used for digital signatures</summary>
        DigitalSignature = 0x80,
        /// <summary>Used for non-repudiation</summary>
        NonRepudiation = 0x40,
        /// <summary>Used for key encipherment</summary>
        KeyEncipherment = 0x20,
        /// <summary>Used for data encipherment</summary>
        DataEncipherment = 0x10,
        /// <summary>Used for key agreement</summary>
        KeyAgreement = 0x8,
        /// <summary>Used for certificate signing</summary>
        KeyCertificateSign = 0x4,
        /// <summary>Used for CRL signing</summary>
        CrlSign = 0x2,
        /// <summary>Used for encipherment only</summary>
        EnciphermentOnly = 0x1
    }

    /// <summary>
    /// Defines the different certificate revocation reason codes.
    /// </summary>
    public enum ReasonCodeExtension : int {
        /// <summary>Unspecified reason</summary>
        Unspecified = 0,
        /// <summary>The certificate key was compomised</summary>
        KeyCompromise = 1,
        /// <summary>The Certificate Authority was compromised</summary>
        CACompromise = 2,
        /// <summary>The affiliations have changed</summary>
        AffiliationChanged = 3,
        /// <summary>The certificate has been superseded</summary>
        Superseded = 4,
        /// <summary>Operations have ceased</summary>
        CessationOfOperation = 5,
        /// <summary>The certificate is on hold</summary>
        CertificateHold = 6,
        /// <summary>The certificate must be removed from the CRL (because it has expired anyway). This is only used for delta-CRLs.</summary>
        RemoveFromCRL = 8
    }
}
