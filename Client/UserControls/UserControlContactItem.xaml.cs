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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="status">Ist der User online ? </param>
        public UserControlContactItem(string email, bool status, int NewMessages)
        {
            InitializeComponent();
            this.email = email;
            tbEmail.Text = this.email;
            lblNewMessages.Content = NewMessages.ToString();

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
        }
    }
}
