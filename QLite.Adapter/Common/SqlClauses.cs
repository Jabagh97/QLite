using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Adapter.Common
{
    public class SqlClauses
    {
        //public static string OldestPossibleTicketTime =>  "dateadd(hour,-8,getdate())";
        public static string OldestPossibleTicketTime => "DATETIME(DATE(), '-480 minutes')";

        

    }
}
