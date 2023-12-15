using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.VariantTypes;
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using System.IO;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Vml;

namespace practice1
{
    /// <summary>
    /// Логика взаимодействия для checkabsent.xaml
    /// </summary>
    /// 
    public partial class checkabsent : Page
    {
        public MainViewModel viewModel;
        public static string type;
        public static string dateabsent;
        private int clickCount = 0;
        public checkabsent()
        {
            InitializeComponent();
            tbname.Text = database.name_worker + (" ") + database.surname_worker;
            //database.load_type();
            loadabsent();
            //database.loadkids();
            lbhooky.ItemsSource = database.absents;
            cbtype.ItemsSource = database.types_Absents;
            tbdate.Text = null;
            viewModel = new MainViewModel();
            DataContext = viewModel;
        }
        private void lbhooky_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectitemlist = lbhooky.SelectedItem as absent;

            if (selectitemlist == null) //Проверка если объект равен 0  тогда выбери объект чтобы код сработал чмарь
            {

            }
            else //если объект выбран тогда записывай данные
            {
                type = selectitemlist.type_absent;
                dateabsent = selectitemlist.date1;
                cbtype.Text = type;
                tbdate.Text = dateabsent;
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            butq.Visibility = Visibility.Hidden;
            tbname.Visibility = Visibility.Hidden;
            sp.Visibility = Visibility.Hidden;
            sp1.Visibility = Visibility.Hidden;
            sp2.Visibility = Visibility.Hidden;
            lbhooky.Visibility = Visibility.Hidden;
            tb3.Visibility = Visibility.Hidden;
            sp3.Visibility = Visibility.Hidden;
            sp4.Visibility = Visibility.Hidden;
            cbtype.Visibility = Visibility.Hidden;

            //database.absents.Clear();
            //database.Kids.Clear();
            database.Kids.Clear();
            database.types_Absents.Clear();
            database.absents.Clear();
            frame.Navigate(new Work());
        }
        private void OpenCalendarButton_Click(object sender, RoutedEventArgs e)
        {
            clickCount++;

            if (clickCount == 2)
            {
                // Действие, которое нужно выполнить при нажатии кнопки дважды
                calendar1.Visibility = Visibility.Hidden;

                // Сброс счетчика
                clickCount = 0;
            }
            else
            {
                calendar1.Visibility = Visibility.Visible;
            }
        }
        private void calendar1_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selected = calendar1.SelectedDate ?? DateTime.MinValue;
            if (selected.DayOfWeek == DayOfWeek.Saturday || selected.DayOfWeek == DayOfWeek.Sunday)
            {
                database.message = ("В выходные нельзя добавлять пропуск");
                database.OpenNotification_Click();
                //MessageBox.Show("В выходные нельзя добавлять пропуск");
            }
            else
            {
                DateTime? selectedDate = calendar1.SelectedDate;
                database.date = selectedDate;
                tbdate.Text = selectedDate.ToString();
                tbdate.Text = DateTime.Parse(tbdate.Text).ToShortDateString();
                calendar1.Visibility = Visibility.Hidden;
                //lbdata.Text = selectedDate.ToString();
                //lbdata.Text = DateTime.Parse(lbdata.Text).ToShortDateString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectitemlist = lbhooky.SelectedItem as absent;
            var selecteditemcb = cbtype.SelectedItem as type_absent;
            DateTime selected = calendar1.SelectedDate ?? DateTime.MinValue;
            if (selected.DayOfWeek == DayOfWeek.Saturday || selected.DayOfWeek == DayOfWeek.Sunday)
            {
                database.message = ("В выходные нельзя добавлять пропуск");
                database.OpenNotification_Click();
                //MessageBox.Show("В выходные нельзя добавлять пропуск");
            }
            else
            {
                if (selectitemlist == null)
                {
                    database.message = ("Выберите пропуск который хотите удалить");
                    database.OpenNotification_Click();
                    //MessageBox.Show("");
                }
                else
                {
                    int id = selectitemlist.id;
                    database.id_absent = id;
                    int id_type = selecteditemcb.id;
                    string type = selecteditemcb.name;
                    if (calendar1.SelectedDate == null)
                    {
                        NpgsqlCommand command = database.GetCommand("UPDATE  \"Absents\" SET  type=@type WHERE \"Absents\".\"id\" = @id_absent");
                        command.Parameters.AddWithValue("@id_absent", NpgsqlDbType.Integer, id);
                        command.Parameters.AddWithValue("@type", NpgsqlDbType.Integer, id_type);
                        var result = command.ExecuteNonQuery();
                        if (result == 1)
                        {
                            database.message = ("Успешно изменено");
                            database.OpenNotification_Click();
                            //MessageBox.Show("Успешно изменено");
                            database.absents.Remove(selectitemlist); //Удаляем выбранный объект из колекции
                            database.Lastman_forupdate(); //Загружаем обновленный 
                            //P.S Я не знал как сделать и решил такой костыль написать если кто то это будет читать прошу не бейте


                        }
                        else
                        {
                            database.message = ("Что то пошло не так");
                            database.OpenNotification_Click();
                            //MessageBox.Show("Что то пошло не так");
                        }
                    }
                    else
                    {
                        DateTime? selectedDate = calendar1.SelectedDate;

                        NpgsqlCommand command = database.GetCommand("UPDATE  \"Absents\" SET  type=@type,  date = @data   WHERE \"Absents\".\"id\" = @id_absent");
                        command.Parameters.AddWithValue("@id_absent", NpgsqlDbType.Integer, id);
                        command.Parameters.AddWithValue("@type", NpgsqlDbType.Integer, id_type);
                        command.Parameters.AddWithValue("@data", NpgsqlDbType.Date, selectedDate);
                        var result = command.ExecuteNonQuery();
                        if (result == 1)
                        {
                            database.message = ("Успешно изменено");
                            database.OpenNotification_Click();
                            //MessageBox.Show("Успешно изменено");
                            database.absents.Remove(selectitemlist);
                            database.Lastman_forupdate();
                        }
                        else
                        {
                            MessageBox.Show("Что то пошло не так");
                        }
                    }
                }
            }

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var selectitemlist = lbhooky.SelectedItem as absent;

            if (selectitemlist == null)
            {
                database.message = ("Выберите пропуск который хотите удалить");
                database.OpenNotification_Click();
                //MessageBox.Show("Выберите пропуск который хотите удалить");
            }
            else
            {
                int id = selectitemlist.id;
                NpgsqlCommand command = database.GetCommand("DELETE FROM \"Absents\" WHERE \"Absents\".\"id\" = @id_absent  ");
                command.Parameters.AddWithValue("@id_absent", NpgsqlDbType.Integer, id);
                var result = command.ExecuteNonQuery();
                if (result == 1)
                {
                    database.message = ("Пропуск успешно удален");
                    database.OpenNotification_Click();
                    //MessageBox.Show("Пропуск успешно удален");
                    database.absents.Remove(selectitemlist);
                }
                else
                {
                    MessageBox.Show("Что то пошло не так");

                }
            }
        }
        public void loadabsent()
        {
            NpgsqlCommand command =
            database.GetCommand("SELECT \"Absents\".\"id\", \"kids\".\"group\",  \"kids\".\"name\",\"kids\".\"surname\"," +
           "\"kids\".\"patronymic\", \"Absents\".\"date\", \"Type_absents\".\"name\" " +
           "FROM \"kids\", \"Absents\", \"Type_absents\" " +
           "WHERE @group = \"group\" AND  \"Absents\".\"type\" = \"Type_absents\".\"id\" AND \"Absents\".\"id_kid\" = \"kids\".\"id\" " +
            "ORDER BY \"Absents\".\"date\" ASC ");
            command.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, database.id_group);
            NpgsqlDataReader aaa = command.ExecuteReader();
            if (aaa.HasRows)
            {
                while (aaa.Read())
                {
                    absent absent = new absent();
                    absent.id = aaa.GetInt32(0);
                    absent.name = aaa.GetString(2);
                    absent.surname = aaa.GetString(3);
                    absent.patronymic = aaa.GetString(4);
                    absent.date = aaa.GetDateTime(5);
                    string a = absent.date.ToString();
                    absent.date1 = DateTime.Parse(a).ToShortDateString();
                    absent.type_absent = aaa.GetString(6);
                    database.absents.Add(absent);
                }
                aaa.Close();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "doc files (*.doc)|*.doc|All files (*.*)|*.*";
            try
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var wordDoc = WordprocessingDocument.Create(saveFileDialog.FileName, WordprocessingDocumentType.Document))
                    {
                        var mainPart = wordDoc.AddMainDocumentPart();

                        // Создаем новый документ
                        mainPart.Document = new Document();

                        // Добавляем раздел, содержащий параграфы
                        var body = new Body();
                        mainPart.Document.Append(body);

                        // Получаем выбранные элементы из ListBox

                        foreach (absent item in lbhooky.Items)
                        {
                            // Создаем новый параграф для каждого выбранного элемента
                            var paragraph = new Paragraph();
                            body.Append(paragraph);

                            // Добавляем текст элемента в параграф

                            var run = new Run(new Text(item.name + " " + item.surname + " " + item.patronymic + " " + item.date1 + " " + item.type_absent));
                            paragraph.Append(run);
                        }
                        database.message = ("Документ Word успешно создан!");
                        database.OpenNotification_Click();
                        //MessageBox.Show("Документ Word успешно создан!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            butq.Visibility = Visibility.Hidden;
            tbname.Visibility = Visibility.Hidden;
            sp.Visibility = Visibility.Hidden;
            sp1.Visibility = Visibility.Hidden;
            sp2.Visibility = Visibility.Hidden;
            lbhooky.Visibility = Visibility.Hidden;
            tb3.Visibility = Visibility.Hidden;
            sp3.Visibility = Visibility.Hidden;
            sp4.Visibility = Visibility.Hidden;
            cbtype.Visibility = Visibility.Hidden;
            database.Kids.Clear();
            database.types_Absents.Clear();
            database.absents.Clear();
            frame.Navigate(new Authorization());
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            butq.Visibility = Visibility.Hidden;
            tbname.Visibility = Visibility.Hidden;
            sp.Visibility = Visibility.Hidden;
            sp1.Visibility = Visibility.Hidden;
            sp2.Visibility = Visibility.Hidden;
            lbhooky.Visibility = Visibility.Hidden;
            tb3.Visibility = Visibility.Hidden;
            sp3.Visibility = Visibility.Hidden;
            sp4.Visibility = Visibility.Hidden;
            cbtype.Visibility = Visibility.Hidden;
            database.events.Clear();
            frame.Navigate(new events());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            database.message = ("Для фильтрации за месяц нужно написать точку - . перед номером месяца. Пример для фильтрации за декабрь напишите .12") ;
            database.OpenNotification_Click();
        }
    }
}


