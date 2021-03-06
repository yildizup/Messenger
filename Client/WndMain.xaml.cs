﻿using System;
using System.Net.Sockets;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for WndMain.xaml
    /// </summary>
    public partial class WndMain : Window
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        CClient cClient;
        public WndMain()
        {
            InitializeComponent();
            cClient = new CClient();
            cClient.LoginOK += new EventHandler(cOnLoginOk); //Das Event "subscriben"
            cClient.LoginNotOk += new EventHandler(cOnLoginNotOk); //Das Event "subscriben"

        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {

            Application.Current.Dispatcher.Invoke((Action)delegate
         {
             this.Hide();
             WndRegistration wndRegistration = new WndRegistration();
             wndRegistration.ShowDialog();
             this.Show();
         });


        }

        private void btnSender_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            cClient.Connect(tbEmail.Text, tbPassword.Text, false);
        }

        void cOnLoginNotOk(object sender, EventArgs e)
        {
            MessageBox.Show("Es konnte keine Verbindung zum Server hergestellt werden.");
        }

        void cOnLoginOk(object sender, EventArgs e)
        {
            //TODO: Recherchieren "The calling thread must be STA"  

            //TODO: Recherchieren über folgende Aussage
            /* 
            If you call a new window UI statement in an existing thread, it throws an error. 
            Instead of that create a new thread inside the main thread and write the window UI statement in the new child thread.
            */
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                this.Hide();
                WndChat wndChat = new WndChat(cClient);
                wndChat.Show();
            });

        }
    }
}
