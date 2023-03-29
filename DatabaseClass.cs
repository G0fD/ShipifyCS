using System;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Linq;
using Microsoft.VisualBasic.ApplicationServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using endproject.Flyweight;
using endproject.Proxy;

namespace endproject
{
    internal class DatabaseClass : IProxy
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=admin;Database=project";
        private NpgsqlConnection conn;
        private static DatabaseClass _instance;
        private Factory factory = new Factory();

        private DatabaseClass()
        {
            conn = new NpgsqlConnection(connectionString);
            conn.Open();
        }

        public static DatabaseClass GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DatabaseClass();
            }
            return _instance;
        }

        public async Task AddNewUserAsync(string name, string surname, string username, string password, string sex, int phone_number)
        {
            string commandText = $"INSERT INTO {"public.\"user\""} (name, surname, username, password, sex, phone_number) VALUES (@name, @surname, @username, @password, @sex, @phone_number)";
            await using (var cmd = new NpgsqlCommand(commandText, conn))
            {
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("surname", surname);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                cmd.Parameters.AddWithValue("sex", sex);
                cmd.Parameters.AddWithValue("phone_number", phone_number);

                await cmd.ExecuteNonQueryAsync();
            }

        }

        public bool CheckLogin(string username)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.\"user\" WHERE \"username\" = ($1);", conn)
            {
                Parameters = {
                        new() {Value = username}
                    }
            };
            NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Close();
                return false;
            }
            reader.Close();

            return true;
        }

        public DBUsers GetUser(string username)
        {
            DBUsers user = new User();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.\"user\" WHERE \"username\" = ($1);", conn)
            {
                Parameters = {
                        new() {Value = username}
                    }
            };
            NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                user = new User
                {
                    Id = reader.GetInt32(0),
                    Username =  reader.GetString(1),
                    BcryptedPassword = reader.GetString(2),
                    Name = factory.GetFlyweight(reader.GetString(3)),
                    Surname = reader.GetString(4),
                    Sex = reader.GetString(5),
                    Phone_number = reader.GetInt32(6)
                };
            }
            reader.Close();

            if (user.Id.Equals(1)) return new Admin(user);

            return user;
        }

        public User GetUserById(int id)
        {
            User user = new User();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM public.\"user\" WHERE id_user = ($1);", conn)
            {
                Parameters = {
                        new() {Value = id}
                    }
            };
            NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                user = new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    BcryptedPassword = reader.GetString(2),
                    Name = factory.GetFlyweight(reader.GetString(3)),
                    Surname = reader.GetString(4),
                    Sex = reader.GetString(5),
                    Phone_number = reader.GetInt32(6)
                };
            }
            reader.Close();

            return user;
        }

        public Song GetSong(string name, string author)
        {
            Song song = new Song();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM song WHERE name = ($1) and author = ($2);", conn)
            {
                Parameters = {
                        new() {Value = name},
                        new() {Value = author}
                    }
            };

            NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                song = new Song
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Author = reader.GetString(2),
                    Album = reader.GetString(3)

                };
            }
            reader.Close();

            return song;
        }

        public Song GetSongById(int id)
        {
            Song song = new Song();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM song WHERE id_song = ($1);", conn)
            {
                Parameters =
                {
                    new() {Value = id}
                }
            };
            NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                song = new Song
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Author = reader.GetString(2),
                    Album = reader.GetString(3)

                };
            }
            reader.Close();

            return song;
        }

        public List<Song> GetUserSongs(User user)
        {
            List<int> list = new List<int>();
            NpgsqlCommand cmd = new NpgsqlCommand("select id_song from liked_by where id_user=($1) ", conn)
            {
                Parameters =
                {
                    new() {Value = user.Id}
                }
            };
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }
            reader.Close();

            List<Song> songs = new List<Song>();
            foreach (int id in list)
            {
                Song song = GetSongById(id);
                songs.Add(song);
            }
            return songs;
        }

        public List<int> GetSongGenresId(Song song)
        {
            List<int> genres = new List<int>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id_genre from song_genres WHERE id_song=($1);", conn)
            {
                Parameters =
                {
                    new() {Value = song.Id}
                }
            };

            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                genres.Add(reader.GetInt32(0));
            }
            reader.Close();

            return genres;
        }

        public List<User> GetUniqueUsersWhoLiked()
        {
            List<int> tmp = new List<int>();
            NpgsqlCommand cmd = new NpgsqlCommand("select id_user from liked_by", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tmp.Add(reader.GetInt32(0));
            }
            reader.Close();

            List<int> list = tmp.Distinct().ToList();
            List<User> users = new List<User>();
            foreach (int id in list)
            {
                User user = GetUserById(id);
                users.Add(user);
            }

            return users;
        }

        public List<List<int>> GetMatches()
        {
            List<List<int>> tmp = new List<List<int>>();

            NpgsqlCommand cmd = new NpgsqlCommand("SELECT * from matches", conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.IsDBNull(0)) { reader.Close(); return tmp; }
                List<int> match = new List<int>();
                match.Add(reader.GetInt32(0));
                match.Add(reader.GetInt32(1));

                tmp.Add(match);
            }
            reader.Close();

            return tmp;
        }

        public List<User> GetUsersMatches(DBUsers user)
        {
            List<int> usersId = new List<int>();
            List<User> users = new List<User>();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT id_user2 from matches where id_user1 = ($1)", conn)
            {
                Parameters =
                {
                    new() {Value = user.Id}
                }
            };
            NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.IsDBNull(0)) { reader.Close(); return users; }
                usersId.Add(reader.GetInt32(0));
            }
            reader.Close();

            cmd = new NpgsqlCommand("SELECT id_user1 from matches where id_user2 = ($1)", conn)
            {
                Parameters =
                {
                    new() {Value = user.Id}
                }
            };
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.IsDBNull(0)) { reader.Close(); return users; }
                usersId.Add(reader.GetInt32(0));
            }
            reader.Close();

            foreach(int id in usersId)
            {
                users.Add(GetUserById(id));
            }
            return users;
        }

        public bool TryToLogin(string username, string password)
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT password FROM public.\"user\" WHERE \"username\" = ($1);", conn)
            {
                Parameters = {
                        new() {Value = username}
                    }
            };
            NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                if (BCrypt.Net.BCrypt.Verify(password, reader.GetString(0)))
                {
                    reader.Close();
                    return true;
                }
            }
            reader.Close();
            return false;
        }

        public async Task AddLikedSong(DBUsers user, Song song)
        {
            string commandText = $"INSERT INTO {"liked_by"} (id_user, id_song) VALUES (@id_user, @id_song)";
            await using (var cmd = new NpgsqlCommand(commandText, conn))
            {
                cmd.Parameters.AddWithValue("id_user", user.Id);
                cmd.Parameters.AddWithValue("id_song", song.Id);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task AddMatch(int id1, int id2)
        {
            string commandText = $"INSERT INTO {"matches"} (id_user1, id_user2) VALUES (@id_user1, @id_user2)";
            await using (var cmd = new NpgsqlCommand(commandText, conn))
            {
                cmd.Parameters.AddWithValue("id_user1", id1);
                cmd.Parameters.AddWithValue("id_user2", id2);

                await cmd.ExecuteNonQueryAsync();
            }
        }

    }
}