using Client.Models;
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

        int[] selectedContact; //index des zuletzt und des neu ausgewählten Kontakts

        public WndChat(CClient cClient)
        {
            InitializeComponent();
            selectedContact = new int[2];
            selectedContact[1] = 99; // zur Initialisierung müssen zwei unterschiedliche Werte vorhanden sein.


            foreach (User user in cClient.contactList.listContacts)
            {
                UserControlContactItem contact = new UserControlContactItem(user.Email, user.Status, user.NewMessages, user.FsName);
                lvContacts.Items.Add(contact);
            }

            #region Kontrollelemente zum Senden einer Nachricht verstecken
            btnSendMessage.Visibility = Visibility.Hidden;
            txtMessage.Visibility = Visibility.Hidden;
            splChat.Children.Add(new UserControlMessageSent("Bitte wählen Sie einen Chat aus, um eine Unterhaltung zu beginnen", " "));
            #endregion

            #region EventListener
            this.Closing += ManageClosing; //Wenn der User das Fenster schließen möchte
            cClient.RefreshContacts += new EventHandler(ReloadContacts);
            this.cClient = cClient;
            receivedHandler = new CReceivedEventHandler(cMessageReceived);//TODO: Recherchieren "Wie übergebe ich mit einem Event Parameter ?"
            this.cClient.MessageReceived += receivedHandler;
            this.cClient.ChatReceived += CClient_ChatReceived;
            this.cClient.AddContactWrong += CClient_AddContactWrong;
            #endregion

        }

        private void CClient_AddContactWrong(object sender, EventArgs e)
        {
            MessageBox.Show("Der Kontakt kann nicht hinzugefügt werden. Bitte kontrollieren Sie die E-Mail Adresse.");
        }

        /// <summary>
        /// Kontaktliste aktualiseren, wenn ein neuer Kontakt hinzugefügt wurde
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadContacts(object sender, EventArgs e)
        {
            // Die neuen Kontaktinformationen werden mit den alten verglichen. Falls Veränderung vorhanden sind, wird das jeweilige item aktualisert.
            Application.Current.Dispatcher.Invoke((Action)delegate
                                 {
                                     //Wenn die Anzahl der Kontakte sich nicht verändert hat.
                                     if (lvContacts.Items.Count == cClient.contactList.listContacts.Count)
                                     {

                                         foreach (UserControlContactItem item in lvContacts.Items)
                                         {
                                             foreach (User user in cClient.contactList.listContacts)
                                             {
                                                 if (item.Email == user.Email)
                                                 {
                                                     item.NewMessages = user.NewMessages;
                                                     item.Status = user.Status;

                                                 }

                                             }
                                         }
                                     }
                                     // Wenn ein neuer Kontakt hinzugefügt wurde
                                     else
                                     {
                                         // der neue Kontakt wird in die "listview" hinzugefügt
                                         User tmpUser = cClient.contactList.listContacts[cClient.contactList.listContacts.Count - 1];
                                         lvContacts.Items.Add(new UserControlContactItem(tmpUser.Email, tmpUser.Status, tmpUser.NewMessages, tmpUser.FsName));
                                     }

                                     // zuletzt ausgewählter Kontakt soll nach der Aktualisierung ausgewählt sein
                                     lvContacts.SelectedIndex = selectedContact[1];

                                 });
        }

        private void CClient_ChatReceived(object sender, CChatContentEventArgs e)
        {

            // Chat wird nur komplett aktualisert, wenn man einen neuen Kontakt auswählt
            if (selectedContact[0] != selectedContact[1])
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
        }

        private void btnAddContact_Click(object sender, RoutedEventArgs e)
        {
            WndAddContact wndAddContact = new WndAddContact();
            wndAddContact.ShowDialog();
            cClient.AddContact(wndAddContact.Email);

        }

        private void lvContacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Um die versteckten Steuerelemente wieder anzuzeigen. (Wird nur am Anfang benötigt)
            if (btnSendMessage.Visibility == Visibility.Hidden && txtMessage.Visibility == Visibility.Hidden)
            {
                btnSendMessage.Visibility = Visibility.Visible;
                txtMessage.Visibility = Visibility.Visible;
                splChat.Children.Clear();
            }

            if (lvContacts.SelectedItem != null)
            {
                // Alle Elemente im Array werden 1 nach links "geshiftet". Wird benötigt, um zu testen, ob ein neuer Kontakt ausgewählt wurde
                selectedContact[0] = selectedContact[1];
                selectedContact[1] = lvContacts.SelectedIndex; //speichern des zuletzt ausgewählten Kontaktes
                cClient.LoadChat(((UserControlContactItem)lvContacts.SelectedItem).Email); //TODO: Das kann man besser lösen. MVVM anschauen

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
                           // Nachricht wird nur angezeigt, wenn man sich im selben Chat befindet
                           if (lvContacts.SelectedItem != null)
                           {
                               if (e.From == ((UserControlContactItem)lvContacts.SelectedItem).Email)
                               {
                                   UserControlMessageReceived messagereceived = new UserControlMessageReceived(e.Message, e.Date);
                                   splChat.Children.Add(messagereceived);
                                   cClient.MessagesRead(((UserControlContactItem)lvContacts.SelectedItem).Email);
                                   // TODO: LoadChat aktualisert den ganzen Chat. Benötigt wird eine Methode, die die Nachrichten als gelesen markiert. (CHECK)
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

    }
}
