using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Linq;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using MySql.Data.MySqlClient;
using Сервер_Лицензирования;

namespace Сервер_Лицензирования
{
    class Program : CryptService
    {
        public static BackgroundWorker OwnerConsole = new BackgroundWorker();
        public static string HWIDTEXT { get; set; }
        #region MySQL Information
        public static string HostMySQL = "127.0.0.1"; // IP
        public static string DataBaseMySQL = "licenseserver"; // DB MySQL
        public static string UserMySQL = "root"; // UserName MySQL
        public static string PasswordMySQL = "root"; // Password MySQL
        public static int PortMySQL = 3306; // Port MySQL
        #endregion
        static public string command; // Команды от клиента
        public static void ReceiveCallback(IAsyncResult AsyncCall) // Ассинхронный метод
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            #region  Переменные_в_Байтах
            Byte[] message = encoding.GetBytes("Я занят");
            byte[] buffer = new byte[1024]; // Буффер получения информации с сервера
            #endregion
            #region Переменные
            string HWID;
            bool HwidFound = false;
            #endregion
            Socket listener = (Socket)AsyncCall.AsyncState; // Прослушка порта
            Socket client = listener.EndAccept(AsyncCall); //
            Console.WriteLine("Принято соединение от: {0}", client.RemoteEndPoint);
            MemoryStream stream = new MemoryStream(buffer);
            BinaryReader reader = new BinaryReader(stream);
            client.Receive(buffer); // Получение информации от клиента
            // Запись информации в различные переменные
            command = reader.ReadString(); 
            string UserName = reader.ReadString();
            string secretword = reader.ReadString();
            HWID = reader.ReadString();
            string Telegram = reader.ReadString();
            string VolumeSerial = reader.ReadString();
            string CPUID = reader.ReadString();
            //
            Console.WriteLine("Данные с клиента {0}", client.RemoteEndPoint + " получены. " + "|" + " Telegram Login: " + Telegram);
            try
            {
                string myConnectionString = "Database=" + DataBaseMySQL + ";Data Source=" + HostMySQL + ";User Id=" +
                                            UserMySQL +
                                            ";Password=" + PasswordMySQL + ";SslMode=none;charset=utf8"; // Строка подключения к MySQL
                Console.WriteLine("Login Iniatilization");
                string SQLCommand = String.Format(
                         "SELECT 'licenseserver' AS `Database`, 'allowed' AS `Table`, COUNT(*) AS `Found rows` FROM `licenseserver`.`allowed` WHERE CONVERT(LOWER(`HWID`) USING utf8mb4) LIKE " + HWID + ";",
                         HWID); // Команда MySQL
                MySqlConnection myConnection = new MySqlConnection(myConnectionString);
                myConnection.Open(); // Подключение
                MySqlCommand cmd = new MySqlCommand(SQLCommand, myConnection);
                MySqlCommand command = new MySqlCommand(SQLCommand);
                MySqlDataReader Read = cmd.ExecuteReader();
                while (Read.Read())
                {
                    string messageServer;
                    int HWIDCount = Read.GetInt32(2);
                    if (HWIDCount >= 1) // Проверка найден ли HWID в БД
                    {
                        Console.WriteLine("HWID For Client {0} ", client.RemoteEndPoint + " Founded");
                        Console.WriteLine("Count:" + " " + Read.GetInt32(2));
                        HwidFound = true;
                        messageServer = "HwidCorrect";
                        byte[] messageBytes = Encoding.ASCII.GetBytes(messageServer);
                        client.Send(messageBytes);
                        SearchInDB();
                    }
                    else
                    {
                        HwidFound = false;
                        messageServer = "HwidIncorrect";
                        byte[] messageBytes = Encoding.ASCII.GetBytes(messageServer);
                        client.Send(messageBytes);
                        Console.WriteLine("HWID Incorrect, MayBe it's Wrong?");
                    }
                }
                myConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("Закрытие соединения c клиентом: {0}", client.RemoteEndPoint);
            client.Close();

            // После того как завершили соединение, говорим ОС что мы готовы принять новое
            listener.BeginAccept(new AsyncCallback(ReceiveCallback), listener);
             }
        static void Main(string[] args)
        {
            Console.WriteLine("Запуск прослушивания для консоли администратора.");
            try
            {

                IPAddress localAddress = IPAddress.Parse("127.0.0.1");

                Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Any, 904);

                listenSocket.Bind(ipEndpoint);

                listenSocket.Listen(10);
                listenSocket.BeginAccept(new AsyncCallback(ReceiveCallback), listenSocket);
                Console.WriteLine("Ожидание соединения… {0}", listenSocket.LocalEndPoint);

                while (true)
                {

                    Console.WriteLine("Сервер запущен");
                    Thread.Sleep(5000);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Произошла ошибка: {0}", e.ToString());
            }

        }

        #region Server Commands

        public static void ServerCommandS()
        {
            switch (command)
            {
                case "Login":
                    Console.WriteLine("Login Iniatilization");
                    // SearchInDB();
                    break;
                case "help":
                    Console.WriteLine("Help command Detected");
                    break;
            }

        }
#endregion

 

        #region Переменные о юзере

        public class InfoUser : CryptService
        {
            public static string UserID { get; set; }
            public static string UserName { get; set; }
            public static string SecretWord { get; set; }
            public static string ThreadCount { get; set; }
            public static string MotherBoard { get; set; }
            public static string Telegram { get; set; }
            public static string Email { get; set; }
        }

        #endregion
        static void SearchInDB()
               {
            string myConnectionString = "Database=" + DataBaseMySQL + ";Data Source=" + HostMySQL + ";User Id=" +
                                        UserMySQL +
                                        ";Password=" + PasswordMySQL + ";SslMode=none;charset=utf8"; // Строка подключения к MySQL
                   Console.WriteLine("Login Iniatilization");
                   string SQLCommand = "SELECT * from allowed Where (allowed.HWID = " + HWIDTEXT + ")"; // Команда MySQL
                   MySqlConnection myConnection = new MySqlConnection(myConnectionString);
                   myConnection.Open(); // Подключение
                   MySqlCommand cmd = new MySqlCommand(SQLCommand, myConnection);
                   MySqlDataReader Read = cmd.ExecuteReader();
                   while (Read.Read())
                   {
                       InfoUser.UserID = Read.GetString(0);
                       InfoUser.UserName = Read.GetString(1);
                       InfoUser.SecretWord = Read.GetString(2);
                       HWIDTEXT = Read.GetString(3);
                       InfoUser.ThreadCount = Read.GetString(4);
                       InfoUser.Telegram = Read.GetString(8);
                       InfoUser.Email = Read.GetString(9);
                       Console.WriteLine(Read.GetString(0) + " " + Read.GetString(1) + " " + Read.GetString(2));
                   }

               }
    }
}

