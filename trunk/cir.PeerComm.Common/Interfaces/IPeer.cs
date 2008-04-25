using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cir.PeerComm
{
    /// <summary>
    /// Describes what a Peer object will look like, modify this to your needs as this is only an example
    /// </summary>
    public interface IPeer
    {
        Guid ID
        { get; set; }

        string Name
        { get; set; }

        string Description
        { get; set; }

    }
}
