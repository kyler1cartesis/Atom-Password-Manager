using System.Collections.Generic;

namespace Atom_Password_Manager {
    class Entry {
        public static List<Entry> Entries = new List<Entry>();
        public string email;
        public string parola;
        public string site;
        public string note;

        public Entry (string _email, string _parola, string _site, string _notite) {
            email = _email;
            parola = _parola;
            site = _site;
            note = _notite;
        }

    }
}