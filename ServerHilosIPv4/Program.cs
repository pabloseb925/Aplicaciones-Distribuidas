using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerHilosIPv4
{
    public class Program
    {
        Socket socketEscucha = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket socketCliente;

        public Program()
        {
            IPAddress[] direccionesIP = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress direccionServidor = direccionesIP[0];
            Console.WriteLine("Direcciones IP: ");
            foreach (IPAddress ip in direccionesIP)
            {
                Console.WriteLine(" * {0}", ip);
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (!ip.Equals("127.0.0.1"))
                    {
                        direccionServidor = ip;
                        Console.WriteLine("El servidor está escuchando en la dirección: {0} puerto:8080", ip);
                    }
                }
            }
            IPEndPoint ipServidor = new IPEndPoint(direccionServidor, 8080);
            socketEscucha.Bind(ipServidor);
            Console.WriteLine("El servidor enlazó el socket...");
            Thread hiloEscucha = new Thread(new ThreadStart(Escuchar));
            hiloEscucha.IsBackground = true;
            hiloEscucha.Start();

        }
        public void Escuchar()
        {
            while (true)
            {
                socketEscucha.Listen(-1);
                Console.WriteLine("El servidor entrá en espera de conexiones...");
                socketCliente = socketEscucha.Accept();
                Console.WriteLine("El servidor ha recibido a un cliente...");
                if (socketCliente.Connected)
                {
                    Thread hiloCliente = new Thread(new ThreadStart(Recibir));
                    hiloCliente.IsBackground = true;
                    hiloCliente.Start();
                }
            }

        }
        public void Recibir()
        {
            Socket socketC = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            lock (this)
            {
                socketC = socketCliente;
            }
            Console.WriteLine("Recibiendo datos...");
            while (true)
            {
                int cantidadBytesRecibidos = 0;
                byte[] bytesRecibidos = new byte[2];
                try
                {
                    cantidadBytesRecibidos = socketC.Receive(bytesRecibidos);
                    if (cantidadBytesRecibidos != 0)
                    {

                        Console.WriteLine(Encoding.ASCII.GetString(bytesRecibidos));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
                if (!socketC.Connected)
                    break;
            }
        }

        static void Main(string[] args)
        {
            new Program();
            Console.Read();
        }
    }
}
