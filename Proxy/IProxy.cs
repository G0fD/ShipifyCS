using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace endproject.Proxy
{
    internal interface IProxy
    {
        public async Task AddNewUserAsync(string name, string surname, string username, string password, string sex, int phone_number) { }
    }
}