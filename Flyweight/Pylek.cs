using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace endproject.Flyweight
{
    internal class Pylek
    {
        private String name;

        public Pylek(String name)
        {
            this.name = name;
        }

        public String GetName()
        {
            return name;
        }
    }
}
