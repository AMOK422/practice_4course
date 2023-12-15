using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace practice1
{
    /// <summary>
    /// Логика взаимодействия для PageForzav.xaml
    /// </summary>
    public partial class PageForzav : Page
    {
        public static int id;
        public static int id_kid;
        public string cbselectedval;
        public static ICollectionView view; 
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) // кринж событие для фильтра
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ObservableCollection<kids> Items //кринж коллекция для фильтра
        {
            get { return database.Kids; }
            set
            {
                if (database.Kids != value)
                {
                    database.Kids = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }
        public PageForzav()
        {
            InitializeComponent();
            loadkids();
            loadgroup();
            tbname.Text = database.name_worker + (" ") + database.surname_worker;
            Binding binding = new Binding();
            binding.Source = database.Kids;
            lbkids.ItemsSource = database.Kids;
            cbgroup.ItemsSource = database.groups;
            cbgroup1.ItemsSource = database.groups;
        }
        public void loadkids()
        {
            NpgsqlCommand command = database.GetCommand("SELECT \"kids\".\"id\",  \"kids\".\"name\", \"surname\", \"patronymic\" , \"group\", \"Group\".\"number\" ,\"Group\".\"avg_age\" " +
           "FROM \"kids\" , \"Group\" " +
           "WHERE \"kids\".\"group\" = \"Group\".\"id\" " +
           " ORDER BY \"surname\"  ");
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
                    kids.namegroup = aaa.GetInt32(5);
                    kids.avg_age_group = aaa.GetString(6);
                    database.Kids.Add(kids);
                }
                aaa.Close();
            }
        }

        public void loadgroup()
        {
            NpgsqlCommand command = database.GetCommand("SELECT *" +
          "FROM \"Group\" ");
            NpgsqlDataReader aaa = command.ExecuteReader();
            if (aaa.HasRows)
            {
                while (aaa.Read())
                {
                    group group = new group();
                    group.id = aaa.GetInt32(0);
                    group.number = aaa.GetInt32(1);
                    group.avgage = aaa.GetString(2);
                    database.groups.Add(group);
                }
            }
        }
        public void ApplyFilterName()
        {
            view = CollectionViewSource.GetDefaultView(Items);
            if (!string.IsNullOrEmpty(cbselectedval))
            {
                // Используйте предикат для фильтрации элементов по заданному критерию поиска
                view.Filter = item =>
                {
                    kids currentItem = item as kids;
                    
                    return currentItem.avg_age_group.Contains(cbselectedval);
                };
            }
            else
            {
                // Сбросьте фильтр, если критерий поиска пуст
                view.Filter = null;
            }
        }

        private void cbgroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbgroup.SelectedItem != null)
            {
                var selectedValue = cbgroup.SelectedItem as group;
                cbselectedval = selectedValue.avgage.ToString();
                ApplyFilterName();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           cbgroup.SelectedItem = null;
           view.Filter = null;
        }

        private void lbkids_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbkids.SelectedItem != null)
            {
                var selectitlb = lbkids.SelectedItem as kids;
                tbnamekid.Text = selectitlb.name.ToString();
                tbpatronymickid.Text = selectitlb.patronymic.ToString();
                tbsurnamekid.Text = selectitlb.surname.ToString();
                cbgroup1.Text = selectitlb.namegroup.ToString();
                butcr.Visibility = Visibility.Hidden;
            }
        }

        private void update(object sender, RoutedEventArgs e)
        {
            var select = lbkids.SelectedItem as kids;
            var selectedValue = cbgroup1.SelectedItem as group;
            if (select == null)
            {
                database.message = ("Выберите ребенка которого хотите обновить");
                database.OpenNotification_Click();
                //MessageBox.Show("Выберите пропуск который хотите удалить");
            }
            else
            {
                id_kid = select.id;
                id = selectedValue.id;
                string name = tbnamekid.Text.Trim();
                string surname = tbsurnamekid.Text.Trim();
                string patronymic = tbpatronymickid.Text.Trim();

                NpgsqlCommand command = database.GetCommand("UPDATE  \"kids\" SET name = @name, surname= @surname, patronymic = @patronymic, \"group\" = @group   WHERE id = @id ");
                command.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id_kid);
                command.Parameters.AddWithValue("@name", NpgsqlDbType.Varchar, name);
                command.Parameters.AddWithValue("@surname", NpgsqlDbType.Varchar, surname);
                command.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, patronymic);
                command.Parameters.AddWithValue("group", NpgsqlDbType.Integer, id);
                var result = command.ExecuteNonQuery();
                if (result == 1)
                {
                    database.message = ("Успешно изменено");
                    database.OpenNotification_Click();
                    //MessageBox.Show("Успешно изменено");
                    database.Kids.Remove(select);
                    database.loadkidsupdate();


                    //P.S Я не знал как сделать и решил такой костыль написать если кто то это будет читать прошу не бейте
                }
                else
                {
                    database.message = ("Что то пошло не так");
                    database.OpenNotification_Click();
                    //MessageBox.Show("Что то пошло не так");
                }
            }
        }

        private void delete (object sender, RoutedEventArgs e)
        {
            tbnamekid.Text = "";
            tbsurnamekid.Text = "";
            tbpatronymickid.Text = "";
            cbgroup1.SelectedItem = null;
            var selectitemlist = lbkids.SelectedItem as kids;
            if (selectitemlist == null)
            {
                database.message = ("Выберите ребенка которого хотите удалить");
                database.OpenNotification_Click();
                //MessageBox.Show("Выберите пропуск который хотите удалить");
            }
            else
            {
                var selectitemlist1 = lbkids.SelectedItem as kids;
                int id = selectitemlist1.id;
                NpgsqlCommand command = database.GetCommand("DELETE FROM \"Absents\" WHERE \"Absents\".\"id_kid\" = @idkid  ");
                NpgsqlCommand command1 = database.GetCommand("DELETE FROM \"kids\" WHERE \"kids\".\"id\" = @idkid  ");
                command.Parameters.AddWithValue("@idkid", NpgsqlDbType.Integer, id);
                command1.Parameters.AddWithValue("@idkid", NpgsqlDbType.Integer, id);
                var result = command1.ExecuteNonQuery();
                if (result == 1)
                {
                    database.message = ("Ребенок успешно удален");
                    database.OpenNotification_Click();
                    //MessageBox.Show("Пропуск успешно удален");
                    database.Kids.Remove(selectitemlist);
                    butcr.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Не получилось");

                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var selectedValue = cbgroup1.SelectedItem as group;
            id = selectedValue.id;
            string name = tbnamekid.Text.Trim();
            string surname = tbsurnamekid.Text.Trim();
            string patronymic = tbpatronymickid.Text.Trim();
            NpgsqlCommand command = database.GetCommand("" +
                             "INSERT INTO  \"kids\" (\"id\", \"name\", \"surname\", \"patronymic\", \"group\" ) " +
                            "VALUES(@id_kid, @name, @surname, @patronymic, @group )");
            database.id_kiid++;
            command.Parameters.AddWithValue("@id_kid", NpgsqlDbType.Integer, database.id_kiid );
            command.Parameters.AddWithValue("@name", NpgsqlDbType.Varchar, name);
            command.Parameters.AddWithValue("@surname", NpgsqlDbType.Varchar, surname);
            command.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, patronymic);
            command.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, id);
            var result = command.ExecuteNonQuery();
            if (result == 1)
            {
                database.message = ("Ребенок успешно внесен в базу");
                database.OpenNotification_Click();
                //MessageBox.Show("Пропуск успешно внесен в базу");
                database.loadkidinsert();

            }
            else
            {
                database.message = ("Упс не получилось");
                database.OpenNotification_Click();
                //MessageBox.Show("Упс не получилось");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            tbnamekid.Text = "";
            tbsurnamekid.Text = "";
            tbpatronymickid.Text = "";
            cbgroup1.SelectedItem = null;
            butcr.Visibility = Visibility.Visible;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            sp.Visibility = Visibility.Hidden;
            sp1.Visibility = Visibility.Hidden;
            sp2.Visibility = Visibility.Hidden;
            tbname.Visibility = Visibility.Hidden;
            tbnamekid.Visibility = Visibility.Hidden;
            tbsurnamekid.Visibility = Visibility.Hidden;
            tbpatronymickid.Visibility = Visibility.Hidden;
            lbkids.Visibility = Visibility.Hidden;
            cbgroup.Visibility = Visibility.Hidden;
            cbgroup1.Visibility = Visibility.Hidden;
            lb.Visibility = Visibility.Hidden;
            lb1.Visibility = Visibility.Hidden;
            butcr.Visibility = Visibility.Hidden;
            butdel.Visibility = Visibility.Hidden;
            butup.Visibility = Visibility.Hidden;
            frame.Navigate(new Authorization());
            database.Kids.Clear();
            database.groups.Clear();
        }
    }
}
