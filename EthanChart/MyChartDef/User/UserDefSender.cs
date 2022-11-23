using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EthChartDef.Sender;

namespace EthChartDef.User
{
    public abstract class UserDefSender
    {
        public int NameIndex { get; protected set; }

        public int KeyCount => senderItems != null ? senderItems.Count : 0;

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

        public virtual bool CheckBeforeRun() { return true; }

        public virtual void Update()
        {

        }
    }
}
