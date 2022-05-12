using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Client : MonoBehaviour
    {
        [SerializeField] private Text _chatText;
        [SerializeField] private InputField _inputMessage;

        private readonly string _host = "192.168.0.12";
        private readonly int _port = 8888;

        private string _userName;
        
        private TcpClient _client;
        private NetworkStream _stream;


        public void Init(string userName)
        {
            this._userName = userName;
            _client = new();

            try
            {
                _client.Connect(_host, _port);
                _stream = _client.GetStream();

                string message = _userName;
                var data = Encoding.Unicode.GetBytes(message);
                _stream.Write(data, 0, data.Length);

                Thread receiveThread = new(new ThreadStart(ReceiveMessage));
                receiveThread.Start();

            }
            catch (System.Exception exception)
            {
                _chatText.text = exception.Message;
            }
        }

        public void SendMessage()
        {
            var message = _inputMessage.text;
            _chatText.text += $"You: {message} \n";

            var data = Encoding.Unicode.GetBytes(message);
            _stream.Write(data, 0, data.Length);

            _inputMessage.text = System.String.Empty;
        }

        private void ReceiveMessage()
        {
            _ = ReceiveMessageAsync();
        }

        private async Task ReceiveMessageAsync()
        {
            while(true)
            {
                await Task.Delay(300);

                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new();
                    int bytes = 0;

                    do
                    {
                        bytes = _stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (_stream.DataAvailable);

                    string message = builder.ToString();
                    _chatText.text += $"{message} \n";

                }
                catch
                {
                    _chatText.text = "Connect error :(";
                    Disconnect();
                }
            }
        }

        private void Disconnect()
        {
            _stream?.Close();
            _client?.Close();

            System.Environment.Exit(0);
        }
    }
}