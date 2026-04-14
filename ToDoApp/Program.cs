using System.Globalization;

namespace ToDoApp {
    internal class Program {
        static void Main() {

            TakenLijst takenLijst = new();

            while (true) {

                takenLijst.PrintTakenLijst("normaal");
                // Vraagt de actie aan de gebruiker
                Console.Write("Acties ([T]oevoegen / [V]oltooien / [D]efinitief verwijderen) ?: ");
                string actie = Console.ReadLine()!.ToUpper().Trim();

                // ACTIE : toevoegen
                string taakBeschrijving;
                if (actie == "T") {
                    do {
                        Console.Write("Geef de taakbeschrijving (max 30 karakters) : ");
                        taakBeschrijving = Console.ReadLine()!.Trim();
                    }
                    while (string.IsNullOrWhiteSpace(taakBeschrijving) || taakBeschrijving.Length > 30);

                    string taakDeadlineString = "";
                    bool inputOk;
                    do {
                        Console.Write("Geef de deadline (yyyy-MM-dd) of [S]kip : ");
                        string inputString = Console.ReadLine()!.Trim().ToUpper();
                        if (inputString == "S") break;
                        inputOk = DateOnly.TryParseExact(inputString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly taakDeadline);
                        if (inputOk) taakDeadlineString = taakDeadline.ToString("yyyy-MM-dd");
                    }
                    while (!inputOk);

                    string[] taak = [taakBeschrijving, taakDeadlineString, ""];

                    // Plaats de taak in het begin van de lijst als hij geen deadline bevat
                    if (string.IsNullOrWhiteSpace(taakDeadlineString)) {
                        int index = 0;
                        takenLijst.VoegTaakInOpIndex(index, taak);
                    }
                    else { // Plaats de taak in het begin van de lijst van taken met een deadline als hij een deadline bevat
                        int index = takenLijst.GeefEersteLegeDeadlineIndex();
                        takenLijst.VoegTaakInOpIndex(index, taak);
                    }
                }
                // ACTIE : voltooien -------------------------------------------
                else if (actie == "V") {
                    takenLijst.PrintTakenLijst("voltooien");

                    bool inputOk;
                    int teVoltooienIndex;
                    do {
                        Console.Write("Geef het nummer van de te voltooien taak : ");
                        inputOk = int.TryParse(Console.ReadLine()!.Trim(), out teVoltooienIndex);
                        teVoltooienIndex--;
                    }
                    while (!inputOk || teVoltooienIndex < 0 || teVoltooienIndex > takenLijst.Aantal - 1);

                    int eersteVoltooiIndex = takenLijst.GeefEersteLegeVoltooideIndex();
                    string[] taak = takenLijst.HaalTaakOpIndex(teVoltooienIndex);
                    taak[2] = TakenLijst.HuidigeDatum().ToString("yyyy-MM-dd");

                    takenLijst.VoegTaakInOpIndex(eersteVoltooiIndex, taak);
                    takenLijst.VerwijderIndex(teVoltooienIndex);
                }
                // ACTIE : verwijderen -------------------------------------------
                else if (actie == "D") {
                    takenLijst.PrintTakenLijst("verwijderen");

                    bool inputok;
                    int teVerwijderenIndex;
                    do {
                        Console.Write("Geef het nummer van de te verwijderen taak : ");
                        inputok = int.TryParse(Console.ReadLine()!.Trim(), out teVerwijderenIndex);
                        teVerwijderenIndex--;
                    }
                    while (!inputok || teVerwijderenIndex < 0 || teVerwijderenIndex > takenLijst.Aantal - 1);

                    takenLijst.VerwijderIndex(teVerwijderenIndex);
                }
                takenLijst.SchrijfTakenWeg();
            }
        }
    }
}
