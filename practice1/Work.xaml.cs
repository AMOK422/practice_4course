using DocumentFormat.OpenXml.Wordprocessing;
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
    /// Логика взаимодействия для Work.xaml
    /// </summary>
    public partial class Work : Page
    {
        public Work()
        {
            InitializeComponent();
            tbname.Text = database.name_worker + (" ") + database.surname_worker;
            tbnamegroup.Text = database.name_group + (" ") + database.number_group.ToString();
            loadkids();
            load_type();
            
            Binding binding = new Binding();
            binding.Source = database.Kids;
            cbtype.ItemsSource = database.types_Absents; 
            lbkids.ItemsSource = database.Kids;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
           
            tbnamegroup.Visibility = Visibility.Hidden;
            tbname.Visibility = Visibility.Hidden;
            sp.Visibility = Visibility.Hidden;
            lbkids.Visibility = Visibility.Hidden;
            lb1.Visibility = Visibility.Hidden;
            lb2.Visibility = Visibility.Hidden;
            database.Kids.Clear();
            frame.Navigate(new Authorization());  
        }

        private void lbkids_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = lbkids.SelectedItem as kids;
            if (selectedItem == null)
            {

            }
            else
            database.id_kid = selectedItem.id;
        }
      
        private void calendar1_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = calendar1.SelectedDate; //я не знаю почему datatime но уже сделано
            DateTime selected = calendar1.SelectedDate ?? DateTime.MinValue;
            if (selected.DayOfWeek == DayOfWeek.Saturday || selected.DayOfWeek == DayOfWeek.Sunday) //Проверка чтобы не вносить пропуски в выходные
            {
                selectedDate = null;
                database.date = null;
                lbdata.Text = ("");
                database.message = ("В выходные нельзя добавлять пропуск");
                database.OpenNotification_Click();
                //MessageBox.Show
            }
            else
            {
                
                selectedDate = calendar1.SelectedDate; 
                database.date = selectedDate;
                lbdata.Text = selectedDate.ToString();
                lbdata.Text = DateTime.Parse(lbdata.Text).ToShortDateString();
            }
        }
        private void cbtype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selecteditemtype = cbtype.SelectedItem as type_absent;
            if (selecteditemtype == null)
            {

            }
            else
           database.selectypeid = selecteditemtype.id;
        }
      
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (database.date == null)
            {
                database.message = ("Выберите дату");
                database.OpenNotification_Click();
                //MessageBox.Show("Выберите дату");
            }
            else
            {
                bool a = checksome();
                if (a == true)
                {
                     database.message = ("Такой пропуск уже есть");
                    database.OpenNotification_Click();
                    //MessageBox.Show("Такой пропуск уже есть");
                }
                else
                {
                    string type = cbtype.Text.Trim();
                    if (type.Length <= 0)
                    {
                        database.message = ("Выберите причину пропуска ");
                        database.OpenNotification_Click();
                        //MessageBox.Show("Выберите причину пропуска ");
                    }
                    else
                    {
                        if (lbkids.SelectedItem != null)
                        {
                            NpgsqlCommand command = database.GetCommand("" +
                             "INSERT INTO \"Absents\"(\"id_kid\", \"type\", \"date\") " +
                            "VALUES(@id_kid, @type, @date)");
                            command.Parameters.AddWithValue("@id_kid", NpgsqlDbType.Integer, database.id_kid);
                            command.Parameters.AddWithValue("@type", NpgsqlDbType.Integer, database.selectypeid);
                            command.Parameters.AddWithValue("@date", NpgsqlDbType.Date, database.date);
                            var result = command.ExecuteNonQuery();
                            if (result == 1)
                            {
                                database.message = ("Пропуск успешно внесен в базу");
                                database.OpenNotification_Click();
                                //MessageBox.Show("Пропуск успешно внесен в базу");
                                database.Lastman_forinsert();

                            }
                            else
                            {
                                database.message = ("Упс не получилось");
                                database.OpenNotification_Click();
                                //MessageBox.Show("Упс не получилось");
                            }
                        }
                        else
                        {
                            database.message = ("Выберите ребенка");
                            database.OpenNotification_Click();
                           /* MessageBox.Show("Выберите ребенка")*/;
                        }
                    }
                }
            }
        }
        public bool checksome()
        {
            NpgsqlCommand command1 = database.GetCommand("SELECT * FROM \"Absents\" Where id_kid = @id_kid AND date= @date");
            command1.Parameters.AddWithValue("@id_kid", NpgsqlDbType.Integer, database.id_kid);
            command1.Parameters.AddWithValue("@date", NpgsqlDbType.Date, database.date);
            NpgsqlDataReader result = command1.ExecuteReader();
            if (result.HasRows)
            {
                result.Close();
                return true;
            }
            result.Close();
            return false;
        }
        private void Button_Click_travel_checkhooky(object sender, RoutedEventArgs e)
        {
            tbnamegroup.Visibility = Visibility.Hidden;
            tbname.Visibility = Visibility.Hidden; 
            sp.Visibility = Visibility.Hidden;
            lbkids.Visibility = Visibility.Hidden;
            lb1.Visibility = Visibility.Hidden;
            lb2.Visibility = Visibility.Hidden;
            database.absents.Clear();
            frame.Navigate(new checkabsent());
        }
        public void loadkids()
        {
            NpgsqlCommand command = database.GetCommand("SELECT \"kids\".\"id\",  \"kids\".\"name\", \"surname\", \"patronymic\" , \"group\"" +
           "FROM \"kids\" " +
           "WHERE @group = \"group\"   ORDER BY \"surname\"  ");
            command.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, database.id_group);
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
                    database.Kids.Add(kids);
                }
                aaa.Close();
            }
        }
      
        public void load_type()
        {
            NpgsqlCommand command1 = database.GetCommand("SELECT \"id\",  \"name\" FROM \"Type_absents\" ORDER BY id");
            NpgsqlDataReader aaa = command1.ExecuteReader();
            if (aaa.HasRows)
            {
                while (aaa.Read())
                {
                    type_absent type = new type_absent();
                    type.id = aaa.GetInt32(0);
                    type.name = aaa.GetString(1);
                    database.types_Absents.Add(type);
                }
                aaa.Close();
            }
        }

        private void lbkids_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            tbnamegroup.Visibility = Visibility.Hidden;
            tbname.Visibility = Visibility.Hidden;
            sp.Visibility = Visibility.Hidden;
            lbkids.Visibility = Visibility.Hidden;
            lb1.Visibility = Visibility.Hidden;
            lb2.Visibility = Visibility.Hidden;
            database.parents.Clear();
            frame.Navigate(new Parents());
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            tbnamegroup.Visibility = Visibility.Hidden;
            tbname.Visibility = Visibility.Hidden;
            sp.Visibility = Visibility.Hidden;
            lbkids.Visibility = Visibility.Hidden;
            lb1.Visibility = Visibility.Hidden;
            lb2.Visibility = Visibility.Hidden;
            database.events.Clear();
            frame.Navigate(new events());
        }
    }  
}
