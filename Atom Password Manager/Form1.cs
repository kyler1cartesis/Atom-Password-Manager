using System;
using System.Windows.Forms;

namespace Atom_Password_Manager; 
public partial class Form1 : Form {
    public Form1 () {
        InitializeComponent();
    }

    private void Load_Click (object sender, EventArgs e) {
        OpenFileDialog openFileDialog = new OpenFileDialog {
            Filter = $"{ManagerParole.ProgamName} files |*{ManagerParole.Extension}",
            RestoreDirectory = true
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK)
            return;
        ManagerParole.Location = openFileDialog.FileName;
        MainPasswordForm PasswordForm = new MainPasswordForm {
            title = "Enter your master password"
        };
        PasswordForm.ShowDialog();
        if (ManagerParole.MainPassword.Length == 0)
            return;
        if (ManagerParole.OpenAndVerifyDatabase(ManagerParole.Location, ManagerParole.MainPassword)) {
            Close();
            Program.CorrectPassword = true;
        }
        else {
            if (ManagerParole.MainPassword.Length != 0)
                MessageBox.Show("Password is wrong!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


    }

    private void Create_Click (object sender, EventArgs e) {

        MainPasswordForm PasswordForm = new MainPasswordForm {
            title = "Create a master password"
        };
        PasswordForm.ShowDialog();

        if (ManagerParole.MainPassword.Length == 0)
            return;

        ManagerParole.AddExamples(1);

        SaveFileDialog saveFileDialog = new SaveFileDialog {
            Filter = $"{ManagerParole.ProgamName} files |*{ManagerParole.Extension}",
            DefaultExt = ManagerParole.Extension,
            AddExtension = true,
            RestoreDirectory = true
        };

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
            ManagerParole.SaveDatabase(saveFileDialog.FileName, ManagerParole.MainPassword);
    }

    private void Form1_Load (object sender, EventArgs e) {
        label1.Select();
    }
}
