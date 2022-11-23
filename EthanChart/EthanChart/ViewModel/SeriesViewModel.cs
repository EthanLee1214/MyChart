using EthChartDef;
using LiveCharts.Definitions.Charts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using EthanChart.CLS;
using EthChartDef.App;

namespace EthanChart.ViewModel
{
    public class SeriesViewModel : ViewModelBase, ISelectWork
    {   
        ChartProxy<SenderSeries> cp;

        ObservableCollection<SeriesItem> series;
        public ObservableCollection<SeriesItem> Series
        {
            get => series;
            set
            {
                series = value;
                OnPropertyChanged();
            }
        }

        string[] headItem;

        public string HeadText1
        {
            get
            {
                if (headItem == null) return null;
                if (headItem.Length < 1) return null;
                return headItem[0];
            }
        }

        public string HeadText2
        {
            get
            {
                if (headItem == null) return null;
                if (headItem.Length < 2) return null;
                return headItem[1];
            }
        }

        public string HeadText3
        {
            get
            {
                if (headItem == null) return null;
                if (headItem.Length < 3) return null;
                return headItem[2];
            }
        }

        public string[] ItemList1 => cp.UDS.ItemList(HeadText1);
        public string[] ItemList2 => cp.UDS.ItemList(HeadText2);
        public string[] ItemList3 => cp.UDS.ItemList(HeadText3);

        public SeriesViewModel()
        {
            cp = ChartProxy<SenderSeries>.Inst;
            series = new ObservableCollection<SeriesItem>();            
        }

        public void Selected()
        {
            if (!cp.IsUserSenderSet)
            {
                string dllName = Environment.CurrentDirectory + "\\UserChartSender_WMX.dll";
                if (cp.LoadUserDefDLL(dllName))
                {
                    headItem = cp.UDS.KeyNames;
                }
                else
                {
                    headItem = null;
                }
                

                OnPropertyChanged(nameof(HeadText1));
                OnPropertyChanged(nameof(HeadText2));
                OnPropertyChanged(nameof(HeadText3));
            }

            if (series.Count == 0)
            {
                series.Add(new SeriesItem());
                series.Add(new SeriesItem());
            }
        }

        public void Released()
        {

        }

        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get { return (this.saveCommand) ?? (this.saveCommand = new DelegateCommand(Save)); }
        }
        private void Save()
        {
            cp.ClearAndAddSeries(series.Count);
            for (int i = 0; i < series.Count; i++)
            {
                (cp.Series[i] as SenderSeries).Indices = series[i].Value;
            }
        }

        private ICommand testCommand;
        public ICommand TestCommand
        {
            get { return (this.testCommand) ?? (this.testCommand = new DelegateCommand(Test)); }
        }
        private void Test()
        {
            MessageBox.Show(series[0].ToString());
        }        
    }

    public class SeriesItem
    {
        int[] val;

        public int Index1 
        { 
            get
            {
                if (val.Length < 1) return -1;
                return val[0];
            }
            set
            {
                if (val.Length < 1) return;
                val[0] = value;
            }
        }
        public int Index2
        {
            get
            {
                if (val.Length < 2) return -1;
                return val[1];
            }
            set
            {
                if (val.Length < 2) return;
                val[1] = value;
            }
        }
        public int Index3
        {
            get
            {
                if (val.Length < 3) return -1;
                return val[2];
            }
            set
            {
                if (val.Length < 3) return;
                val[2] = value;
            }
        }

        public int[] Value => val;

        public SeriesItem()
        {
            val = new int[ChartProxy<SenderSeries>.Inst.UDS.KeyCount];
            Clear();
        }

        private void Clear()
        {
            Array.Clear(val, 0, val.Length);
        }

        public override string ToString()
        {
            return String.Format("{0} - {1} - {2}", Index1, Index2, Index3);
        }
    }    
}