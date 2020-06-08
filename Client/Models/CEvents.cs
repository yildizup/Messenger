using System;
using System.Data;

namespace Client
{

    public class CReceivedEventArgs : EventArgs
    {
        string email;
        string message;
        string date;

        public CReceivedEventArgs(string user, string message, string date)
        {
            this.email = user;
            this.message = message;
            this.date = date;
        }

        public string From
        {
            get { return email; }
        }

        public string Message
        {
            get { return message; }
        }
        public string Date
        {
            get { return date; }
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
