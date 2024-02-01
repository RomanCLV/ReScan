using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UDPSenderCS
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            bool _isRunning = true;
            ConsoleKey consoleKey;
            while (_isRunning)
            {
                Console.WriteLine("Choice:");
                Console.WriteLine("[0] Exit");
                Console.WriteLine("[1] Test if a port is used");
                Console.WriteLine("[2] Write a message to a port");

                do
                {
                    consoleKey = Console.ReadKey(true).Key;
                } while (consoleKey != ConsoleKey.NumPad0 && consoleKey != ConsoleKey.NumPad1 && consoleKey != ConsoleKey.NumPad2);
                Console.WriteLine();

                if (consoleKey == ConsoleKey.NumPad0)
                {
                    _isRunning = false;
                }
                else if (consoleKey == ConsoleKey.NumPad1)
                {
                    ushort port = SelectPort();

                    if (IsPortInUse(port, ProtocolType.Tcp))
                    {
                        Console.WriteLine($"Port {port} is used in a TCP connection.");
                    }
                    else if (IsPortInUse(port, ProtocolType.Udp))
                    {
                        Console.WriteLine($"Port {port} is used in a UDP connection.");
                    }
                    else
                    {
                        Console.WriteLine($"Port {port} seems to be free.");
                    }
                }
                else if (consoleKey == ConsoleKey.NumPad2)
                {
                    ushort port = SelectPort();
                    Console.Write("Message: ");
                    string message = Console.ReadLine();
                    SendUDP(message, "127.0.0.1", port);
                }

                if (consoleKey != ConsoleKey.NumPad0)
                {
                    Console.ReadKey(true);
                }

                Console.Clear();
            }
        }

        private static ushort SelectPort()
        {
            Console.Write("Port: ");
            string portStr;
            ushort port;
            do
            {
                portStr = Console.ReadLine();
            } while (!ushort.TryParse(portStr, out port));
            return port;
        }

        private static bool IsPortInUse(int port, ProtocolType protocolType)
        {
            bool isPortInUse = false;
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            switch (protocolType)
            {
                case ProtocolType.Tcp:
                    IPEndPoint[] tcpListeners = ipGlobalProperties.GetActiveTcpListeners();
                    foreach (IPEndPoint endpoint in tcpListeners)
                    {
                        if (endpoint.Port == port)
                        {
                            isPortInUse = true;
                            break;
                        }
                    }
                    break;

                case ProtocolType.Udp:
                    IPEndPoint[] udpListeners = ipGlobalProperties.GetActiveUdpListeners();
                    foreach (IPEndPoint endpoint in udpListeners)
                    {
                        if (endpoint.Port == port)
                        {
                            isPortInUse = true;
                            break;
                        }
                    }
                    break;
            }

            return isPortInUse;
        }

        private static void SendUDP(string message, string adresseIP, int port)
        {
            UdpClient udpClient = new UdpClient();

            try
            {
                // Convertir le message en tableau d'octets
                byte[] data = Encoding.UTF8.GetBytes(message);

                // Envoyer le message à l'adresse et au port spécifiés
                udpClient.Send(data, data.Length, adresseIP, port);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().Name}: {e.Message}");
            }
            finally
            {
                udpClient.Close();
            }
        }
    }
}
