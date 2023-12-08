using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace practice1
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
      
        public Authorization()
        {
            InitializeComponent();
            database.Connect("localhost", "5432", "postgres", "1", "sad");
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Log = logtb.Text.Trim();
            string Pass = passwordBox1.Password.Trim();
            if (Log.Length <= 0 || Pass.Length <= 0)
            {
                database.message = ("Заполните поля");
                database.OpenNotification_Click();
            }
            else
            {
                NpgsqlCommand command = database.GetCommand(
                "SELECT \"Accounts\".\"login\",  \"Accounts\".\"name\",  \"Accounts\".\"surname\", password \"role\", \"Role\".\"name\", \"group\" , \"number\", \"avg_age\"" +
                "FROM \"Accounts\", \"Role\", \"Group\"" +
                "WHERE \"Accounts\".\"role\" = \"Role\".\"id\" AND  \"Accounts\".\"login\"= @login AND \"Group\".\"id\" = \"Accounts\".\"group\" AND \"Accounts\".password = @password ");   
                command.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, Pass);
                command.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, Log);
                NpgsqlDataReader result = command.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    string role = result.GetString(4);
                    database.name_worker = result.GetString(1);
                    database.surname_worker = result.GetString(2);
                    database.id_group = result.GetInt32(5);
                    database.number_group = result.GetInt32(6);
                    database.name_group = result.GetString(7);
                    switch (role)
                    {
                        case "worker":

                            result.Close();
                            //sp.Visibility = Visibility.Hidden;
                            database.Kids.Clear();
                            database.types_Absents.Clear();
                            frame.Navigate(new Work());
                            break;
                        case "manager":
                            result.Close();
                            sp.Visibility = Visibility.Hidden;
                            frame.Navigate(new PageForzav());
                            break;
                    }
                }
                else
                {
                    database.message = ("неверный логин или пароль");
                    database.OpenNotification_Click();
                    //MessageBox.Show("неверный логин или пароль");
                }
                result.Close();
            }
        }
     
    }
}
