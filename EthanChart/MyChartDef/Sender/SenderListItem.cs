using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthChartDef.Sender
{
    public class SenderListItem
    {
        SenderListItem[] items;
        public int Index { get; private set; }
        public int Count => items != null ? items.Length : 0;

        public SenderListItem this[int inx]
        {
            get => items[inx];
            set => items[inx] = value;
        }
        Action<double> act;

        public SenderListItem(int index, Action<double> act)
        {
            this.Index = index;
            this.act = act;
        }

        public SenderListItem(int index, int itemCount)
        {
            this.Index = index;
            items = new SenderListItem[itemCount];
        }

        public void Set(double val)
        {
            if (act != null)
            {
                act(val);
            }
        }
    }
}
