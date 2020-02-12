﻿using System;
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
            cClient.RegistrationNotOk += new EventHandler(cOnRegistrationNotOK);
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
