using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.PeerResolvers;

namespace cir.PeerComm
{
    /// <summary>
    /// Serves as a wrapper for the IComm interface allowing it to 
    /// be used for two-way communication via an IClientChannel
    /// </summary>
    public interface ICommDuplexChannel : IComm, IClientChannel
    {}
}
