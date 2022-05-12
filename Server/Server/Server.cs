using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Server
    {
        private static TcpListener _tcpListener;
        private readonly List<Client> _clients = new();


        public void AddConnection(Client client)
        {
            _clients.Add(client);
        }

        public void RemoveConnection(string id)
        {
            var currentClien = _clients.FirstOrDefault(x => x.Id == id);

            if (currentClien != null)
                _clients.Remove(currentClien);

        }

        public void Listen()
        {
            try
            {
                int port = 8888;
                _tcpListener = new TcpListener(IPAddress.Any, port);
                _tcpListener.Start();

                Console.WriteLine("The server is running. Waiting for connections...");

                while(true)
                {
                    var tcpClient = _tcpListener.AcceptTcpClient();

                    var client = new Client(tcpClient, this);
                    var clientThread = new Thread(new ThreadStart(client.BeginChat));
                    clientThread.Start();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Disconnect();
            }
        }

        public void Messaging(string message, string id)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);

            for (int i = 0; i < _clients.Count; i++)
            {
                var current = _clients[i];

                if(current.Id != id)
                {
                    current.Stream.Write(data, 0, data.Length);
                }
            }
        }

        public void Disconnect()
        {
            _tcpListener.Stop();

            foreach (var client in _clients)
            {
                client.Close();
            }

            Environment.Exit(0);
        }
    }
}
