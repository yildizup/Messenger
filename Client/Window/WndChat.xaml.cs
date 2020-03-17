using SharedClass; //TODO: Recherchieren
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for WndChat.xaml
    /// </summary>
    public partial class WndChat : Window
    {

        public CClient cClient;
        CReceivedEventHandler receivedHandler;
        int selectedContact; //index des ausgewählten Kontaktes

        public WndChat(CClient cClient)
        {
            InitializeComponent();

            this.cClient = cClient;
            receivedHandler = new CReceivedEventHandler(cMessageReceived);//TODO: Recherchieren "Wie übergebe ich mit einem Event Parameter ?"

            this.cClient.MessageReceived += receivedHandler;
            this.cClient.ChatReceived += CClient_ChatReceived;

            foreach (User user in cClient.contactList.listContacts)
            {
                UserControlContactItem contact = new UserControlContactItem(user.Email, user.Status, user.NewMessages);
                lvContacts.Items.Add(contact);
            }


            this.Closing += ManageClosing; //Wenn der User das Fenster schließen möchte

            cClient.RefreshContacts += new EventHandler(ReloadContacts);

            YourEmail.Content = cClient.email;

        }

        /// <summary>
        /// Kontaktliste aktualiseren, wenn ein neuer Kontakt hinzugefügt wurde
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadContacts(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
                                 {
                                     lvContacts.Items.Clear();
                                     foreach (User user in cClient.contactList.listContacts)
                                     {
                                         UserControlContactItem contact = new UserControlContactItem(user.Email, user.Status, user.NewMessages);
                                         lvContacts.Items.Add(contact);
                                     }

                                     lvContacts.SelectedIndex = selectedContact;

                                 });
        }

        private void CClient_ChatReceived(object sender, CChatContentEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
                       {
                           splChat.Children.Clear(); //Stackpanel säubern
                       });
            DataTable tmp = e.DtChat;

            foreach (DataRow row in tmp.Rows)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                               {
                                   //TODO: Nach einer anderen Lösungsmöglichkeit recherchieren
                                   if (row["main_email"].ToString() == "Sie")
                                   {
                                       UserControlMessageSent messagesent = new UserControlMessageSent(row["message"].ToString(), row["thetime"].ToString());
                                       splChat.Children.Add(messagesent);
                                   }
                                   else
                                   {
                                       UserControlMessageReceived messagereceived = new UserControlMessageReceived(row["message"].ToString(), row["thetime"].ToString());
                                       splChat.Children.Add(messagereceived);
                                   }
                               });

            }

            // Nach unten scrollen
            Application.Current.Dispatcher.Invoke((Action)delegate
                       {
                           chatViewScroller.ScrollToBottom();
                       });
        }

        private void btnAddContact_Click(object sender, RoutedEventArgs e)
        {
            cClient.AddContact(tbContactName.Text);
        }

        private void lvContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvContacts.SelectedItem != null)
            {
                selectedContact = lvContacts.SelectedIndex; //speichern des zuletzt ausgewählten Kontaktes
                cClient.LoadChat(((UserControlContactItem)lvContacts.SelectedItem).Email); //TODO: Das kann man besser lösen. MVVM anschauen

            }
            else
            {
                splChat.Children.Clear();
            }
        }

        #region Nachrichten senden und empfangen

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (lvContacts.SelectedItem != null) //Nur wenn ein Kontakt ausgewählt ist
            {
                cClient.SendMessage(((UserControlContactItem)lvContacts.SelectedItem).Email, txtMessage.Text);
                UserControlMessageSent messagesent = new UserControlMessageSent(txtMessage.Text, DateTime.Now.ToString());
                splChat.Children.Add(messagesent);


                // Nach unten scrollen
                Application.Current.Dispatcher.Invoke((Action)delegate
                           {
                               chatViewScroller.ScrollToBottom();
                           });
            }
        }


        /// <summary>
        /// neue Nachricht anzeigen 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cMessageReceived(object sender, CReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
                       {
                           // Nachricht wird nur angezeigt, wenn man sich im selben Chat befinden
                           if (lvContacts.SelectedItem != null)
                           {
                               if (e.From == ((UserControlContactItem)lvContacts.SelectedItem).Email)
                               {
                                   UserControlMessageReceived messagereceived = new UserControlMessageReceived(e.Message, e.Date);
                                   splChat.Children.Add(messagereceived);
                               }
                           }

                       });
        }

        #endregion

        private void ManageClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; //TODO: Recherchieren

            if (MessageBox.Show("Wollen Sie sich wirklich abmelden ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                cClient.CloseConn();
                Environment.Exit(0);
            }

        }

        private void tmpWhoIsOnline_Click(object sender, RoutedEventArgs e)
        {
            cClient.WhoIsOnline();
        }

    }
}
