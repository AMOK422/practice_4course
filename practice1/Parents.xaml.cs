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
    /// Логика взаимодействия для Parents.xaml
    /// </summary>
    public partial class Parents : Page
    {
        public static string Namedfat;
        public static string Namedmat;
        public Parents()
        {
            InitializeComponent();
            Binding binding = new Binding();
            //lbbatya.ItemsSource = database.parents;
            //lbmother.ItemsSource = database.parents;
            tbname.Text = database.name_worker + (" ") + database.surname_worker;
            loadparent();
            lb1.Content = Namedfat;
            lb2.Content = Namedmat;
            lbparent.ItemsSource = database.parents;
            lbmother.ItemsSource = database.parents;
        ;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //sp1.Visibility = Visibility.Hidden;
            //database.Kids.Clear();
            //database.types_Absents.Clear();
            //database.absents.Clear();
            //database.parents.Clear()
            // lb1.Visibility = Visibility.Hidden;
            tbname.Visibility = Visibility.Hidden;
            lb1.Visibility = Visibility.Hidden;
            lb2.Visibility = Visibility.Hidden;
            lbmother.Visibility = Visibility.Hidden;
            lbparent.Visibility = Visibility.Hidden;
            sp1.Visibility = Visibility.Hidden; ;
            database.Kids.Clear();
            database.types_Absents.Clear();
            sp1.Visibility = Visibility.Hidden;
            frame.Navigate(new Work());
        }

        public void loadparent()
        {
            NpgsqlCommand command = database.GetCommand("SELECT  \"Named_fat\".\"Named\" , \"Named_mat\".\"Named\",  " +
            " \"parents\".\"id\", \"parents\".\"FIO_father\", \"parents\".\"FIO_mother\", \"parents\".\"address_mother\",   " +
            " \"parents\".\"address_father\", \"parents\".\"telephone_mother\", \"parents\".\"telephone_father\"" +
            "FROM \"kids\", \"parents\" , \"Named_fat\", \"Named_mat\"  " +
            "WHERE @id_kid = \"kids\".\"id\" AND \"kids\".\"parent_id\" = \"parents\".\"id\" AND  \"Named_fat\".\"id\" =  \"parents\".\"named_fat\" AND" +
            "\"Named_mat\".\"id\" =  \"parents\".\"named_mat\"");
            //command.Parameters.AddWithValue("@Namedfat", NpgsqlDbType.Varchar, Named);
            command.Parameters.AddWithValue("@id_kid", NpgsqlDbType.Integer, database.id_kid);
            NpgsqlDataReader aaa = command.ExecuteReader();
            if (aaa.HasRows)
            {
                while (aaa.Read())
                {
                    parents Parents = new parents();
                    Namedfat = aaa.GetString(0);
                    Namedmat = aaa.GetString(1);
                    Parents.id = aaa.GetInt32(2);
                    Parents.FIO_father = aaa.GetString(3);
                    Parents.FIO_mother = aaa.GetString(4);
                    Parents.address_mother = aaa.GetString(5);
                    Parents.address_father = aaa.GetString(6);
                    Parents.telephone_mother = aaa.GetString(7);
                    Parents.telephone_father = aaa.GetString(8);
                    database.parents.Add(Parents);
                }
                aaa.Close();
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            tbname.Visibility = Visibility.Hidden;
            lb1.Visibility = Visibility.Hidden;
            lb2.Visibility = Visibility.Hidden;
            lbmother.Visibility = Visibility.Hidden;
            lbparent.Visibility = Visibility.Hidden;
            sp1.Visibility = Visibility.Hidden; ;
            frame.Navigate(new Authorization());
        }

        private void lbparent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           lbparent.UnselectAll();
        }
    }
}
