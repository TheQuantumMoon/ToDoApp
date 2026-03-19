using System.Globalization;

namespace ToDoApp {
    internal class Program {
        static void Main(string[] args) {

            // Haal data op uit databank.txt en parse dit naar 3 arrays -------------------------------------------
            const string DATABANKPAD = "databank.txt";
            if (!File.Exists(DATABANKPAD)) File.WriteAllLines(DATABANKPAD, new string[3]);

            string[] databank = File.ReadAllLines(DATABANKPAD);
            string[] takenBeschrijving = databank[0].Split("|");
            string[] takenDeadline = databank[1].Split("|");
            string[] takenVoltooid = databank[2].Split("|");

            DateOnly vandaag = DateOnly.FromDateTime(DateTime.Now);
            int capaciteit = takenBeschrijving.Length;
            int aantal = capaciteit;
            string modus = "normaal";

            while (true) {
                // Druk de lijst van taken gegroepeerd af op de console -------------------------------------------
                string takenOplijsting = "";
                string takenDeadlineOplijsting = "";
                string takenVoltooidOplijsting = "";

                for (int i = 0; i < aantal; i++) {
                    // Compileer de taken zonder extra info -------------------------------------------
                    if (takenDeadline[i].IsWhiteSpace() && takenVoltooid[i].IsWhiteSpace()) {
                        if (modus == "normaal")
                            takenOplijsting += $" *   {takenBeschrijving[i]}\n";
                        else
                            takenOplijsting += $"[{i + 1, 2}]   {takenBeschrijving[i]}\n";
                    }
                    // Compileer de taken met deadline en zonder voltooidatum -------------------------------------------
                    else if (!takenDeadline[i].IsWhiteSpace() && takenVoltooid[i].IsWhiteSpace()) {
                        DateOnly taakDeadline = DateOnly.ParseExact(takenDeadline[i], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                        if (modus == "normaal")
                            takenDeadlineOplijsting += $" *   deadline: {takenDeadline[i]} (nog: {taakDeadline.DayNumber - vandaag.DayNumber} dagen) | {takenBeschrijving[i]}\n";
                        else
                            takenDeadlineOplijsting += $"[{i + 1, 2}]   deadline: {takenDeadline[i]} (nog: {taakDeadline.DayNumber - vandaag.DayNumber} dagen) | {takenBeschrijving[i]}\n";
                    }
                    // Compileer de taken zonder deadline en met voltooidatum -------------------------------------------
                    else if (takenDeadline[i].IsWhiteSpace() && !takenVoltooid[i].IsWhiteSpace()) {
                        if (modus == "verwijderen")
                        takenVoltooidOplijsting += $"[{i + 1,2}]   voltooid op: {takenVoltooid[i]} | {takenBeschrijving[i]}\n";
                        else 
                            takenVoltooidOplijsting += $" *   voltooid op: {takenVoltooid[i]} | {takenBeschrijving[i]}\n";
                    }
                    // Compileer de taken met deadline en zonder voltooidatum -------------------------------------------
                    else if (!takenDeadline[i].IsWhiteSpace() && !takenVoltooid[i].IsWhiteSpace()) {
                        if (modus == "verwijderen")
                            takenVoltooidOplijsting += $"[{i + 1,2}]   voltooid op: {takenVoltooid[i]} | deadline: {takenDeadline[i]} | {takenBeschrijving[i]}\n";
                        else
                            takenVoltooidOplijsting += $" *   voltooid op: {takenVoltooid[i]} | deadline: {takenDeadline[i]} | {takenBeschrijving[i]}\n";
                    }
                }
                Console.Clear();
                Console.WriteLine( // Print alle taken per groep
                    "Taken: \n" +
                    takenOplijsting +
                    "\nTaken met deadline: \n" +
                    takenDeadlineOplijsting +
                    "\nVoltooide taken: \n" +
                    takenVoltooidOplijsting
                    );

                // Vraagt de actie aan de gebruiker -------------------------------------------
                string actie = "";
                if (modus == "normaal") {
                    Console.Write("Acties ([T]oevoegen / [V]oltooien / [D]efinitief verwijderen) ?: ");
                    actie = Console.ReadLine().ToUpper().Trim();
                }
                string taakBeschrijving = "";
                string taakDeadlineString = "";
                string taakVoltooidString = "";

                // ACTIE : toevoegen -------------------------------------------
                if (actie == "T") {
                    do {
                        Console.Write("Geef de taakberschrijving : ");
                        taakBeschrijving = Console.ReadLine().Trim();
                    }
                    while (taakBeschrijving.IsWhiteSpace());

                    bool inputOk;
                    do {
                        Console.Write("Geef de deadline (yyyy-MM-dd) of [S]kip : ");
                        string inputString = Console.ReadLine().Trim();
                        if (inputString.ToUpper() == "S") break;
                        inputOk = DateOnly.TryParseExact(inputString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly taakDeadline);
                        if (inputOk) taakDeadlineString = taakDeadline.ToString("yyyy-MM-dd");
                    }
                    while (!inputOk);

                    Array.Resize(ref takenBeschrijving, aantal + 1);
                    Array.Resize(ref takenDeadline, aantal + 1);
                    Array.Resize(ref takenVoltooid, aantal + 1);

                    if (taakDeadlineString.IsWhiteSpace()) {
                        for (int i = aantal; i > 0; i--) {
                            takenBeschrijving[i] = takenBeschrijving[i - 1];
                            takenDeadline[i] = takenDeadline[i - 1];
                            takenVoltooid[i] = takenVoltooid[i - 1];
                        }
                        takenBeschrijving[0] = taakBeschrijving;
                        takenDeadline[0] = taakDeadlineString;
                        takenVoltooid[0] = taakVoltooidString;
                        aantal++;

                    }
                    else {
                        int firstDeadlineIndex = Math.Abs(Array.FindIndex(takenDeadline, i => !i.IsWhiteSpace()));
                        for (int i = aantal; i >= firstDeadlineIndex; i--) {
                            takenBeschrijving[i] = takenBeschrijving[i - 1];
                            takenDeadline[i] = takenDeadline[i - 1];
                            takenVoltooid[i] = takenVoltooid[i - 1];
                        }
                        takenBeschrijving[firstDeadlineIndex] = taakBeschrijving;
                        takenDeadline[firstDeadlineIndex] = taakDeadlineString;
                        takenVoltooid[firstDeadlineIndex] = taakVoltooidString;
                        aantal++;
                    }

                }

                // ACTIE : voltooien -------------------------------------------
                else if (modus == "voltooien") {
                    bool inputOk;
                    int teVoltooienIndex;
                    do {
                        Console.WriteLine("Geef het nummer van de te voltooien taak : ");
                        inputOk = int.TryParse(Console.ReadLine().Trim(), out teVoltooienIndex);
                        teVoltooienIndex--;
                    }
                    while (!inputOk || teVoltooienIndex < 0 || teVoltooienIndex > aantal - 1);

                    string teVoltooienTaakBeschrijving = takenBeschrijving[teVoltooienIndex];
                    string teVoltooienTaakDeadlineString = takenDeadline[teVoltooienIndex];
                    string teVoltooienTaakVoltooidString = takenVoltooid[teVoltooienIndex];

                    for (int i = teVoltooienIndex; i < aantal - 1; i++) {
                        takenBeschrijving[i] = takenBeschrijving[i + 1];
                        takenDeadline[i] = takenDeadline[i + 1];
                        takenVoltooid[i] = takenVoltooid[i + 1];
                    }
                    takenBeschrijving[aantal - 1] = teVoltooienTaakBeschrijving;
                    takenDeadline[aantal - 1] = teVoltooienTaakDeadlineString;
                    takenVoltooid[aantal - 1] = vandaag.ToString("yyyy-MM-dd");

                    modus = "normaal";
                }
                else if (actie == "V") {
                    modus = "voltooien";
                    continue;
                }

                // ACTIE : verwijderen -------------------------------------------
                else if (modus == "verwijderen") {
                    bool inputok;
                    int teVerwijderenIndex;
                    do {
                        Console.WriteLine("Geef het nummer van de te verwijderen taak : ");
                        inputok = int.TryParse(Console.ReadLine().Trim(), out teVerwijderenIndex);
                        teVerwijderenIndex--;
                    }
                    while (!inputok || teVerwijderenIndex < 0 || teVerwijderenIndex > aantal - 1);

                    for (int i = teVerwijderenIndex; i < aantal - 1; i++) {
                        takenBeschrijving[i] = takenBeschrijving[i + 1];
                        takenDeadline[i] = takenDeadline[i + 1];
                        takenVoltooid[i] = takenVoltooid[i + 1];
                    }
                    Array.Resize(ref takenBeschrijving, aantal - 1);
                    Array.Resize(ref takenDeadline, aantal - 1);
                    Array.Resize(ref takenVoltooid, aantal - 1);
                    aantal--;

                    modus = "normaal";
                    continue;
                }
                else if (actie == "D") {
                    modus = "verwijderen";
                    continue;
                }

                // Scrijft de taken weg naar databanken.txt
                databank[0] = string.Join("|", takenBeschrijving);
                databank[1] = string.Join("|", takenDeadline);
                databank[2] = string.Join("|", takenVoltooid);
                File.WriteAllLines(DATABANKPAD, databank);
            }
        }
    }
}
