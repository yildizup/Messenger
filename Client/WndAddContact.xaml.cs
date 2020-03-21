using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for WndAddContact.xaml
    /// </summary>
    public partial class WndAddContact : Window
    {
        public WndAddContact()
        {
            InitializeComponent();
        }

        private void btnAddContact_Click(object sender, RoutedEventArgs e)
        {

            Email = tbEmail.Text;
            this.Close();
        }

        public string Email { get; set; }
    }
}
