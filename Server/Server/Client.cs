using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Client
    {
        public string Id { get; private set; }
        public NetworkStream Stream { get; private set; }

        private readonly TcpClient _client;
        private readonly Server _server;

        private string _useName;
        

        public Client(TcpClient client, Server server)
        {
            this._client = client;
            this._server = server;

            Id = Guid.NewGuid().ToString();
            server.AddConnection(this);
        }

        public void BeginChat()
        {
            try
            {
                Stream = _client.GetStream();

                string message = GetMessage();
                _useName = message;

                message = $"{_useName} Connected";
                _server.Messaging(message, Id);
                Console.WriteLine(message);

                while(true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", _useName, message);
                        _server.Messaging(message, Id);

                        Console.WriteLine(message);
                    }
                    catch
                    {
                        message = String.Format("{0}: Disconnected", _useName);
                        _server.Messaging(message, Id);

                        Console.WriteLine(message);

                        break;
                    }
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                _server.RemoveConnection(Id);
                Close();
            }
        }

        private string GetMessage()
        {
            var data = new byte[64];
            StringBuilder builder = new();
            do
            {
                int bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
            } 
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        public void Close()
        {
            Stream?.Close();
            _client?.Close();
        }
    }
}
