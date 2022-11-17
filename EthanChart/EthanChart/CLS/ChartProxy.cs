using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EthChartDef;
using LiveCharts.Definitions.Charts;
using MyChartDataSender;
using System.Reflection;

namespace EthanChart.CLS
{
    public sealed class ChartProxy
    {
        static ChartProxy inst;
        public static ChartProxy Inst => inst ?? (inst = new ChartProxy());
        public IList<ISeriesData> Series { get; private set; }

        DataSender ds;

        public UserDefSender UDS { get; private set; }
        public string DLLName { get; private set; }

        public bool IsOpen => UDS != null;

        private ChartProxy() 
        {
            Series = new List<ISeriesData>();
            ds = new DataSender();            
        }

        public void LoadUserDefDLL()
        {
            DLLName = @"C:\Users\HP\Desktop\MyPortfolio\MyChart\EthanChart\exe\x64\UserChartSender_WMX.dll";

            var asm = Assembly.LoadFile(DLLName);
            var types = asm.GetExportedTypes();

            UserDefSender uds = null;

            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].BaseType == typeof(UserDefSender))
                {
                    uds = Activator.CreateInstance(types[i]) as UserDefSender;
                    break;
                }
            }

            if (uds == null) return;

            if (ds == null) ds = new DataSender();
            ds.Close();
            ds.Open(uds);
            UDS = uds;
        }

        public void Start()
        {
            if (ds == null || !ds.IsOpen) return;
            ds.ConfigSeries(Series);
            ds.Start();
        }

        public void Stop()
        {
            ds.Stop();
        }

        public void ClearAndAddSeries(int count)
        {
            if (count < 0) return;
            if (count > Define.MaxSeriesNum) return;            
            Series.Clear();

            for (int i = 0; i < count; i++)
            {
                Series.Add(new SenderSeries());
            }
        }

        public void ClearSeriesData()
        {
            foreach (var s in Series)
            {
                (s as SenderSeries).CV.Clear();
            }
        }
    }

    public class SenderSeries : ISeriesData
    {
        public IChartValues CV { get; set; }

        public int[] Indices { get; set; }

        public Action<double> ValueReceived { get; private set; }

        public SenderSeries()
        {
            ValueReceived += val =>
            {
                if (CV == null) return;
                CV.Add(val);
            };
        }

        public void SetIndices(int[] val)
        {
            Indices = val;
        }
    }
}
