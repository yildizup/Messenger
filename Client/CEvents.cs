using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{

    public class CReceivedEventArgs : EventArgs
    {
        string email;
        string msg;

        public CReceivedEventArgs(string user, string msg)
        {
            this.email = user;
            this.msg = msg;
        }

        public string From
        {
            get { return email; }
        }
        public string Message
        {
            get { return msg; }
        }
    }

    public class CChatContentEventArgs : EventArgs
    {
        DataTable dtChat;

        public CChatContentEventArgs(DataTable dt)
        {
            dtChat = dt;
        }

        public DataTable DtChat
        {
            get { return dtChat; }
        }
    }



    public delegate void CReceivedEventHandler(object sender, CReceivedEventArgs e);
    public delegate void CChatContentEventHandler(object sender, CChatContentEventArgs e);

}
