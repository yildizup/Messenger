using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for UserControlContactItem.xaml
    /// </summary>
    public partial class UserControlContactItem : UserControl
    {
        private string email;
        private bool status;
        private int newMessages;
        private string fsname; //vor- und nachname
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="status">Ist der User online ? </param>
        public UserControlContactItem(string email, bool status, int newMessages, string fsname)
        {
            InitializeComponent();
            this.email = email;

            this.status = status;
            SetStatusColor();

            this.newMessages = newMessages;
            lblNewMessages.Content = newMessages.ToString();

            this.fsname = fsname;
            tbName.Text = this.fsname;



        }

        void SetStatusColor()
        {
            if (status)
            {
                borderStatus.Background = Brushes.LightGreen;
            }
            else
            {
                borderStatus.Background = Brushes.DarkRed;
            }
        }

        public string Email
        {
            get { return email; }
            //set { email = value; }. Die Email ist unveränderbar
        }
        public bool Status
        {
            get { return status; }
            set
            {
                status = value;
                SetStatusColor();
            }
        }

        public int NewMessages
        {
            get { return newMessages; }
            set
            {
                newMessages = value;
                lblNewMessages.Content = newMessages.ToString();
            }
        }

    }
}
