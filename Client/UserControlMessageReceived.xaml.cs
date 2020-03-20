using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for UserControlMessageReceived.xaml
    /// </summary>
    public partial class UserControlMessageReceived : UserControl
    {
        public UserControlMessageReceived(string msg, string date)
        {
            InitializeComponent();
            tbMessage.Text = msg;
            tbDate.Text = date;
        }
    }
}
