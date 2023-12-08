using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace practice1
{
    public class MainViewModel : INotifyPropertyChanged //Пояснение этот класс я душе не ебу но здесь фильтры функции 
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static ICollectionView view;
        public static ICollectionView view1;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ObservableCollection<absent> Items
        {
            get { return database.absents; }
            set
            {
                if (database.absents != value)
                {
                    database.absents = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }
        private string searchKeyword;
        public string SearchKeyword
        {
            get { return searchKeyword; }
            set
            {
                if (searchKeyword != value)
                {
                    searchKeyword = value;
                    OnPropertyChanged(nameof(SearchKeyword));
                    ApplyFilterName();
                }
            }
        }
        private void ApplyFilterName()
        {
            view = CollectionViewSource.GetDefaultView(Items);
            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                // Используйте предикат для фильтрации элементов по заданному критерию поиска
                view.Filter = item =>
                {
                    absent currentItem = item as absent;
                    return currentItem.name.Contains(SearchKeyword) || currentItem.surname.Contains(SearchKeyword) || currentItem.patronymic.Contains(SearchKeyword);
                };
            }
            else
            {
                // Сбросьте фильтр, если критерий поиска пуст
                view.Filter = null;
            }
        }
        private string searchKeyworddate;
        public string SearchKeyworddate
        {
            get { return searchKeyworddate; }
            set
            {
                if (searchKeyworddate != value)
                {
                    searchKeyworddate = value;
                    OnPropertyChanged(nameof(SearchKeyworddate));
                    ApplyFilterDate();
                }
            }
        }
        private void ApplyFilterDate()
        {
             view1 = CollectionViewSource.GetDefaultView(Items);
            //view1 = CollectionViewSource.GetDefaultView(Items);
            if (!string.IsNullOrEmpty(SearchKeyworddate))
            {
                // Используйте предикат для фильтрации элементов по заданному критерию поиска
                view1.Filter = item =>
                {
                    absent currentItem = item as absent;
                    return currentItem.date1.Contains(SearchKeyworddate);
                };
            }
            else
            {
                // Сбросьте фильтр, если критерий поиска пуст
                view1.Filter = null;
            }
        }

    }
}
