using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace BroadCastChatt_Te16
{
    class Program
    {
        private const int ListenPort = 11000;

        static void Main(string[] args)
        {
            //Skapar en tråd som körs samtidigt med huvudprogrammet
            var listenThread = new Thread(Listener);
            listenThread.Start();

            //Skapar en lokal anslutning via udp och IPv4
            Socket socket = new Socket(AddressFamily.InterNetwork, 
                SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;

            IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, ListenPort);

            Thread.Sleep(1000);

            while(true)
            {
                //Användaren skriver meddelande
                Console.Write(">");
                string msg = Console.ReadLine();
                
                //Skicka meddelandet
                byte[] sendbuf = Encoding.UTF8.GetBytes(msg);
                socket.SendTo(sendbuf, ep);

                //Gör en paus
                Thread.Sleep(200);
            }
        }

        static void Listener()
        {
            //Skapar ett objekt som ska lyssna efter meddelanden
            UdpClient listener = new UdpClient(ListenPort);
            try
            {
                while(true)
                {
                    //Skapa objekt som lyssnar efter trafik från valfri ip-adress men via port
                    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, ListenPort);

                    //Tar emot meddelande i en array
                    byte[] bytes = listener.Receive(ref groupEP);

                    //Skriver ut meddelandet
                    Console.WriteLine("Mottaget meddelande från {0} : {1}\n",
                        groupEP.ToString(), Encoding.UTF8.GetString(bytes, 0, bytes.Length));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
