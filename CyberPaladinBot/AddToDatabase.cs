using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace CyberPaladinBot
{
    class AddToDatabase//класс для логирования данных
    {
        public static void AddToError(string errrorText, string User) 
        {
            string con = "server=localhost;user=root;password=1234;database=diplom;port=3306";
            string CommandText = "Insert Into errors(ErrorText,User,Date) Values('"+errrorText+"','"+User+"','"+DateTime.Now.ToString()+"')";
            MySqlConnection myCon = new MySqlConnection(con);
            MySqlCommand myCom = new MySqlCommand(CommandText,myCon);
            myCon.Open();
            myCom.ExecuteNonQuery();
            return;
        }

        public static void AddToCommands(string commandText, string User)
        {
            string con = "server=localhost;user=root;password=1234;database=diplom;port=3306";
            string CommandText = "Insert Into commands(CommandText,User,Date) Values('"+commandText+"','"+User+"','"+DateTime.Now.ToString()+"')";
            MySqlConnection myCon = new MySqlConnection(con);
            MySqlCommand myCom = new MySqlCommand(CommandText,myCon);
            myCon.Open();
            myCom.ExecuteNonQuery();
            return;
        }
        public static void AddToMessages(string MessageText, string User)
        {
            string con = "server=localhost;user=root;password=1234;database=diplom;port=3306";
            string CommandText = "Insert Into messages(MessageText,User,Date) Values('"+MessageText+"','"+User+"','"+DateTime.Now.ToString()+"')";
            MySqlConnection myCon = new MySqlConnection(con);
            MySqlCommand myCom = new MySqlCommand(CommandText,myCon);
            myCon.Open();
            myCom.ExecuteNonQuery();
            return;
        }

        public static void AddToBot_Activicy(string activicyMessage)
        {
            try
            {
                string con = "server=localhost;user=root;password=1234;database=diplom;port=3306";
                string CommandText = "Insert Into bot_activicy(ActivicyMessage,Date) Values('"+activicyMessage+"','"+DateTime.Now.ToString()+"')";
                MySqlConnection myCon = new MySqlConnection(con);
                MySqlCommand myCom = new MySqlCommand(CommandText, myCon);
                myCon.Open();
                myCom.ExecuteNonQuery();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void AddToJokes(string JokeText)
        {
            try
            {
                string con = "server=localhost;user=root;password=1234;database=diplom;port=3306";
                string CommandText = "Insert Into jokes(JokeText) Values('" + JokeText + "')";
                MySqlConnection myCon = new MySqlConnection(con);
                MySqlCommand myCom = new MySqlCommand(CommandText, myCon);
                myCon.Open();
                myCom.ExecuteNonQuery();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}