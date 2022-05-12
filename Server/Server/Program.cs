using System;

namespace Server
{
    internal class Program
    {
        private static Server _server;
        private static Thread _listedThread;

        static void Main(string[] args)
        {
            try
            {
                _server = new();
                _listedThread = new Thread(new ThreadStart(_server.Listen));
                _listedThread.Start();
            }
            catch (Exception exception)
            {
                _server.Disconnect();
                Console.WriteLine(exception.Message);
            }
        }
    }
}