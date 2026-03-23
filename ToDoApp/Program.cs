using System.Globalization;
using System.Text;

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

            int capaciteit = takenBeschrijving.Length;
            int aantal = capaciteit;
            string modus = "normaal";

            while (true) {
                DateOnly vandaag = DateOnly.FromDateTime(DateTime.Now);

                // Druk de lijst van taken gegroepeerd af op de console -------------------------------------------
                StringBuilder takenOplijsting = new("\nTaken:\n");
                StringBuilder takenDeadlineOplijsting = new("\nTaken met deadline:\n");
                StringBuilder takenVoltooidOplijsting = new("\nVoltooide taken:\n");

                for (int i = 0; i < aantal; i++) {
                    bool heeftBeschrijving = !string.IsNullOrWhiteSpace(takenBeschrijving[i]);
                    bool heeftDeadline = !string.IsNullOrWhiteSpace(takenDeadline[i]);
                    bool isVoltooid = !string.IsNullOrWhiteSpace(takenVoltooid[i]);
                    string prefix = (modus == "normaal") ? " *" : $"[{i + 1,2}]";
                    string prefixVoltooid = (modus != "verwijderen") ? " *" : $"[{i + 1,2}]";

                    // Compileer de taken zonder extra info -------------------------------------------
                    if (heeftBeschrijving && !heeftDeadline && !isVoltooid) {
                        takenOplijsting.AppendLine($"{prefix}   {takenBeschrijving[i]}");
                    }
                    // Compileer de taken met deadline en zonder voltooidatum -------------------------------------------
                    else if (heeftDeadline && !isVoltooid) {
                        DateOnly taakDeadline = DateOnly.ParseExact(takenDeadline[i], "yyyy-MM-dd");
                        int dagenTotDeadline = taakDeadline.DayNumber - vandaag.DayNumber;
                        takenDeadlineOplijsting.AppendLine($"{prefix}   deadline: {takenDeadline[i]} (nog: {dagenTotDeadline} dagen) | {takenBeschrijving[i]}");
                    }
                    // Compileer de taken zonder deadline en met voltooidatum -------------------------------------------
                    else if (!heeftDeadline && isVoltooid) {
                        takenVoltooidOplijsting.AppendLine($"{prefixVoltooid}   voltooid op: {takenVoltooid[i]} | {takenBeschrijving[i]}");
                    }
                    // Compileer de taken met deadline en met voltooidatum -------------------------------------------
                    else if (heeftDeadline && isVoltooid) {
                        takenVoltooidOplijsting.AppendLine($"{prefixVoltooid}   voltooid op: {takenVoltooid[i]} | deadline: {takenDeadline[i]} | {takenBeschrijving[i]}");
                    }
                }
                Console.Clear();
                Console.WriteLine("***************************** TODO APP by Arnout *****************************\n" +
                                 $"Aantal taken : {aantal}, Lengte array : {takenBeschrijving.Length}\n" +
                                 $"{takenOplijsting}{takenDeadlineOplijsting}{takenVoltooidOplijsting}");

                // Vraagt de actie aan de gebruiker -------------------------------------------
                string actie = "";
                if (modus == "normaal") {
                    Console.Write("Acties ([T]oevoegen / [V]oltooien / [D]efinitief verwijderen) ?: ");
                    actie = Console.ReadLine()!.ToUpper().Trim();
                }
                string taakBeschrijving = "";
                string taakDeadlineString = "";
                string taakVoltooidString = "";

                // ACTIE : toevoegen -------------------------------------------
                if (actie == "T") {
                    do {
                        Console.Write("Geef de taakbeschrijving (max 30 karakters) : ");
                        taakBeschrijving = Console.ReadLine()!.Trim();
                    }
                    while (string.IsNullOrWhiteSpace(taakBeschrijving) || taakBeschrijving.Length > 30);

                    bool inputOk;
                    do {
                        Console.Write("Geef de deadline (yyyy-MM-dd) of [S]kip : ");
                        string inputString = Console.ReadLine()!.Trim();
                        if (inputString.ToUpper() == "S") break;
                        inputOk = DateOnly.TryParseExact(inputString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly taakDeadline);
                        if (inputOk) taakDeadlineString = taakDeadline.ToString("yyyy-MM-dd");
                    }
                    while (!inputOk);

                    if (aantal + 1 > capaciteit) { // Verdubbelt de grootte van de arrays als er plaats te kort is
                        capaciteit *= 2;
                        Array.Resize(ref takenBeschrijving, capaciteit);
                        Array.Resize(ref takenDeadline, capaciteit);
                        Array.Resize(ref takenVoltooid, capaciteit);
                    }

                    if (string.IsNullOrWhiteSpace(taakDeadlineString)) { // Plaats de taak in het begin van de lijst als hij geen deadline bevat
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
                    else { // Plaats de taak in het begin van de lijst van taken met een deadline als hij een deadline bevat
                        int eersteDeadlineIndex = Array.FindIndex(takenDeadline, i => !String.IsNullOrWhiteSpace(i));
                        if (eersteDeadlineIndex < 0) eersteDeadlineIndex = 0;

                        for (int i = aantal; i > eersteDeadlineIndex; i--) {
                            takenBeschrijving[i] = takenBeschrijving[i - 1];
                            takenDeadline[i] = takenDeadline[i - 1];
                            takenVoltooid[i] = takenVoltooid[i - 1];
                        }
                        takenBeschrijving[eersteDeadlineIndex] = taakBeschrijving;
                        takenDeadline[eersteDeadlineIndex] = taakDeadlineString;
                        takenVoltooid[eersteDeadlineIndex] = taakVoltooidString;
                        aantal++;
                    }
                }
                // ACTIE : voltooien -------------------------------------------
                else if (modus == "voltooien") {
                    bool inputOk;
                    int teVoltooienIndex;
                    do {
                        Console.Write("Geef het nummer van de te voltooien taak : ");
                        inputOk = int.TryParse(Console.ReadLine()!.Trim(), out teVoltooienIndex);
                        teVoltooienIndex--;
                    }
                    while (!inputOk || teVoltooienIndex < 0 || teVoltooienIndex > aantal - 1);

                    string teVoltooienTaakBeschrijving = takenBeschrijving[teVoltooienIndex];
                    string teVoltooienTaakDeadlineString = takenDeadline[teVoltooienIndex];
                    string teVoltooienTaakVoltooidString = takenVoltooid[teVoltooienIndex];

                    int eersteVoltooiIndex = Array.FindIndex(takenVoltooid, i => !String.IsNullOrWhiteSpace(i));
                    if (eersteVoltooiIndex < 0) eersteVoltooiIndex = aantal;

                    for (int i = teVoltooienIndex; i < eersteVoltooiIndex; i++) { // Plaats de taak in het begin van de lijst van voltooide taken
                        takenBeschrijving[i] = takenBeschrijving[i + 1];
                        takenDeadline[i] = takenDeadline[i + 1];
                        takenVoltooid[i] = takenVoltooid[i + 1];
                    }
                    takenBeschrijving[eersteVoltooiIndex - 1] = teVoltooienTaakBeschrijving;
                    takenDeadline[eersteVoltooiIndex - 1] = teVoltooienTaakDeadlineString;
                    takenVoltooid[eersteVoltooiIndex - 1] = vandaag.ToString("yyyy-MM-dd");

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
                        Console.Write("Geef het nummer van de te verwijderen taak : ");
                        inputok = int.TryParse(Console.ReadLine()!.Trim(), out teVerwijderenIndex);
                        teVerwijderenIndex--;
                    }
                    while (!inputok || teVerwijderenIndex < 0 || teVerwijderenIndex > aantal - 1);

                    for (int i = teVerwijderenIndex; i < aantal - 1; i++) {
                        takenBeschrijving[i] = takenBeschrijving[i + 1];
                        takenDeadline[i] = takenDeadline[i + 1];
                        takenVoltooid[i] = takenVoltooid[i + 1];
                    }
                    takenBeschrijving[^1] = default!;
                    takenDeadline[^1] = default!;
                    takenVoltooid[^1] = default!;
                    aantal--;

                    modus = "normaal";
                }
                else if (actie == "D") {
                    modus = "verwijderen";
                    continue;
                }
                // Scrijft de taken weg naar databanken.txt -------------------------------------------
                databank[0] = string.Join("|", takenBeschrijving, 0, aantal);
                databank[1] = string.Join("|", takenDeadline, 0, aantal);
                databank[2] = string.Join("|", takenVoltooid, 0, aantal);
                File.WriteAllLines(DATABANKPAD, databank);
            }
        }
    }
}
