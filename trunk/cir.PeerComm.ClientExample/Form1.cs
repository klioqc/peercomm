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
            X509Certificate2 cert = cir.PeerComm.Certificate.CreateCertificate("FiveInchFish", "c:\\fif.cer");

            if (cert != null)
            { Debug.WriteLine(cert.ToString()); }
        }

        private void _MakeSatAss_Click(object sender, EventArgs e)
        {
            cir.PeerComm.Certificate.buildSatelliteAssembly();
        }
    }
}
