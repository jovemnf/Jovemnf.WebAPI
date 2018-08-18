using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClientCA
{
    public class ProxyProhibited: Exception
    {

        public ProxyProhibited()
        {

        }

        public ProxyProhibited(string message) : base(message)
        {

        }

        public ProxyProhibited(string message, Exception inner) : base(message, inner)
        {

        }

    }
}