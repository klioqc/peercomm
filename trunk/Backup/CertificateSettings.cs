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

namespace Org.Mentalis.Security.Tools {
    /// <summary>
    /// Holds the different settings for the self-signed certificate generation process.
    /// </summary>
    public class CertificateSettings {
        /// <summary>
        /// Initializes a new version of the CertificateSettings class.
        /// </summary>
        public CertificateSettings() {
            _startDate = DateTime.Now;
            _endDate = DateTime.Now.AddYears(1);
            _signCertificate = true;
            _referenceKey = true;
            _extensions = new X509ExtensionCollection();
        }

        /// <summary>
        /// Gets or sets a value that represents the start date of the certificate.
        /// </summary>
        /// <value>A <see cref="DateTime"/> instance.</value>
        public DateTime StartDate {
            get {
                return _startDate;
            }
            set {
                _startDate = value;
            }
        }
        /// <summary>
        /// Gets or sets a value that represents the end date of the certificate.
        /// </summary>
        /// <value>A <see cref="DateTime"/> instance.</value>
        public DateTime EndDate {
            get {
                return _endDate;
            }
            set {
                _endDate = value;
            }
        }
        /// <summary>
        /// Gets or sets a value that indicates whether the certificate should be signed.
        /// </summary>
        /// <value><b>true</b> if a signature must be generated, <b>false</b> otherwise.</value>
        public bool SignCertificate {
            get {
                return _signCertificate;
            }
            set {
                _signCertificate = value;
            }
        }
        /// <summary>
        /// Gets or sets a value that indicates whether the certificate must reference its private key.
        /// </summary>
        /// <value><b>true</b> if the private key must be referenced, <b>false</b> otherwise.</value>
        public bool ReferencePrivateKey {
            get {
                return _referenceKey;
            }
            set {
                _referenceKey = value;
            }
        }
        /// <summary>
        /// Gets a collection of X509 extensions.
        /// </summary>
        /// <value>A <see cref="X509ExtensionCollection"/> instance.</value>
        public X509ExtensionCollection Extensions {
            get {
                return _extensions;
            }
        }

        internal uint Flags {
            get {
                uint ret = 0;
                if (!this.SignCertificate)
                    ret |= CERT_CREATE_SELFSIGN_NO_SIGN;
                if (!this.ReferencePrivateKey)
                    ret |= CERT_CREATE_SELFSIGN_NO_KEY_INFO;
                return ret;
            }
        }

        private bool _referenceKey;
        private DateTime _startDate;
        private DateTime _endDate;
        private bool _signCertificate;
        private X509ExtensionCollection _extensions;

        private const uint CERT_CREATE_SELFSIGN_NO_SIGN = 1;
        private const uint CERT_CREATE_SELFSIGN_NO_KEY_INFO = 2;
    }
}
