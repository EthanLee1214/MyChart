using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EthChartDef.App;
using EthChartDef.User;

namespace EthChartDef.Sender
{
    public interface IDataSender
    {
        bool IsOpen { get; }
        bool ConfigSeries(IList<ISeriesData> seriesList);
        bool Open(UserDefSender userSender);
        bool Start(ushort updateTime_ms = 50);
        void Stop();
        void Close();
    }
}
