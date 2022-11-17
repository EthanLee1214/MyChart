using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthChartDef
{
    public static class Define
    {
        public const byte MaxItemNum = 3;
        public const byte MaxSeriesNum = 4;
    }

    public interface ISeriesData
    {
        /// <summary>
        /// Length == IDataSenderItem - KeyNames Length,
        /// Array Index == IDataSenderItem - KeyName - ItemList Array Index
        /// </summary>
        int[] Indices { get; }
        Action<double> ValueReceived { get; }        
    }

    // DataSender <-> UserDefSender
    public interface IDataSenderForUser
    {
        void Set(double value, params int[] index);
        SenderListlItem ListItem { get; }
    }

    public class SenderListlItem
    {
        SenderListlItem[] items;
        public int Index { get; private set; }
        public int Count => items != null ? items.Length : 0;

        public SenderListlItem this[int inx]
        {
            get => items[inx];
            set => items[inx] = value;
        }
        Action<double> act;

        public SenderListlItem(int index, Action<double> act)
        {
            this.Index = index;
            this.act = act;
        }

        public SenderListlItem(int index, int itemCount)
        {
            this.Index = index;
            items = new SenderListlItem[itemCount];
        }

        public void Set(double val)
        {
            if (act != null)
            {
                act(val);
            }
        }
    }

    public abstract class UserDefSender
    {
        public int NameIndex { get; protected set; }

        public int KeyCount { get; protected set; }

        public IDataSenderForUser DataSender { protected get; set; }

        /// <summary>
        /// Head Name, Item Names
        /// </summary>
        protected Dictionary<string, string[]> senderItems;

        public string[] KeyNames => senderItems != null ? senderItems.Keys.ToArray() : null;

        public string[] ItemList(string keyName)
        {
            if (string.IsNullOrEmpty(keyName)) return null;

            if (senderItems.ContainsKey(keyName))
            {
                return senderItems[keyName];
            }
            else
            {
                return null;
            }
        }

        public UserDefSender()
        {
            ConfigItem();
        }

        public virtual void ConfigItem()
        {

        }

        public virtual void Update()
        {

        }
    }
}