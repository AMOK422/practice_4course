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
    /// Логика взаимодействия для events.xaml
    /// </summary>
    public partial class events : Page
    {
      
        public events()
        {
            InitializeComponent();
            loadevent();
            lbevents.ItemsSource = database.events;
            tbname.Text = database.name_worker + (" ") + database.surname_worker;

        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {   
            tbname.Visibility = Visibility.Hidden;
            lbevents.Visibility = Visibility.Hidden;
            sp1.Visibility = Visibility.Hidden;
            lb.Visibility = Visibility.Hidden;
            database.Kids.Clear();
            database.types_Absents.Clear();
            
            frame.Navigate(new Work());
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            tbname.Visibility = Visibility.Hidden;
            lbevents.Visibility = Visibility.Hidden;
            sp1.Visibility = Visibility.Hidden;
            //database.absents.Clear();
            lb.Visibility = Visibility.Hidden;
            frame.Navigate(new checkabsent());
        }
        private void lbparent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lbevents.UnselectAll();
        }
        public void loadevent()
        {
            
            NpgsqlCommand command1 = database.GetCommand("SELECT  \"events\".\"id\",  \"events\".\"name\",  \"events\".\"start\", \"events\".\"end\",  \"place\".\"name_place\"" +
            " FROM \"events\", \"place\", \"Group\"" +
            " WHERE \"events\".\"place\" = \"place\".\"id\" AND \"events\".\"group_id\" = \"Group\".\"id\" AND  \"Group\".\"id\" = @group ");
            command1.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, database.id_group);
            NpgsqlDataReader aaa = command1.ExecuteReader();
            if (aaa.HasRows)
            {
                while (aaa.Read())
                {
                   
                    Event events = new Event();
                    events.name = aaa.GetString(1);
                    events.start = aaa.GetDateTime(2);
                    events.start1 = events.start.ToString("dd/MM/yyyy HH:mm:ss");
                    events.end = aaa.GetDateTime(3);
                    events.end1 = events.end.ToString("dd/MM/yyyy HH:mm:ss");
                    events.place = aaa.GetString(4);
                    database.events.Add(events);
                }
                aaa.Close();
            }
        }

        private void RadioButton_Click_2(object sender, RoutedEventArgs e)
        {
            tbname.Visibility = Visibility.Hidden;
            lbevents.Visibility = Visibility.Hidden;
            sp1.Visibility = Visibility.Hidden;
            lb.Visibility = Visibility.Hidden;
            frame.Navigate(new Authorization());
        }
    }
}
