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

namespace Org.Mentalis.Security.Tools {
    /// <summary>
    /// The RSAExponentOfOne class generates so-called 'exponent-of-one' RSA keys. These keys are
    /// special RSA keys, in the sense that the output buffer matches the input buffer for an
    /// RSA encryption or decryption. Practically, this means that if you generate a key-exchange
    /// message using a RSAPKCS1KeyExchangeFormatter class that was initialized with such a special
    /// key, the resulting key exchange buffer will include the secret key unencrypted.
    /// Exponent-of-one keys are very useful when you're in the debugging stage of your application.
    /// You can use these keys instead of 'normal' RSA keys, and the entire system keeps working
    /// as expected. The only difference is that the 'encrypted' data is the same as the unencrypted
    /// data - a very useful feature for debugging purposes.
    /// </summary>
    /// <remarks>
    /// Exponent-of-one keys should <b>never</b> be used in production! They do not offer any
    /// security whatsoever.
    /// </remarks>
    public static class RSAExponentOfOne {
        /// <summary>
        /// Creates a new exponent-of-one key.
        /// </summary>
        /// <returns>An <see cref="RSA"/> instance that represents the exponen-of-one key.</returns>
        /// <remarks>The modulus length of this key will be 1024 bits.</remarks>
        public static RSA Create() {
            return Create(1024);
        }
        /// <summary>
        /// Creates a new exponent-of-one key.
        /// </summary>
        /// <param name="bitLength">The length of the modulus, in bits.</param>
        /// <returns>An <see cref="RSA"/> instance that represents the exponen-of-one key.</returns>
        public static RSA Create(int bitLength) {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(bitLength);
            RSAParameters pars = rsa.ExportParameters(true);
            pars.Exponent = new byte[] { 1 };
            byte[] eoo = new byte[pars.Modulus.Length / 2];
            eoo[eoo.Length - 1] = 1;
            pars.DP = eoo;
            pars.DQ = eoo;
            pars.D = new byte[pars.Modulus.Length];
            pars.D[pars.D.Length - 1] = 1;
            rsa.ImportParameters(pars);
            return rsa;
        }
    }
}
