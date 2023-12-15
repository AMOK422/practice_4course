using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace practice1
{
    public class database
    {
        public static int id_kid;
        public static int counter;
        public static int id_group;
        public static DateTime? date;
        public static string seltype;
        public static int selectypeid;
        public static int number_group;
        public static string name_group;
        public static string name_worker;
        public static string surname_worker;
        public static int id_absent;
        public static string message;
        public static int id_kiid = 9;

        public static NpgsqlConnection Connection;
        public static ObservableCollection<kids> Kids { get; set; } = new ObservableCollection<kids>();
        public static ObservableCollection <parents> parents { get; set; } =  new ObservableCollection<parents> ();
        public static ObservableCollection <absent> absents { get; set; } = new ObservableCollection<absent> ();
        public static ObservableCollection <Event> events { get; set; } = new ObservableCollection<Event> ();
        public static ObservableCollection <group> groups { get; set; } = new ObservableCollection<group> ();
        public static ObservableCollection <type_absent> types_Absents { get; set; } = new ObservableCollection<type_absent>();
        
        public static void Connect(string host, string port, string user, string password, string database)
        {
            if (Connection == null)
            {
                string db = string.Format("Server={0}; Port={1}; User Id={2}; Password={3}; Database={4}", host, port, user, password, database);
                Connection = new NpgsqlConnection(db);
                Connection.Open();
            }
        }
        public static NpgsqlCommand GetCommand(string sql)
        {
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = Connection;
            command.CommandText = sql;
            return command;
        }
        
        public static void Lastman_forinsert()
        {
            NpgsqlCommand command1 = database.GetCommand("SELECT \"Absents\".\"id\" , \"kids\".\"name\",\"kids\".\"surname\",\"kids\".\"patronymic\", \"Absents\".\"date\", \"Type_absents\".\"name\" " +
             "FROM \"kids\", \"Absents\", \"Type_absents\" " +
             "WHERE @group = \"group\" AND  \"Absents\".\"type\" = \"Type_absents\".\"id\" AND \"Absents\".\"id_kid\" = \"kids\".\"id\"" +
             "  ORDER BY \"Absents\".\"id\"  DESC LIMIT 1");
            command1.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, database.id_group);
            NpgsqlDataReader aaa = command1.ExecuteReader();
            if (aaa.HasRows)
            {
                while (aaa.Read())
                {
                    absent absent = new absent();
                    absent.id = aaa.GetInt32(0);
                    absent.name = aaa.GetString(1);
                    absent.surname = aaa.GetString(2);
                    absent.patronymic = aaa.GetString(3);
                    absent.date = aaa.GetDateTime(4);
                    string a = absent.date.ToString();
                    absent.date1 = DateTime.Parse(a).ToShortDateString();
                    absent.type_absent = aaa.GetString(5);
                    database.absents.Add(absent);
                }
                aaa.Close();
            }
        }
        public static void Lastman_forupdate()
        {
            NpgsqlCommand command1 = database.GetCommand("SELECT \"Absents\".\"id\" , \"kids\".\"name\",\"kids\".\"surname\",\"kids\".\"patronymic\", \"Absents\".\"date\", \"Type_absents\".\"name\" " +
            "FROM \"kids\", \"Absents\", \"Type_absents\" " +
            "WHERE @group = \"group\" AND  \"Absents\".\"type\" = \"Type_absents\".\"id\" AND \"Absents\".\"id_kid\" = \"kids\".\"id\" AND \"Absents\".\"id\" = @id_absent " +
            "  ORDER BY \"Absents\".\"id\"  DESC LIMIT 1");
            command1.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, database.id_group);
            command1.Parameters.AddWithValue("@id_absent", NpgsqlDbType.Integer, database.id_absent);
            NpgsqlDataReader aaa = command1.ExecuteReader();
            if (aaa.HasRows)
            {
                while (aaa.Read())
                {
                    absent absent = new absent();
                    absent.id = aaa.GetInt32(0);
                    absent.name = aaa.GetString(1);
                    absent.surname = aaa.GetString(2);
                    absent.patronymic = aaa.GetString(3);
                    absent.date = aaa.GetDateTime(4);
                    string a = absent.date.ToString();
                    absent.date1 = DateTime.Parse(a).ToShortDateString();
                    absent.type_absent = aaa.GetString(5);
                    database.absents.Add(absent);
                }
                aaa.Close();
            }
        }
        public static void OpenNotification_Click()
        {
            var notificationWindow = new Message();

            notificationWindow.tbmes.Text = database.message;
            notificationWindow.Show();
        }
        public static void loadkidsupdate()
        {
            NpgsqlCommand command = database.GetCommand("SELECT \"kids\".\"id\",  \"kids\".\"name\", \"surname\", \"patronymic\" , \"group\", \"Group\".\"number\" ,\"Group\".\"avg_age\" " +
           "FROM \"kids\" , \"Group\" " +
           "WHERE  \"kids\".\"id\" = @id AND \"kids\".\"group\" = \"Group\".\"id\" " +
           " ORDER BY  \"kids\".\"id\"  DESC LIMIT 1");
            command.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, PageForzav.id_kid);
            NpgsqlDataReader aaa = command.ExecuteReader();
            if (aaa.HasRows)
            {
                while (aaa.Read())
                {
                    kids kids = new kids();
                    kids.id = aaa.GetInt32(0);
                    kids.name = aaa.GetString(1);
                    kids.surname = aaa.GetString(2);
                    kids.patronymic = aaa.GetString(3);
                    kids.group = aaa.GetInt32(4);
                    kids.namegroup = aaa.GetInt32(5);
                    kids.avg_age_group = aaa.GetString(6);
                    database.Kids.Add(kids);
                }
                aaa.Close();
            }
        }
        public static void loadkidinsert()
        {
            NpgsqlCommand command = database.GetCommand("SELECT \"kids\".\"id\",  \"kids\".\"name\", \"surname\", \"patronymic\" , \"group\", \"Group\".\"number\" ,\"Group\".\"avg_age\" " +
           "FROM \"kids\" , \"Group\" " +
           "WHERE  \"kids\".\"id\" = @id AND \"kids\".\"group\" = \"Group\".\"id\" " +
           " ORDER BY  \"kids\".\"id\"  DESC LIMIT 1");
            command.Parameters.AddWithValue("@id", NpgsqlDbType.Integer,id_kiid);
            NpgsqlDataReader aaa = command.ExecuteReader();
            if (aaa.HasRows)
            {
                while (aaa.Read())
                {
                    kids kids = new kids();
                    kids.id = aaa.GetInt32(0);
                    
                    kids.name = aaa.GetString(1);
                    kids.surname = aaa.GetString(2);
                    kids.patronymic = aaa.GetString(3);
                    kids.group = aaa.GetInt32(4);
                    kids.namegroup = aaa.GetInt32(5);
                    kids.avg_age_group = aaa.GetString(6);
                    database.Kids.Add(kids);
                   
                }
                aaa.Close();
            }
        }

        public static void loadkidid()
        {
            NpgsqlCommand command = database.GetCommand("SELECT \"kids\".\"id\",  \"kids\".\"name\", \"surname\", \"patronymic\" , \"group\", \"Group\".\"number\" ,\"Group\".\"avg_age\" " +
           "FROM \"kids\" , \"Group\" " +
           "WHERE  \"kids\".\"id\" = @id AND \"kids\".\"group\" = \"Group\".\"id\" " +
           " ORDER BY  \"kids\".\"id\"  DESC LIMIT 1");
            command.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id_kiid);
            NpgsqlDataReader aaa = command.ExecuteReader();
            if (aaa.HasRows)
            {
                while (aaa.Read())
                {
                    id_kiid = aaa.GetInt32(0);
                }
                aaa.Close();
            }
            aaa.Close() ;
        }
    }
}

