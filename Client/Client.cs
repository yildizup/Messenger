using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Client
{
    class Client
    {
        Thread tcpThread;
        bool _conn = false;    // Ist der Client schon verbunden ?

        public string Server { get { return "localhost"; } }
        public int Port { get { return 2000; } }

        void SetupConn()  // Verbindung aufbauen
        {
        }
        void CloseConn() // Verbindung beenden
        {
        }

        // Verbindung aufbauen und thread starten
        void connect(string user, string password, bool register)
        {
            if (!_conn)
            {
                _conn = true;

                // Verbindung mit dem Server starten und in einem neuen Thread kommunizieren
                tcpThread = new Thread(new ThreadStart(SetupConn));
                tcpThread.Start();
            }
        }

        public void Disconnect()
        {
            if (_conn) // Wenn der Client verbunden ist, kann man auch wieder die Verbindung schließen
            {
                CloseConn();
            }
        }

    }
}
