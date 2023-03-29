using endproject.Flyweight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace endproject
{
    internal class Admin : Decorator
    {
        public Admin(DBUsers dbusers) : base(dbusers)
        {
            this.Id = dbusers.Id;
            this.Username = dbusers.Username;
            this.BcryptedPassword = dbusers.BcryptedPassword;
            this.Name = dbusers.Name;
            this.Surname = dbusers.Surname;
            this.Sex = dbusers.Sex;
            this.Phone_number = dbusers.Phone_number;
    }

        public override bool IsAdmin()
        {
            return true;
        }
    }
}