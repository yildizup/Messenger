using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for WndChat.xaml
    /// </summary>
    public partial class WndChat : Window
    {

        public CClient cClient;
        CReceivedEventHandler receivedHandler;

        public WndChat(CClient cClient)
        {
            InitializeComponent();

            this.cClient = cClient;
            receivedHandler = new CReceivedEventHandler(cMessageReceived);//TODO: Recherchieren "Wie übergebe ich mit einem Event Parameter ?"

            this.cClient.MessageReceived += receivedHandler;
            this.cClient.ChatReceived += CClient_ChatReceived;

            lbContactList.ItemsSource = cClient.contactList.listContacts;

            this.Closing += ManageClosing; //Wenn der User das Fenster schließen möchte

            cClient.RefreshContacts += new EventHandler(ReloadContacts);

        }

        private void ReloadContacts(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
                       {
                           lbContactList.ItemsSource = cClient.contactList.listContacts;
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

        }


        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            cClient.SendMessage(lbContactList.SelectedItem.ToString(), txtMessage.Text);
            UserControlMessageSent messagesent = new UserControlMessageSent(txtMessage.Text, DateTime.Now.ToString());
            splChat.Children.Add(messagesent);
        }

        void cMessageReceived(object sender, CReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
                       {
                           UserControlMessageReceived messagereceived = new UserControlMessageReceived(e.Message, e.Date);
                           splChat.Children.Add(messagereceived);
                       });

        }

        private void ManageClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; //TODO: Recherchieren

            if (MessageBox.Show("Wollen Sie sich wirklich abmelden ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                cClient.CloseConn();
                Environment.Exit(0);
            }

        }

        private void lbContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cClient.LoadChat(lbContactList.SelectedItem.ToString());

        }

        private void btnAddContact_Click(object sender, RoutedEventArgs e)
        {
            cClient.AddContact(tbContactName.Text);
        }

    }
}
