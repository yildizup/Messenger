using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for UserControlMessageSent.xaml
    /// </summary>
    public partial class UserControlMessageSent : UserControl
    {
        public UserControlMessageSent(string msg, string date)
        {
            InitializeComponent();
            tbMessage.Text = msg;
            tbDate.Text = date;
        }
    }
}
