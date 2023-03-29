using endproject.Flyweight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace endproject
{
    internal abstract class DBUsers
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string BcryptedPassword { get; set; }
        public Pylek Name;
        public string Surname { get; set; }
        public string Sex { get; set; }
        public int Phone_number { get; set; }
        public abstract bool IsAdmin();
    }
}
