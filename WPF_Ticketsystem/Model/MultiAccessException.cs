using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Ticketsystem
{
    class MultiAccessException : Exception
    {
        public MultiAccessException(string msg) : base(msg)
        {
            
        }
    }
}
