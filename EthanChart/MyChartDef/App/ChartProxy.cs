using EthChartDef.Sender;
using EthChartDef.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EthChartDef.App
{
    public class ChartProxy<TSeries>        
        where TSeries : ISeriesData
    {
        static ChartProxy<TSeries> inst;
        public static ChartProxy<TSeries> Inst => inst ?? (inst = new ChartProxy<TSeries>());
        public IList<ISeriesData> Series { get; protected set; }

        protected IDataSender ds;
        private string dataSenderDllName;

        public UserDefSender UDS { get; protected set; }
        public string DLLName { get; protected set; }

        public virtual IDataSender DS 
        { 
            get
            {
                if (ds == null)
                {
                    if (string.IsNullOrEmpty(dataSenderDllName))
                    {
                        dataSenderDllName = Environment.CurrentDirectory + "\\MyChartDataSender.dll";
                    }

                    try
                    {
                        ds = Util.GetInterfaceToAssembly<IDataSender>(dataSenderDllName);
                    }
                    catch (Exception ex)                    
                    {
                        // Err
                    }
                }
                return ds;
            }
        }

        public bool IsDataSenderSet => DS != null;
        public bool IsUserSenderSet => UDS != null;

        protected ChartProxy()
        {
            Series = new List<ISeriesData>();
        }

        public ChartProxy(string dataSenderDllName) : this()
        {
            this.dataSenderDllName = dataSenderDllName;
        }

        public bool SetUDS(UserDefSender uds)
        {
            if (!IsDataSenderSet) return false;
            if (uds == null) return false;
            DS.Close();
            DS.Open(uds);
            UDS = uds;
            return true;
        }

        public bool LoadUserDefDLL(string dllName)
        {
            if (!IsDataSenderSet) return false;

            var uds = Util.GetInstToAssembly<UserDefSender>(dllName);
            if (uds == null) return false;

            DS.Close();
            DS.Open(uds);
            UDS = uds;
            DLLName = dllName;
            return true;
        }

        public void Start()
        {
            if (!IsDataSenderSet) return;
            if (!DS.IsOpen) return;
            if (!IsUserSenderSet) return;
            if (!UDS.CheckBeforeRun()) return;

            DS.ConfigSeries(Series);
            DS.Start();
        }

        public void Stop()
        {
            if (!IsDataSenderSet) return;
            DS.Stop();
        }

        public void ClearAndAddSeries(int count)
        {
            if (count < 0) return;
            if (count > Define.MaxSeriesNum) return;
            Series.Clear();

            for (int i = 0; i < count; i++)
            {
                Series.Add(Activator.CreateInstance(typeof(TSeries)) as ISeriesData);
            }
        }

        public void ClearSeriesData()
        {
            foreach (var s in Series)
            {
                s.Clear();
            }
        }
    }

    public sealed class ChartProxy<TSender, TSeries> : ChartProxy<TSeries>
        where TSender : IDataSender
        where TSeries : ISeriesData
    {
        static ChartProxy<TSender, TSeries> inst;
        public static new ChartProxy<TSender, TSeries> Inst => inst ?? (inst = new ChartProxy<TSender, TSeries>());

        public override IDataSender DS
        {
            get
            {
                if (ds == null)
                {
                    ds = Activator.CreateInstance(typeof(TSender)) as IDataSender;
                }
                return ds;
            }
        }        
    }      
}
