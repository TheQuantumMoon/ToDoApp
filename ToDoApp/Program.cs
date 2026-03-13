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

            string[] databank;
            if (File.Exists("databank.txt") && File.ReadAllLines("databank.txt").Length >= 3) {
                databank = File.ReadAllLines("databank.txt");
            }
            else databank = ["", "", ""];

            string[] takenBeschrijving = databank[0].Split("|");
            string[] takenDeadline = databank[1].Split("|");
            string[] takenVoltooid = databank[2].Split("|");

            Console.WriteLine("Taken:"); // Toont een taak als er een beschrijving is, maar geen deadline of voltooidtijd
            for (int i = 0; i < takenBeschrijving.Length; i++) {
                if (takenBeschrijving[i] != "" && takenDeadline[i] == "" && takenVoltooid[i] == "") {
                    Console.WriteLine(" *   " + takenBeschrijving[i]);
                }
            }
            Console.WriteLine("\nTaken met deadline:"); // Toont een deadline taak als er een beschrijving is, een deadline en geen voltooidtijd
            for (int i = 0; i < takenBeschrijving.Length; i++) {
                if (takenBeschrijving[i] != "" && takenDeadline[i] != "" && takenVoltooid[i] == "") {
                    Console.WriteLine($" *   deadline: {takenDeadline[i]} | {takenBeschrijving[i]}");
                }
            }
            Console.WriteLine("\nVoltooide taken:"); // Toont een voltooide taak als er een beschrijving is, eventueel een deadline en een voltooidtijd
            for (int i = 0; i < takenBeschrijving.Length; i++) {
                if (takenBeschrijving[i] != "" && takenVoltooid[i] != "" && takenDeadline[i] == "") {
                    Console.WriteLine($" *   voltooid op: {takenVoltooid[i]} | {takenBeschrijving[i]}");
                }
                else if (takenVoltooid[i] != "" && takenDeadline[i] != "") {
                    Console.WriteLine($" *   voltooid op: {takenVoltooid[i]} | deadline: {takenDeadline[i]} | {takenBeschrijving[i]}");
                }
            }

            databank[0] = string.Join("|", takenBeschrijving);
            databank[1] = string.Join("|", takenDeadline);
            databank[2] = string.Join("|", takenVoltooid);

            File.WriteAllLines("databank.txt", databank);

            Console.WriteLine("\nEINDE PROGRAMMA");
            Console.ReadKey();
        }
    }
}
