using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Management;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Клиент_Тест
{
    class Program
    {
        static string Command = "1";
        static string UserName = "1";
        static string SecretWord = "1";
        static string HWID = "1";
        static string TELEGRAM = "1";
        static string EMAIL = "1";
        static string VolumeSerials = "1";
        static string CPUIDs = "1";

        static byte[] buffer = new byte[1024];
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static MemoryStream stream = new MemoryStream(buffer);
        static BinaryWriter writer = new BinaryWriter(stream);
        static void Main(string[] args)
        {
            byte[] commandBytes = new byte[1024];
            byte[] answerBytes = new byte[1024];
            writer.Write(Command);
            writer.Write(UserName);
            writer.Write(SecretWord);
            writer.Write(HWID);
            writer.Write(TELEGRAM);
            writer.Write(VolumeSerials);
            writer.Write(CPUIDs);
            socket.Connect("127.0.0.1", 904);
            socket.Send(buffer);
            socket.Receive(answerBytes);
            string answer = Encoding.ASCII.GetString(answerBytes);
            Console.WriteLine(answer);
            Console.ReadLine();
        }
        private string getUniqueID(string drive)
        {
            if (drive == string.Empty)
            {
                //Find first drive
                foreach (DriveInfo compDrive in DriveInfo.GetDrives())
                {
                    if (compDrive.IsReady)
                    {
                        drive = compDrive.RootDirectory.ToString();
                        break;
                    }
                }
            }

            if (drive.EndsWith(":\\"))
            {
                //C:\ -> C
                drive = drive.Substring(0, drive.Length - 2);
            }
            VolumeSerials = getVolumeSerial(drive);
            CPUIDs = getCPUID();
            string volumeSerial = getVolumeSerial(drive);
            string cpuID = getCPUID();

            //Mix them up and remove some useless 0's
            HWID = cpuID.Substring(13) + cpuID.Substring(1, 4) + volumeSerial + cpuID.Substring(4, 4);
            return cpuID.Substring(13) + cpuID.Substring(1, 4) + volumeSerial + cpuID.Substring(4, 4);
        }

        private string getVolumeSerial(string drive)
        {
            ManagementObject disk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            disk.Get();

            string volumeSerial = disk["VolumeSerialNumber"].ToString();
            disk.Dispose();

            return volumeSerial;
        }

        private string getCPUID()
        {
            string cpuInfo = "";
            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();

            foreach (ManagementObject managObj in managCollec)
            {
                if (cpuInfo == "")
                {
                    //Get only the first CPU's ID
                    cpuInfo = managObj.Properties["processorID"].Value.ToString();
                    break;
                }
            }

            return cpuInfo;
        }
    }
}
