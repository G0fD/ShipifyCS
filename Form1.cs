using endproject.Proxy;
using static System.Net.Mime.MediaTypeNames;

namespace endproject
{
    public partial class Form1 : Form
    {
        Posrednik proxy = new Posrednik();
        DBUsers user;
        DatabaseClass db;
        Matcher matcher = new Matcher();
        List<int> matchedId= new List<int>();
        public Form1()
        {
            InitializeComponent();
            db = DatabaseClass.GetInstance();
        }
        
        private void LoadMatches()
        {
            foreach(User au in db.GetUsersMatches(this.user))
            {
                matchedId.Add(au.Id);
                listBox1.Items.Add(au.Name.GetName() + " " + au.Surname);
            }
        }

        private void RefreshAll()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            fileTitleText.Clear();
            LoadMatches(); 
        }

        private async void signupButton_Click(object sender, EventArgs e)
        {
            if (proxy.CheckIfCorrect(signNameText.Text, signSurnameText.Text, signPhoneText.Text, signGenderText.Text))
            {
                if (db.CheckLogin(signUsernameText.Text) == true)
                {
                    int phone = int.Parse(signPhoneText.Text);
                    await proxy.AddNewUserAsync(signNameText.Text, signSurnameText.Text, signUsernameText.Text, signPasswordText.Text, signGenderText.Text, phone);
                    user = db.GetUser(signUsernameText.Text);

                    panel3.BringToFront();
                    LoadMatches();
                }
                else
                {
                    MessageBox.Show("Username is taken", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("You used illegal characters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void signChangeButton_Click(object sender, EventArgs e)
        {
            panel2.BringToFront();
        }

        private void logLoginButton_Click(object sender, EventArgs e)
        {
            if (db.TryToLogin(logUsernameText.Text, logPasswordText.Text))
            {
                user = db.GetUser(logUsernameText.Text);
                if(user.IsAdmin()) MessageBox.Show("Welcome back master", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                panel3.BringToFront();
                LoadMatches();
            }
            else
            {
                MessageBox.Show("The username or password is incorrect ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            User tmp = db.GetUserById(matchedId[listBox1.SelectedIndex]);
            fileTitleText.Text = tmp.Name.GetName() + " " + tmp.Surname + " " + tmp.Phone_number.ToString() + " " + tmp.Sex;

            foreach (Song s in db.GetUserSongs(tmp))
            {
                listBox2.Items.Add(s.Name + " " + s.Author + " " +s.Album);
            }
        }

        private void addFileButton_ClickAsync(object sender, EventArgs e)
        {
            panel4.BringToFront();
        }

        private async void addButtonSubmit_Click(object sender, EventArgs e)
        {
            if(proxy.CheckIfCorrectSong(addTitleText.Text, addAuthorText.Text))
            {
                await proxy.AddSong(addTitleText.Text, addAuthorText.Text,user);
                panel3.BringToFront();
            }
            else
            {
                MessageBox.Show("There is no such song in database ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await matcher.FindMatches();
            RefreshAll();
        }
    }
}