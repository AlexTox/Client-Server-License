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
    public class CryptService
    {
        private string UserID = Program.InfoUser.UserID;
        private string UserName = Program.InfoUser.UserName;
        private string SecretWord = Program.InfoUser.SecretWord;
        private string HWID = Program.HWIDTEXT;
        private string ThreadCount = Program.InfoUser.ThreadCount;
        private string Telegram = Program.InfoUser.Telegram;
        private string Email = Program.InfoUser.Email;

        public void CryptKey()
        {
            string FirstStep = UserID + UserName;
            string SecondStep = SecretWord + Telegram;
            byte[] ThirdStepByte = new byte[10024];
            ThirdStepByte = Encoding.UTF8.GetBytes(FirstStep);
            byte[] FourStepByte = new byte[10024];
            FourStepByte = Encoding.UTF8.GetBytes(SecondStep);
            string ThirdStep = Convert.ToBase64String(FourStepByte);
            string Complete = (ThirdStepByte + "@" + FourStepByte + "/@" + Telegram);
            string GeneratedKey = Complete;
        }


    }


    }