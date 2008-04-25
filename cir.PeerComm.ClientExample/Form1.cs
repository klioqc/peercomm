using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Security.Cryptography.X509Certificates;

using System.Diagnostics;

namespace cir.PeerComm.ClientExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void _MakeCert_Click(object sender, EventArgs e)
        {
            bool certMade = cir.PeerComm.Security.Certificate.CreateCertificate("FiveInchFish", "fif", cir.PeerComm.Security.Certificate.KeyTypes.exchange,"BlahBlahBlah");
            X509Certificate2 cert = cir.PeerComm.Security.Certificate.ReadCert("fif.pfx");

            if (cert != null)
            { 
                Debug.WriteLine(cert.ToString());
                Debug.WriteLine(cert.PrivateKey.ToString());
            }
        }

        private void _MakeSatAss_Click(object sender, EventArgs e)
        {
            //cir.PeerComm.Security.Certificate.buildSatelliteAssembly();
        }
    }
}
