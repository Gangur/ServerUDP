using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;

namespace ServerUDP
{
    class Program
    {
        public static IPAddress Address { get; set; }
        public static int Port { get; set; }
        public static Random Random { get; set; }
        public static long MaxValue { get; set; }
        public static long MinValue { get; set; }
        public static ulong Counter { get; set; }

        static void Main(string[] args)
        {
            try
            {
                InitApp();
                SendMessage();
            }
            catch (Exception ex){ Console.WriteLine(ex.Message);}
        }

        private static void SendMessage()
        {
            UdpClient client = new UdpClient(); 
            IPEndPoint endPoint = new IPEndPoint(Address, Port);
            try
            {
                while (true)
                {
                    Counter++;
                    byte[] data = Encoding.ASCII.GetBytes($"{LongRandom()},{Counter}");
                    Array.Resize(ref data, data.Length + 1);
                    data[data.Length - 1] = GetControlByte(data);
                    client.Send(data, data.Length, endPoint);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally { client.Close(); }
        }

        private static long LongRandom()
        {
            long result = Random.Next((Int32)(MinValue >> 32), (Int32)(MaxValue >> 32));
            result = (result << 32);
            result = result | (long)Random.Next((Int32)MinValue, (Int32)MaxValue);
            return result;
        }

        private static byte GetControlByte(byte[] data)
        {
            byte b = data[0];
            for (int i = 1; i < data.Length; i++)
                b ^= data[i];
            return b;
        }

        private static void InitApp()
        {
            var map = new ExeConfigurationFileMap { ExeConfigFilename = "server.config" };
            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            string group = config.AppSettings.Settings["m_group"].Value;
            Address = IPAddress.Parse(group);
            Port = Convert.ToInt32(config.AppSettings.Settings["port"].Value);
            MaxValue = Convert.ToInt64(config.AppSettings.Settings["max_value"].Value);
            MinValue = Convert.ToInt64(config.AppSettings.Settings["min_value"].Value);
            Random = new Random();
            Console.WriteLine("Server started...");
        }
    }
}
