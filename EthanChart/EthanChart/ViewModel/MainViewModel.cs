using EthanChart.CLS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EthanChart.ViewModel
{
    public class PageCLS : ViewModelBase
    {
        int index;
        string text;
        bool isSelected;
        UserControl uc;

        public int Index
        {
            get => index;
            set
            {
                index = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public UserControl UC
        {
            get => uc;
            set
            {
                uc = value;
                OnPropertyChanged();
            }
        }

        public PageCLS(int index, string text)
        {
            Index = index;
            Text = text;                        
        }

        public PageCLS(int index, string text, UserControl uc) : this(index, text)
        {
            UC = uc;
        }
    }

    public class MainViewModel : ViewModelBase
    {
        PageCLS selectedPage;
        ObservableCollection<PageCLS> pages = new ObservableCollection<PageCLS>();

        public PageCLS SelectedPage
        {
            get => selectedPage;
            set
            {
                if (selectedPage == value) return;

                if (selectedPage != null)
                {
                    selectedPage.IsSelected = false;
                    (selectedPage.UC.DataContext as ISelectWork)?.Released();
                }
                selectedPage = value;
                selectedPage.IsSelected = true;
                (selectedPage.UC.DataContext as ISelectWork)?.Selected();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PageCLS> Pages
        {
            get => pages;
            set
            {   
                pages = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            pages.Add(new PageCLS(0, "Setting", new View.SettingView()));
            pages.Add(new PageCLS(1, "Series", new View.SeriesView()));
            pages.Add(new PageCLS(2, "Chart", new View.ChartView()));

            SelectedPage = pages[2];
        }
    }
}
