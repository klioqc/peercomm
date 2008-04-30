using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cir.PeerComm
{
    /// <summary>
    /// Represents the data being transmitted  
    /// </summary>
    public struct Message : IMessage
    {
        private string _MessageSubject;

        /// <summary>
        /// Like in an email, this is the subject line.
        /// An example field, feel free to change or remove this.
        /// </summary>
        public string MessageSubject
        {
            get { return _MessageSubject; }
            set { _MessageSubject = value; }
        }

        private string _MessageBody;

        /// <summary>
        /// Some message text, what good is a message if there's no info in it?.
        /// An example field, feel free to change or remove this.
        /// You might want to send a custom struct or class instead of just some text.
        /// </summary>
        public string MessageBody
        {
            get { return _MessageBody; }
            set { _MessageBody = value; }
        }


        #region IMessage Members

        private Guid _MessageID;
        public Guid MessageID
        {
            get
            { return _MessageID; }
            set
            { _MessageID = value; }
        }


        private Guid _MessageTo;
        public Guid MessageTo
        {
            get
            { return _MessageTo; }
            set
            { _MessageTo = value; }
        }

        private Guid _MessageFrom;
        public Guid MessageFrom
        {
            get
            { return _MessageFrom; }
            set
            { _MessageFrom = value; }
        }

        #endregion
    }
}
