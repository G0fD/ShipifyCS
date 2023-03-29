using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace endproject.Proxy
{
    internal class Posrednik : IProxy
    {
        DatabaseClass db = DatabaseClass.GetInstance();
        private bool CheckIfCorrrectString(String name)
        {
            return Regex.IsMatch(name, @"^[\p{L}]+$");
        }

        private String FixString(String name)
        {
            String capName = name.Substring(0,1).ToUpper() + name.Substring(1).ToLower();
            return capName;
        }

        private bool CheckIfPhone(String phone)
        {
            return Regex.IsMatch(phone, @"^[0-9]+$");
        }

        public bool CheckIfCorrect(String name, String surname, String phone, String sex)
        {
            if (!CheckIfCorrrectString(name)) return false;
            if (!CheckIfCorrrectString(surname)) return false;
            if (!CheckIfCorrrectString(sex)) return false;
            if (!CheckIfPhone(phone)) return false;

            return true;
        }
        public async Task AddNewUserAsync(string name, string surname, string username, string password, string sex, int phone_number)
        {
            string bcrypted = BCrypt.Net.BCrypt.HashPassword(password);
            surname = FixString(surname);
            name = FixString(name);
            sex = FixString(sex);

            await db.AddNewUserAsync(name, surname, username, bcrypted, sex, phone_number);
        }

        public bool CheckIfCorrectSong(String name, String author)
        {
            Song song = db.GetSong(name, author);
            if(song.Author is null) return false;
            return true;
        }

        public async Task AddSong(String name, String author, DBUsers user)
        {
            Song song = db.GetSong(name, author);
            await db.AddLikedSong(user, song);
        }
    }
}
