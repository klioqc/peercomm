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
    internal static class SecurityTools {
        // clears an RSAParameters structure by zeroing-out all the
        // byte arrays it contains
        public static void ClearRSAParameters(RSAParameters pars) {
            if (pars.D != null) Array.Clear(pars.D, 0, pars.D.Length);
            if (pars.DP != null) Array.Clear(pars.DP, 0, pars.DP.Length);
            if (pars.DQ != null) Array.Clear(pars.DQ, 0, pars.DQ.Length);
            if (pars.Exponent != null) Array.Clear(pars.Exponent, 0, pars.Exponent.Length);
            if (pars.InverseQ != null) Array.Clear(pars.InverseQ, 0, pars.InverseQ.Length);
            if (pars.Modulus != null) Array.Clear(pars.Modulus, 0, pars.Modulus.Length);
            if (pars.P != null) Array.Clear(pars.P, 0, pars.P.Length);
            if (pars.Q != null) Array.Clear(pars.Q, 0, pars.Q.Length);
        }

        // clones an RSAParameters structure
        public static RSAParameters CloneRSAParameters(RSAParameters rsa) {
            RSAParameters rsaClone = new RSAParameters();
            if (rsa.D != null) rsaClone.D = (byte[])rsa.D.Clone();
            if (rsa.DP != null) rsaClone.DP = (byte[])rsa.DP.Clone();
            if (rsa.DQ != null) rsaClone.DQ = (byte[])rsa.DQ.Clone();
            if (rsa.Exponent != null) rsaClone.Exponent = (byte[])rsa.Exponent.Clone();
            if (rsa.InverseQ != null) rsaClone.InverseQ = (byte[])rsa.InverseQ.Clone();
            if (rsa.Modulus != null) rsaClone.Modulus = (byte[])rsa.Modulus.Clone();
            if (rsa.P != null) rsaClone.P = (byte[])rsa.P.Clone();
            if (rsa.Q != null) rsaClone.Q = (byte[])rsa.Q.Clone();
            return rsaClone;
        }

        public static byte[] TrimTrailingNulls(byte[] input) {
            int ic = 0;
            for (int i = input.Length - 1; i >= 0; i--) {
                if (input[i] != 0)
                    break;
                ic++;
            }
            if (ic == 0) {
                return input;
            } else if (ic == input.Length) {
                return new byte[] { 0 };
            } else {
                byte[] ret = new byte[input.Length - ic];
                Array.Copy(input, ret, ret.Length);
                return ret;
            }
        }

        public static byte[] TrimLeadingNulls(byte[] input) {
            int ic = 0;
            for (int i = 0; i < input.Length; i++) {
                if (input[i] != 0)
                    break;
                ic++;
            }
            if (ic == 0) {
                return input;
            } else if (ic == input.Length) {
                return new byte[] { 0 };
            } else {
                byte[] ret = new byte[input.Length - ic];
                Array.Copy(input, ic, ret, 0, ret.Length);
                return ret;
            }
        }

    }
}
