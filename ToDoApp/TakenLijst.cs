using System.Text;

namespace ToDoApp {
    internal class TakenLijst {

        private const string DATABANKPAD = "databank.txt";
        private int aantal;
        private int capaciteit;
        private string[] databank;
        private string[] takenBeschrijvingen;
        private string[] takenDeadlines;
        private string[] takenVoltooid;

        public int Aantal { get => aantal; }

        public TakenLijst() {
            // Haal data op uit databank.txt en parse dit naar 3 arrays
            if (!File.Exists(DATABANKPAD)) File.WriteAllLines(DATABANKPAD, new string[3]);
            databank = File.ReadAllLines(DATABANKPAD);
            takenBeschrijvingen = databank[0].Split("|");
            takenDeadlines = databank[1].Split("|");
            takenVoltooid = databank[2].Split("|");

            capaciteit = takenBeschrijvingen.Length;
            aantal = capaciteit;
        }
        // Geeft een string array weer met de 3 eigenschappen van de taak
        public string[] HaalTaakOpIndex(int index) => [takenBeschrijvingen[index], takenDeadlines[index], takenVoltooid[index]];
        // Scrijft de taken weg naar databanken.txt -------------------------------------------
        public void SchrijfTakenWeg() {
            databank[0] = string.Join("|", takenBeschrijvingen, 0, aantal);
            databank[1] = string.Join("|", takenDeadlines, 0, aantal);
            databank[2] = string.Join("|", takenVoltooid, 0, aantal);
            File.WriteAllLines(DATABANKPAD, databank);
        }
        // Voegt een beschrijving, deadline en voltooide datum toe aan de parralelle arrays op een geven index
        public void VoegTaakInOpIndex(int index, string[] taak) {
            VoegLegeIndexIn(index);
            takenBeschrijvingen[index] = taak[0];
            takenDeadlines[index] = taak[1];
            takenVoltooid[index] = taak[2];
        }
        // Wisselt 2 taken van plaats
        public void WisselTaken(int index1, int index2) {
            (takenBeschrijvingen[index1], takenBeschrijvingen[index2]) = (takenBeschrijvingen[index2], takenBeschrijvingen[index1]);
            (takenDeadlines[index1], takenDeadlines[index2]) = (takenDeadlines[index2], takenDeadlines[index1]);
            (takenVoltooid[index1], takenVoltooid[index2]) = (takenVoltooid[index2], takenVoltooid[index1]);
        }
        // Haalt de datum van vandaag op -------------------------------------------
        public static DateOnly HuidigeDatum() => DateOnly.FromDateTime(DateTime.Now);

        // Vindt de index van het eerste lege item van takenDeadlines 
        public int GeefEersteLegeDeadlineIndex() {
            int index = Array.FindIndex(takenDeadlines, i => !String.IsNullOrWhiteSpace(i));
            if (index < 0) index = 0;
            return index;
        }
        // Vindt de index van het eerste lege item van takenVoltooid 
        public int GeefEersteLegeVoltooideIndex() {
            int index = Array.FindIndex(takenVoltooid, i => !String.IsNullOrWhiteSpace(i));
            if (index < 0) index = 0;
            return index;
        }
        // Druk de lijst van taken gegroepeerd af op de console -------------------------------------------
        public void PrintTakenLijst(string modus) {
            StringBuilder takenOplijsting = new("\nTaken:\n");
            StringBuilder takenDeadlineOplijsting = new("\nTaken met deadline:\n");
            StringBuilder takenVoltooidOplijsting = new("\nVoltooide taken:\n");

            for (int i = 0; i < aantal; i++) {
                bool heeftBeschrijving = !string.IsNullOrWhiteSpace(takenBeschrijvingen[i]);
                bool heeftDeadline = !string.IsNullOrWhiteSpace(takenDeadlines[i]);
                bool isVoltooid = !string.IsNullOrWhiteSpace(takenVoltooid[i]);
                string prefix = (modus == "normaal") ? " *" : $"[{i + 1,2}]";
                string prefixVoltooid = (modus != "verwijderen") ? " *" : $"[{i + 1,2}]";

                // Compileer de taken zonder extra info
                if (heeftBeschrijving && !heeftDeadline && !isVoltooid) {
                    takenOplijsting.AppendLine($"{prefix}   {takenBeschrijvingen[i]}");
                }
                // Compileer de taken met deadline en zonder voltooidatum
                else if (heeftDeadline && !isVoltooid) {
                    DateOnly taakDeadline = DateOnly.ParseExact(takenDeadlines[i], "yyyy-MM-dd");
                    int dagenTotDeadline = taakDeadline.DayNumber - HuidigeDatum().DayNumber;
                    takenDeadlineOplijsting.AppendLine($"{prefix}   deadline: {takenDeadlines[i]} (nog: {dagenTotDeadline} dagen) | {takenBeschrijvingen[i]}");
                }
                // Compileer de taken zonder deadline en met voltooidatum
                else if (!heeftDeadline && isVoltooid) {
                    takenVoltooidOplijsting.AppendLine($"{prefixVoltooid}   voltooid op: {takenVoltooid[i]} | {takenBeschrijvingen[i]}");
                }
                // Compileer de taken met deadline en met voltooidatum
                else if (heeftDeadline && isVoltooid) {
                    takenVoltooidOplijsting.AppendLine($"{prefixVoltooid}   voltooid op: {takenVoltooid[i]} | deadline: {takenDeadlines[i]} | {takenBeschrijvingen[i]}");
                }
            }
            // Print alles af
            Console.Clear();
            Console.WriteLine("***************************** TODO APP by Arnout *****************************\n" +
                             $"Aantal taken : {aantal}, Lengte array : {takenBeschrijvingen.Length}\n" +
                             $"{takenOplijsting}{takenDeadlineOplijsting}{takenVoltooidOplijsting}");
        }

        public void VoegLegeIndexIn(int index) {
            if (aantal + 1 > capaciteit) {// Verdubbelt de grootte van de arrays als er plaats te kort is
                capaciteit *= 2;
                Array.Resize(ref takenBeschrijvingen, capaciteit);
                Array.Resize(ref takenDeadlines, capaciteit);
                Array.Resize(ref takenVoltooid, capaciteit);
            }
            for (int i = aantal; i > 0; i--) {
                takenBeschrijvingen[i] = takenBeschrijvingen[i - 1];
                takenDeadlines[i] = takenDeadlines[i - 1];
                takenVoltooid[i] = takenVoltooid[i - 1];
            }
            aantal++;
        }

        public void VerwijderIndex(int index) {
            for (int i = index; i < aantal - 1; i++) {
                takenBeschrijvingen[i] = takenBeschrijvingen[i + 1];
                takenDeadlines[i] = takenDeadlines[i + 1];
                takenVoltooid[i] = takenVoltooid[i + 1];
            }
            takenBeschrijvingen[^1] = default!;
            takenDeadlines[^1] = default!;
            takenVoltooid[^1] = default!;
            aantal--;
        }
    }
}
