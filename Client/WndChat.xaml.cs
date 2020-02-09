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
        CRece

        public WndChat(CClient cClient)
        {
            InitializeComponent();
            this.cClient = cClient;
            this.cClient.MessageReceived += new EventHandler(cOnMessageReceived); //TODO: Recherchieren "Wie übergebe ich mit einem Event Parameter ?"
            receivedHa
        }


        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            cClient.SendMessage(txtTo.Text, txtMessage.Text);
        }


        void cOnMessageReceived(object sender, EventArgs e)
        {



        }






    }
}
