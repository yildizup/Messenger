using System.Windows.Controls;
using System.Windows.Media;

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
            SetNewMessages(newMessages);

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

        void SetNewMessages(int count)
        {
            if (count == 0)
            {
                lblNewMessages.Content = "";
            }
            else
            {
                lblNewMessages.Content = count.ToString();
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
                SetNewMessages(newMessages);
            }
        }

    }
}
