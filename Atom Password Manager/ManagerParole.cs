using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Atom_Password_Manager; 
class ManagerParole {
    public static Random rand = new Random();
    public static string MainPassword = "";
    public static string Extension = ".atom";
    public static string ProgamName = "ATOM";
    public static string Location = "";
    public static int PasswordLength = Properties.Settings.Default.lungimeParola;
    public static int VisibleTime = 5;

    public static void SaveDatabase (string path, string pass) {
        string json = JsonConvert.SerializeObject(Entry.Entries);
        File.WriteAllText(path, json);
        Encryption.EncryptFile(path, pass);
    }

    public static bool OpenAndVerifyDatabase (string path, string parola) {
        var bkCaracterePermise = AllowedCharacters;
        AllowedCharacters = "ABCDEFHIJKLMNOPQRSTUVWXYZ1234567890";

        string tempFilePath = Path.GetTempPath() + $"atomtemp{RandomString(8)}.atom";
        AllowedCharacters = bkCaracterePermise;
        File.Copy(path, tempFilePath);
        try {
            Encryption.DecryptFile(tempFilePath, MainPassword);

            string convertMe = File.ReadAllText(tempFilePath);
            File.Delete(tempFilePath);
            Entry.Entries = JsonConvert.DeserializeObject<List<Entry>>(convertMe);
        }
        catch {
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
            return false;
        }
        return true;
    }
    public static void AddExamples (int nr) {
        for (int i = 0; i < nr; i++) {
            Entry.Entries.Add(new Entry($"email{i}", RandomString(PasswordLength), $"site{i}", "cont exemplu"));
        }
    }
    public static string AllowedCharacters = Properties.Settings.Default.caractereParola;
    public static string RandomString (int length) {
        StringBuilder res = new StringBuilder();

        int randomInteger = rand.Next(0, AllowedCharacters.Length);
        while (0 < length--) {
            res.Append(AllowedCharacters[randomInteger]);
            randomInteger = rand.Next(0, AllowedCharacters.Length);
        }
        return res.ToString();
    }
}
