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
using System.Security.Cryptography;
using System.IO;

namespace Org.Mentalis.Security.Tools {
    /// <summary>
    /// Represents a Strong Name file (also known as an SNK file).
    /// </summary>
    public sealed class StrongNameFile : IDisposable {
        /// <summary>
        /// Initializes a new instance of the StrongNameFile class.
        /// </summary>
        /// <param name="rsa">The private key that this strong name file represents.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rsa"/> is a null reference.</exception>
        public StrongNameFile(RSA rsa) {
            if (rsa == null)
                throw new ArgumentNullException("rsa");
            RSAParameters parameters;
            bool includePrivate = true;
            try {
                parameters = rsa.ExportParameters(true);
            } catch { 
                // retry without private parameters
                parameters = rsa.ExportParameters(false);
                includePrivate = false;
            }
            InitializeFromParameters(parameters, includePrivate);
        }
        /// <summary>
        /// Initializes a new instance of the StrongNameFile class.
        /// </summary>
        /// <param name="rsa">The private key that this strong name file represents.</param>
        /// <exception cref="ArgumentException"><paramref name="rsa"/> contains invalid data.</exception>
        public StrongNameFile(RSAParameters rsa) {
            RSAParameters rsaClone = SecurityTools.CloneRSAParameters(rsa);
            InitializeFromParameters(rsaClone, (rsaClone.P != null && rsaClone.Q != null));
        }
        /// <summary>
        /// Initializes a new instance of the StrongNameFile class.
        /// </summary>
        /// <param name="file">The path to the SNK file to open.</param>
        /// <exception cref="FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="InvalidDataException">The file contains invalid data.</exception>
        public StrongNameFile(string file) : this(new FileInfo(file)) { }
        /// <summary>
        /// Initializes a new instance of the StrongNameFile class.
        /// </summary>
        /// <param name="file">The SNK file to open.</param>
        /// <exception cref="FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="InvalidDataException">The file contains invalid data.</exception>
        public StrongNameFile(FileInfo file) {
            RSAParameters pars = ExtractParametersFromFile(file);
            InitializeFromParameters(pars, (pars.P != null && pars.Q != null));
        }

        private RSAParameters ExtractParametersFromFile(FileInfo file) {
            if (!file.Exists)
                throw new FileNotFoundException("The specified file doesn't exist.");
            RSAParameters ret = new RSAParameters();
            using (BinaryReader reader = new BinaryReader(file.OpenRead())) {
                // read the BLOBHEADER
                byte blobType = reader.ReadByte();
                if ((BlobType)blobType != BlobType.PrivateKey) {
                    // we might be dealing with an SNK file that contains
                    // only the public key
                    reader.BaseStream.Seek(12, SeekOrigin.Begin);
                    blobType = reader.ReadByte();
                    if ((BlobType)blobType != BlobType.PublicKey)
                        throw new InvalidDataException("The specified file is not an SNK file.");
                }
                reader.BaseStream.Seek(3, SeekOrigin.Current);
                if ((AlgorithmType)reader.ReadInt32() != AlgorithmType.RsaSignature)
                    throw new InvalidDataException("The key in the SNK file is not a signature key.");
                // read the RSAPUBKEY
                int rsaKeyType = reader.ReadInt32();
                if (!((RsaKeyIdentifier)rsaKeyType == RsaKeyIdentifier.PrivateKey && (BlobType)blobType == BlobType.PrivateKey)
                        && !((RsaKeyIdentifier)rsaKeyType == RsaKeyIdentifier.PublicKey && (BlobType)blobType == BlobType.PublicKey))
                    throw new InvalidDataException("The BLOBHEADER doesn't correspond with the RSAPUBKEY.");
                int bitLen = reader.ReadInt32();
                if (bitLen % 8 != 0)
                    throw new InvalidDataException("The public key length is invalid.");
                ret.Exponent = SecurityTools.TrimTrailingNulls(reader.ReadBytes(4));
                Array.Reverse(ret.Exponent); // CryptoAPI uses little-endian, .NET uses big-endian
                // read the modulus
                int modLength = bitLen >> 3;
                ret.Modulus = reader.ReadBytes(modLength);
                Array.Reverse(ret.Modulus);
                if (ret.Modulus.Length != modLength)
                    throw new InvalidDataException("Unexpected end-of-file found.");
                // read the private parameters, if available
                if ((BlobType)blobType == BlobType.PrivateKey) {
                    int secLen = bitLen >> 4;
                    ret.P = reader.ReadBytes(secLen);
                    Array.Reverse(ret.P);
                    ret.Q = reader.ReadBytes(secLen);
                    Array.Reverse(ret.Q);
                    ret.DP = reader.ReadBytes(secLen);
                    Array.Reverse(ret.DP);
                    ret.DQ = reader.ReadBytes(secLen);
                    Array.Reverse(ret.DQ);
                    ret.InverseQ = reader.ReadBytes(secLen);
                    Array.Reverse(ret.InverseQ);
                    ret.D = reader.ReadBytes(modLength);
                    Array.Reverse(ret.D);
                    if (ret.P.Length != secLen || ret.Q.Length != secLen || ret.DP.Length != secLen ||
                            ret.DQ.Length != secLen || ret.InverseQ.Length != secLen || ret.D.Length != modLength)
                        throw new InvalidDataException("Unexpected end-of-file found.");
                }
            }
            return ret;
        }

        private void InitializeFromParameters(RSAParameters rsa, bool includesPrivate) {
            if (rsa.Modulus == null || rsa.Exponent == null)
                throw new ArgumentException("The modulus and exponent are required parameters.");
            if (includesPrivate && (rsa.P == null || rsa.Q == null || rsa.DP == null || rsa.DQ == null || rsa.InverseQ == null || rsa.D == null))
                throw new ArgumentException("The private parameters are required to initialize a ");
            _hasPrivateKey = includesPrivate;
            _keyParameters = rsa;
            _publicKeyParameters = new RSAParameters();
            _publicKeyParameters.Modulus = rsa.Modulus;
            _publicKeyParameters.Exponent = rsa.Exponent;
        }

        /// <summary>
        /// Gets a value that indicates whether the SNK file contains a private key or not.
        /// </summary>
        /// <value><b>true</b> if the file contains a private key, <b>false</b> otherwise.</value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public bool HasPrivateKey {
            get {
                if (_disposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                return _hasPrivateKey;
            }
        }

        /// <summary>
        /// Gets an <see cref="RSA"/> instance that represents the public key.
        /// </summary>
        /// <value>An <see cref="RSA"/> instance.</value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public RSA PublicKey {
            get {
                if (_disposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(512); // initialize with a (small) 512-bit key
                rsa.ImportParameters(_publicKeyParameters);
                return rsa;
            }
        }

        /// <summary>
        /// Gets an <see cref="RSA"/> instance that represents the public/private key pair.
        /// </summary>
        /// <value>An <see cref="RSA"/> instance.</value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <exception cref="NotSupportedException">The private key is not available.</exception>
        public RSA PrivateKey {
            get {
                if (_disposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                if (!this.HasPrivateKey)
                    throw new NotSupportedException("The private key is not available.");
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(512); // initialize with a (small) 512-bit key
                rsa.ImportParameters(_keyParameters);
                return rsa;
            }
        }

        /// <summary>
        /// Saves this StrongNameFile instance to disk in the SNK file format.
        /// </summary>
        /// <param name="fileName">The filename to save to.</param>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <exception cref="NotSupportedException">The data cannot be exported.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is a null reference.</exception>
        public void Save(string fileName) {
            Save(fileName, this.HasPrivateKey);
        }
        /// <summary>
        /// Saves this StrongNameFile instance to disk in the SNK file format.
        /// </summary>
        /// <param name="fileName">The filename to save to.</param>
        /// <param name="includePrivateKey"><b>true</b> if the private key must be included, <b>false</b> otherwise.</param>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <exception cref="NotSupportedException">The private key is not available or the data cannot be exported.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is a null reference.</exception>
        public void Save(string fileName, bool includePrivateKey) {
            if (_disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            if (includePrivateKey && !this.HasPrivateKey)
                throw new NotSupportedException("The private key is not available.");
            if (fileName == null)
                throw new ArgumentNullException("file");
            if (SecurityTools.TrimLeadingNulls(_publicKeyParameters.Exponent).Length > 4)
                throw new NotSupportedException("The public exponent must not exceed more than 32 bits!");
            
            FileInfo file = new FileInfo(fileName);
            using (BinaryWriter writer = new BinaryWriter(file.OpenWrite())) {
                // initialize the header bytes (default to 'private key')
                byte[] header = new byte[] { 0x7, 0x2, 0x0, 0x0, 0x0, 0x24, 0x0, 0x0, 0x52, 0x53, 0x41, 0x32 };
                // write the BLOBHEADER
                if (!includePrivateKey) {
                    // write the 'SigAlgID' and 'HashAlgID' fields
                    writer.Write(new byte[] { 0x0, 0x24, 0x0, 0x0, 0x4, 0x80, 0x0, 0x0 });
                    // write the 'cbPublicKey' field
                    uint size = (uint)(8 /* sizeof(BLOBHEADER) */ + 12 /* sizeof(RSAPUBKEY) */ + _publicKeyParameters.Modulus.Length);
                    writer.Write(size);
                    // modify the header
                    header[0] = 0x6; // this is only a public key
                    header[11] = 0x31; // magic is RSA1, not RSA2
                }
                // write the header bytes (the BLOBHEADER, and the 'magic' field of the RSAPUBKEY)
                writer.Write(header);
                // write the bit length
                uint bitLen = (uint)(_publicKeyParameters.Modulus.Length * 8);
                writer.Write(bitLen);
                // write the public exponent
                byte[] exponentBuffer = new byte[4];
                if (_publicKeyParameters.Exponent.Length < 4) {
                    Array.Copy(_publicKeyParameters.Exponent, 0, exponentBuffer, 4 - _publicKeyParameters.Exponent.Length, _publicKeyParameters.Exponent.Length);
                } else {
                    Array.Copy(_publicKeyParameters.Exponent, _publicKeyParameters.Exponent.Length - 4, exponentBuffer, 0, 4);
                }
                Array.Reverse(exponentBuffer); // write it as little-endian
                writer.Write(exponentBuffer);
                // write the modulus
                WriteBufferRev(writer, _publicKeyParameters.Modulus);
                // write the private variables
                if (includePrivateKey) {
                    WriteBufferRev(writer, _keyParameters.P);
                    WriteBufferRev(writer, _keyParameters.Q);
                    WriteBufferRev(writer, _keyParameters.DP);
                    WriteBufferRev(writer, _keyParameters.DQ);
                    WriteBufferRev(writer, _keyParameters.InverseQ);
                    WriteBufferRev(writer, _keyParameters.D);
                }
            }
        }
        private void WriteBufferRev(BinaryWriter writer, byte[] input) {
            byte[] buffer = (byte[])input.Clone();
            Array.Reverse(buffer);
            writer.Write(buffer);
            Array.Clear(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Releases all managed and unmanaged resources.
        /// </summary>
        public void Dispose() {
            if (!_disposed) {
                _disposed = true;
                // zero out sensitive data
                SecurityTools.ClearRSAParameters(_keyParameters);
                SecurityTools.ClearRSAParameters(_publicKeyParameters);
                
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Releases all managed and unmanaged resources.
        /// </summary>
        ~StrongNameFile() {
            Dispose();
        }

        private RSAParameters _keyParameters;
        private RSAParameters _publicKeyParameters;
        private bool _hasPrivateKey;
        private bool _disposed;
    }

    internal enum BlobType { 
        PublicKey = 0x6,
        PrivateKey = 0x7
    }

    internal enum AlgorithmType {
        RsaSignature = 0x00002400
    }

    internal enum RsaKeyIdentifier { 
        PublicKey = 0x31415352, //RSA1
        PrivateKey = 0x32415352 //RSA2
    }
}