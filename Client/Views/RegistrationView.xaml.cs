using Client.Models;
using System;
using System.Windows;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for RegistrationView.xaml
    /// </summary>
    public partial class RegistrationView : Window
    {
        CClient cClient;
        public RegistrationView()
        {
            InitializeComponent();

            cClient = new CClient();
            cClient.RegistrationOK += new EventHandler(cOnRegistrationOK);
            cClient.RegistrationNotOk += new EventHandler(cOnRegistrationNotOK);
            cClient.LoginNotOk += new EventHandler(cOnLoginNotOk);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            bool validEmail;
            bool validPassword;

            validEmail = IsValidEmail(tbEmail.Text);
            validPassword = tbPassword.Text == tbPassword_Again.Text;

            #region Bedingungen
            if (!validEmail && !validPassword) // 00
            {
                MessageBox.Show("Bitte überprüfen Sie Ihr Passwort und Ihre E-Mail Adresse.");
            }
            if (!validEmail && validPassword) //01
            {
                MessageBox.Show("Bitte überprüfen Sie Ihre E-Mail Adresse.");
            }
            if (validEmail && !validPassword) //10
            {
                MessageBox.Show("Bitte überprüfen Sie Ihr Passwort.");
            }
            if (validEmail && validPassword) //11
            {
                cClient.FsName = tbfsName.Text; //Vor- und Nachname initialisieren
                cClient.Connect(tbEmail.Text, tbPassword.Text, true);
            }
            #endregion




        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                //return addr.Address == email;
                return true;
            }
            catch
            {
                return false;
            }
        }

        void cOnLoginNotOk(object sender, EventArgs e)
        {
            MessageBox.Show("Es konnte keine Verbindung zum Server hergestellt werden.");
        }
        void cOnRegistrationOK(object sender, EventArgs e)
        {
            MessageBox.Show("Die Registrierung war erfolgreich. Sie können sich nun erfolgreich anmelden");
            Application.Current.Dispatcher.Invoke((Action)delegate
                       {
                           this.Close();
                       });
        }


        void cOnRegistrationNotOK(object sender, EventArgs e)
        {
            MessageBox.Show("Die Registrierung war nicht erfolgreich. Diese E-Mail Adresse existiert bereits. Bitte versuchen Sie es erneut.");
        }
    }
}
