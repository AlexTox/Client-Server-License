using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Клиент_Тест
{

    class Program
    {
        static string command;
        public static byte[] MessageBytes = new byte[1024];
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            while (true) {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("/n Введи команду:");
                command = Console.ReadLine();
                switch (command)
                {
                    case "commands":
                        try
                        {
                            socket.Connect("127.0.0.1", 1024);
                            socket.Send(Encoding.ASCII.GetBytes(command));
                            socket.Close();
                        }
                        catch (Exception er)
                        {
                            Console.WriteLine("Произошла ошибка: {0}", er.ToString());
                        }
                        break;
                    default:
                        Console.WriteLine("What?");
                        break;
                }
                Thread.Sleep(1000);

            }
        }
    }
}
