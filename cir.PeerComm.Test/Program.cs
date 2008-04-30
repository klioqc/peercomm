using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


using System.Diagnostics;

// http://channel9.msdn.com/wiki/default.aspx/SecurityWiki.ValidateRSASigCode

namespace cir.PeerComm.Test
{
    class Program
    {


        static void Main(string[] args)
        {

            cir.PeerComm.Security.DigitalSignature signature = new cir.PeerComm.Security.DigitalSignature();

            Message msg = new cir.PeerComm.Message();

            msg.MessageFrom = Guid.NewGuid();
            msg.MessageTo = Guid.NewGuid();
            msg.MessageID = Guid.NewGuid();
            msg.MessageText = "Blah, blah, blah.";

            byte[] sigBytes = signature.SignMessage(message);

            Console.ReadLine();
        }

    }
}
