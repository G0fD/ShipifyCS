using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace endproject
{
    internal class Song
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Album { get; set; }

        public override string ToString()
        {
            return Name+ " " +Author+ " " + Album;
        }
    }
}
