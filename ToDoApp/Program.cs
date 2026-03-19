using System.Globalization;

namespace ToDoApp {
    internal class Program {
        static void Main(string[] args) {
            /* --------------------Regels--------------------
             * Minder dan 250 lijnen
             * Alles in het Nederlands
             * Geen methods maken
             * Informatie bijhouden in paralelle arrays
             * Lijsten opslaan in een tekst bestand, ga er van uit dat het altijd correct is opgeslagen
             * Regelatig comitten naar de repo
             */

            // COLLECT AND PARSE LIST
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

                // PRINT LIST
                string takenOplijsting = "";
                string takenDeadlineOplijsting = "";
                string takenVoltooidOplijsting = "";
                int fsl = 2;

                for (int i = 0; i < aantal; i++) {
                    if (takenDeadline[i].IsWhiteSpace() && takenVoltooid[i].IsWhiteSpace()) {
                        if (modus == "normaal")
                            takenOplijsting += $" *   {takenBeschrijving[i]}\n";
                        else
                            takenOplijsting += $"[{i + 1, 2}]   {takenBeschrijving[i]}\n";
                    }
                    else if (!takenDeadline[i].IsWhiteSpace() && takenVoltooid[i].IsWhiteSpace()) {
                        DateOnly taakDeadline = DateOnly.ParseExact(takenDeadline[i], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                        if (modus == "normaal")
                            takenDeadlineOplijsting += $" *   deadline: {takenDeadline[i]} (nog: {taakDeadline.DayNumber - vandaag.DayNumber} dagen) | {takenBeschrijving[i]}\n";
                        else
                            takenDeadlineOplijsting += $"[{i + 1, 2}]   deadline: {takenDeadline[i]} (nog: {taakDeadline.DayNumber - vandaag.DayNumber} dagen) | {takenBeschrijving[i]}\n";
                    }

                    else if (takenDeadline[i].IsWhiteSpace() && !takenVoltooid[i].IsWhiteSpace()) {
                        takenVoltooidOplijsting += $" *   voltooid op: {takenVoltooid[i]} | {takenBeschrijving[i]}\n";
                    }

                    else if (!takenDeadline[i].IsWhiteSpace() && !takenVoltooid[i].IsWhiteSpace()) {
                        takenVoltooidOplijsting += $" *   voltooid op: {takenVoltooid[i]} | deadline: {takenDeadline[i]} | {takenBeschrijving[i]}\n";
                    }
                }

                Console.Clear();
                Console.WriteLine(
                    "Taken: \n" +
                    takenOplijsting +
                    "\nTaken met deadline: \n" +
                    takenDeadlineOplijsting +
                    "\nVoltooide taken: \n" +
                    takenVoltooidOplijsting
                    );

                // ASK ACTION
                string actie = "";
                if (modus == "normaal") {
                    Console.Write("Acties ([T]oevoegen / [V]oltooien / [D]efinitief verwijderen) ?: ");
                    actie = Console.ReadLine().ToUpper().Trim();
                }

                string taakBeschrijving = "";
                string taakDeadlineString = "";
                string taakVoltooidString = "";

                if (actie == "T") {  // ACTION : ADD --------------------------------------------------------
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

                else if (modus == "voltooien") {  // ACTION : COMPLETE -----------------------------------------

                    bool inputok;
                    int voltooienIndex;
                    do {
                        Console.WriteLine("Geef het nummer van de te voltooien taak : ");
                        inputok = int.TryParse(Console.ReadLine().Trim(), out voltooienIndex);
                        voltooienIndex--;
                    }
                    while (!inputok || voltooienIndex < 0 || voltooienIndex > aantal - 1);

                    string teVoltooienTaakBeschrijving = takenBeschrijving[voltooienIndex];
                    string teVoltooienTaakDeadlineString = takenDeadline[voltooienIndex];
                    string teVoltooienTaakVoltooidString = takenVoltooid[voltooienIndex];

                    for (int i = voltooienIndex; i < aantal - 1; i++) {
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

                else if (modus == "verwijderen") { // ACTION : DELETE -------------------------------------------

                    modus = "normaal";
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

            Console.WriteLine("\nEINDE PROGRAMMA");
            Console.ReadKey();
        }
    }
}
