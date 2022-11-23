using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthChartDef.Sender
{
    // DataSender <-> UserDefSender
    public interface IDataSenderForUser
    {   
        SenderListItem ListItem { get; }
        void Set(double value, params int[] index);
    }
}
