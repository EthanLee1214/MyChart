using System;
using System.Windows;
using System.ComponentModel;
using LiveCharts;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Components;
using LiveCharts.Wpf.Charts.Base;

using LiveCharts.Definitions.Charts;
using System.Windows.Input;
using System.Threading;

using EthanChart.CLS;
using EthChartDef;

namespace EthanChart.ViewModel
{
    public class ChartViewModel : ViewModelBase, ISelectWork
    {
        ChartProxy cp;

        public SeriesCollection Series { get; set; }
        public Func<double, string> XFommatter { get; set; }
        public Func<double, string> YFommatter { get; set; }
        public string[] XLabels { get; set; }

        public ChartViewModel()
        {
            Series = new SeriesCollection();

            cp = ChartProxy.Inst;

            XFommatter = val => Math.Round(val) + "ea";
            YFommatter = val => Math.Round(val) + "";
        }

        public void Selected()
        {
            if (cp.Series.Count != Series.Count)
            {
                Series.Clear();
                for (int i = 0; i < cp.Series.Count; i++)
                {
                    var srs = new LineSeries();
                    srs.Title = cp.UDS.ItemList(cp.UDS.KeyNames[0])[cp.Series[i].Indices[0]];
                    srs.Values = new ChartValues<double>();
                    Series.Add(srs);
                }
            }

            for (int i = 0; i < Series.Count; i++)
            {
                (Series[i] as Series).Title = cp.UDS.ItemList(cp.UDS.KeyNames[0])[cp.Series[i].Indices[0]];
                if (!Series[i].Values.Equals((cp.Series[i] as SenderSeries).CV))
                {
                    (cp.Series[i] as SenderSeries).CV = Series[i].Values;
                }
            }
        }

        public void Released()
        {

        }

        private ICommand startCommand;
        public ICommand StartCommand
        {
            get { return (this.startCommand) ?? (this.startCommand = new DelegateCommand(cp.Start)); }
        }

        private ICommand stopCommand;
        public ICommand StopCommand
        {
            get { return (this.stopCommand) ?? (this.stopCommand = new DelegateCommand(cp.Stop)); }
        }

        private ICommand clearCommand;
        public ICommand ClearCommand
        {
            get { return (this.clearCommand) ?? (this.clearCommand = new DelegateCommand(cp.ClearSeriesData)); }
        }

        Random rd = new Random();

        private ICommand testCommand;
        public ICommand TestCommand
        {
            get { return (this.testCommand) ?? (this.testCommand = new DelegateCommand(Test)); }
        }
        private void Test()
        {
            TestAddAsync();
        }

        private async Task TestAddAsync()
        {
            byte cnt = 0;
            while (cnt != 10)
            {
                Series[0].Values.Add((double)rd.Next(0, 10));
                Series[1].Values.Add((double)rd.Next(0, 10));
                cnt++;
                await Task.Delay(100);
            }
        }

        private ICommand test1Command;
        public ICommand Test1Command
        {
            get { return (this.test1Command) ?? (this.test1Command = new DelegateCommand(Test1)); }
        }
        private void Test1()
        {
            string msg = Series[0].Values[0].ToString();
            for (int i = 1; i < Series[0].Values.Count; i++)
            {
                msg += "\r\n" + Series[0].Values[i];
            }
            MessageBox.Show(msg);
        }
    }
}
