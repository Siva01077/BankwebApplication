using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginsModel
{
   public  class TransactionModel
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Reference { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal BalanceAfter { get; set; }

        public string SenderName { get; set; }
        public string RecieverName { get; set; }
        public long SenderAccount { get; set; }
        public long ReceiverAccount { get; set; }
    }
}
