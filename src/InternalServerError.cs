using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jovemnf.WebAPI
{
    public class InternalServerError : Exception
    {

        public InternalServerError()
        {

        }

        public InternalServerError(string message) : base(message)
        {

        }

        public InternalServerError(string message, Exception inner) : base(message, inner)
        {

        }

    }
}
