using endproject.Flyweight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace endproject
{
    internal class User : DBUsers
    {
        public override bool IsAdmin()
        {
            return false;
        }
    }
}
