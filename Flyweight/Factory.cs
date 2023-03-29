using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace endproject.Flyweight
{
    internal class Factory
    {
        private List<Pylek> names = new List<Pylek>();

        public Pylek GetFlyweight(String name)
        {
            foreach(Pylek tmp in names)
            {
                if (tmp.GetName().Equals(name))
                {
                    return tmp;
                }
            }

            Pylek tmp2 = new Pylek(name);
            names.Add(tmp2);
            return names[names.IndexOf(tmp2)];
        }
    }
}
