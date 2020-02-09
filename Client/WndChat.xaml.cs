using System;
using System.Collections.Generic;
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

        }


        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            cClient.SendMessage(txtTo.Text, txtMessage.Text);
        }


        void cMessageReceived(object sender, CReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
                       {
                           txtbReceivedMessage.Text += String.Format("{0}: {1}{2}", e.From, e.Message, Environment.NewLine); //\r\n würde auch für eine neue Zeile reichen
                       });


        }

    }
}
