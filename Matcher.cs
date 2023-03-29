using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace endproject
{
    internal class Matcher
    {
        DatabaseClass db;
        Dictionary<int, List<int>> usergenres = new Dictionary<int,List<int>>();
        public Matcher()
        {
            db = DatabaseClass.GetInstance();
        }

        private async Task TryAddAsync(int id1, int id2)
        {
            List<int> tmp = new List<int>();
            tmp.Add(id1);
            tmp.Add(id2);
            tmp.Sort();

            int i = 0;
            foreach (List<int> l in db.GetMatches())
            {
                l.Sort();
                if (l[0] == tmp[0] && l[1] == tmp[1]) i++;
            }

            if (i == 0) await db.AddMatch(tmp[0], tmp[1]);
        }

        public async Task FindMatches()
        {
            Dictionary<int, int> test = new Dictionary<int, int>();
            int x = 0;

            foreach (User au in db.GetUniqueUsersWhoLiked())
            {
                //au - konkretny użytkownik
                //potrzeba listy list id gatunków słuchanych przez tego użytkownika
                List<List<int>> gatunki = new List<List<int>>();
                foreach (Song song in db.GetUserSongs(au))
                {
                    gatunki.Add(db.GetSongGenresId(song));
                }
                //lista ze wszystkimi gatunkami (mogą się powtarzać)
                List<int> finalList = gatunki.SelectMany(x => x).ToList();
                //bez duplikatów
                finalList = finalList.Distinct().ToList();

                usergenres[au.Id] = finalList;
                test[x] = au.Id;
                x++;
            }

            for(int i = 0; i < usergenres.Count(); i++)
            {
                for (int j = i+1; j < usergenres.Count(); j++)
                {
                    var CommonList = usergenres[test[i]].Intersect(usergenres[test[j]]);
                    if (CommonList.Count() >= (usergenres[test[i]].Count())/2 && CommonList.Count() >= (usergenres[test[j]].Count()) / 2)
                    {
                        await TryAddAsync(test[i], test[j]);
                    }
                }
                
            }
        }
    }
}
