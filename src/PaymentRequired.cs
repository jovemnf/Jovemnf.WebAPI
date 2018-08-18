using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClientCA
{
    public class PaymentRequired : Exception
    {
        public PaymentRequired()
        {

        }

        public PaymentRequired(string message) : base(message)
        {

        }

        public PaymentRequired(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
