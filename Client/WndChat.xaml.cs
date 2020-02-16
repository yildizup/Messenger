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


            this.Closing += ManageClosing; //Wenn der User das Fenster schließen möchte
            lbContactList.ItemsSource = cClient.contactList.listContacts;


        }

        private void CClient_ChatReceived(object sender, CChatContentEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
                       {
                           txtbReceivedMessage.Text = "";
                       });
            DataTable tmp = e.DtChat;
            foreach (DataRow row in tmp.Rows)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                               {
                                   txtbReceivedMessage.Text += String.Format("[{3}] {0}: {1}{2}", row["main_email"], row["message"], Environment.NewLine, row["thetime"]);
                               });
            }

        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            cClient.SendMessage(lbContactList.SelectedItem.ToString(), txtMessage.Text);


            txtbReceivedMessage.Text += String.Format("[{0}]Sie: {1}{2}", DateTime.Now ,txtMessage.Text, Environment.NewLine); //\r\n würde auch für eine neue Zeile reichen
        }


        void cMessageReceived(object sender, CReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
                       {
                           txtbReceivedMessage.Text += String.Format("{0}: {1}{2}", e.From, e.Message, Environment.NewLine); //\r\n würde auch für eine neue Zeile reichen
                       });

        }

        private void ManageClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; //TODO: Recherchieren

            if (MessageBox.Show("Wollen Sie sich wirklich abmelden ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Environment.Exit(0);
        }

        private void lbContactList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cClient.LoadChat(lbContactList.SelectedItem.ToString());
        }
    }
}
