using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atom_Password_Manager; 
public partial class MainAtomForm : Form {
    private const string hidden_pwd = "************";
    bool is_visible = false;
    public MainAtomForm () {
        InitializeComponent();
    }

    private void MainAtomForm_Load (object sender, EventArgs e) {
        caractereParolaTextBox.Text = Properties.Settings.Default.caractereParola;
        lungimeParolaTextBox.Text = Properties.Settings.Default.lungimeParola.ToString();
        listView1.FullRowSelect = true;
        listView1.GridLines = true;
        listView1.Columns.Add("Login", 143, HorizontalAlignment.Left);
        listView1.Columns.Add("Password", 122, HorizontalAlignment.Left);
        listView1.Columns.Add("Site", 136, HorizontalAlignment.Left);
        listView1.Columns.Add("Notes", 198, HorizontalAlignment.Left);
        RefreshList(is_visible);
    }

    private void AddButton_Click (object sender, EventArgs e) {
        string login = loginBox.Text;
        string parola = passwordBox.Text;
        string site = siteBox.Text;
        string notita = noteBox.Text;
        if (login == "" && parola == "" && site == "" && notita == "") {
            MessageBox.Show("Please fill all the forms!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        Entry.Entries.Add(new Entry(login, parola, site, notita));
        RefreshList(is_visible);
        loginBox.Text = passwordBox.Text = siteBox.Text = noteBox.Text = "";
    }

    private void DelButton_Click (object sender, EventArgs e) {
        if (listView1.SelectedItems.Count == 0) {
            MessageBox.Show("Please choose an account you want to edit!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        DialogResult res = MessageBox.Show("Are you sure you want to delete the selected data?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (res == DialogResult.Yes) {
            Entry.Entries.RemoveAt(listView1.SelectedItems[0].Index);
            RefreshList(is_visible);
            loginBox.Text = passwordBox.Text = siteBox.Text = noteBox.Text = "";
        }
    }
    private void ReplaceButton_Click (object sender, EventArgs e) {
        if (listView1.SelectedItems.Count == 0) {
            MessageBox.Show("Please choose an account you want to edit!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        Entry.Entries.RemoveAt(listView1.SelectedItems[0].Index);
        string login = loginBox.Text;
        string parola = passwordBox.Text;
        string site = siteBox.Text;
        string notita = noteBox.Text;
        Entry.Entries.Insert(listView1.SelectedItems[0].Index, new Entry(login, parola, site, notita));
        RefreshList(is_visible);
        loginBox.Text = passwordBox.Text = siteBox.Text = noteBox.Text = "";

    }
    private void GenButton_Click (object sender, EventArgs e) {
        passwordBox.Text = ManagerParole.RandomString(ManagerParole.PasswordLength);
    }

    private void ArataParola_Click (object sender, EventArgs e) {
        if (arataParola.Text == "Show") {
            arataParola.Text = "Hide";
            passwordBox.UseSystemPasswordChar = false;
        }
        else {
            arataParola.Text = "Show";
            passwordBox.UseSystemPasswordChar = true;
        }
    }

    private void AscundeColumnPass_Click (object sender, EventArgs e) {
        if (ascundeParole.Text == "  Show passwords") {
            is_visible = true;
            ascundeParole.Text = "  Hide passwords";
        }
        else {
            is_visible = false;
            ascundeParole.Text = "  Show passwords";
        }
        RefreshList(is_visible);

    }

    private void RefreshList (bool replaceparola) {
        listView1.Items.Clear();
        foreach (Entry contCurent in Entry.Entries) {
            string parola;
            if (replaceparola)
                parola = contCurent.parola;
            else
                parola = hidden_pwd;
            ListViewItem Date = new ListViewItem(new string[] {
                contCurent.email,
                parola,
                contCurent.site,
                contCurent.note,
            }) {
                Tag = contCurent.parola
            };
            listView1.Items.Add(Date);
        }
    }

    private void ListView1_SelectedIndexChanged (object sender, EventArgs e) {
        if (listView1.SelectedItems.Count == 0) {
            loginBox.Text = passwordBox.Text = siteBox.Text = noteBox.Text = "";
            return;
        }
        else {
            loginBox.Text = listView1.SelectedItems[0].SubItems[0].Text;
            passwordBox.Text = listView1.SelectedItems[0].Tag.ToString();
            siteBox.Text = listView1.SelectedItems[0].SubItems[2].Text;
            noteBox.Text = listView1.SelectedItems[0].SubItems[3].Text;
        }
    }

    private void SalvareSchimbari_Click (object sender, EventArgs e) {
        ManagerParole.SaveDatabase(ManagerParole.Location, ManagerParole.MainPassword);
        Properties.Settings.Default.caractereParola = ManagerParole.AllowedCharacters;
        Properties.Settings.Default.lungimeParola = ManagerParole.PasswordLength;
        Properties.Settings.Default.Save();
    }

    private void SchimbareParola_Click (object sender, EventArgs e) {
        var Schimbare = new SchimbareParolaForm();
        Schimbare.Show();
    }

    public async void SetClipboardPw () {
        if (listView1.SelectedItems.Count != 0) {
            Clipboard.SetText(listView1.SelectedItems[0].Tag.ToString());
            await Task.Delay(TimeSpan.FromSeconds(ManagerParole.VisibleTime));
            Clipboard.Clear();
        }
    }

    private void listView1_KeyDown (object sender, KeyEventArgs e) {
        if (e.Control && e.KeyCode == Keys.C)
            SetClipboardPw();
    }

    private void caractereParolaTextBox_TextChanged (object sender, EventArgs e) {
        if (lungimeParolaTextBox.Text.Length == 0)
            return;
        ManagerParole.AllowedCharacters = caractereParolaTextBox.Text;
    }

    private void lungimeParolaTextBox_TextChanged (object sender, EventArgs e) {
        if (lungimeParolaTextBox.Text.Length == 0)
            return;
        try {
            ManagerParole.PasswordLength = int.Parse(lungimeParolaTextBox.Text);
            if (ManagerParole.PasswordLength > 1000 || ManagerParole.PasswordLength < 1)
                throw new Exception("Passsword lenght is invalid!");
        }
        catch { MessageBox.Show("Please enter a valid number!"); lungimeParolaTextBox.Text = ""; ManagerParole.PasswordLength = Properties.Settings.Default.lungimeParola; }
    }

    private void listView1_MouseDoubleClick (object sender, MouseEventArgs e) {
        SetClipboardPw();
    }

    private void Complexitate_Click (object sender, EventArgs e) {
        int complexity = 0;
        if (passwordBox.Text.Length >= 4)
            complexity += 10;
        if (passwordBox.Text.Length >= 8)
            complexity += 10;
        if (passwordBox.Text.IndexOfAny("abcdefghijklmnopqrstuvwxyz".ToCharArray()) >= 0)
            complexity += 15;
        if (passwordBox.Text.IndexOfAny("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray()) >= 0)
            complexity += 15;
        if (passwordBox.Text.IndexOfAny("0123456789".ToCharArray()) >= 0)
            complexity += 25;
        if (passwordBox.Text.IndexOfAny("!@#$%^&*()_-=+;[]{}:.,<>/?".ToCharArray()) >= 0)
            complexity += 25;
        complexBar.Value = complexity;
        complexLabel.Text = $"Complexity ({complexity}/100):";
    }
}
