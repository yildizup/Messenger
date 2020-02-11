using System;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for windowRegister.xaml
    /// </summary>
    public partial class WndRegistration : Window
    {
        string password;
        CClient cClient;
        public WndRegistration()
        {
            InitializeComponent();

            cClient = new CClient();
            cClient.RegistrationOK += new EventHandler(cOnRegistrationOK);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {

            if (tbPassword.Text == tbPassword_Again.Text)
            {
                password = tbPassword.Text;
            }

            cClient.ConnectToRegistrate(tbEmail.Text, password);

        }

        void cOnRegistrationOK(object sender, EventArgs e)
        {


            MessageBox.Show("Alles Ok");



        }
    }
}
