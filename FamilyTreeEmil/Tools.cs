using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyTreeEmil
{
    class Tools
    {
        /// <summary>
        /// Felhantera användar input i ex switch menyer.
        /// </summary>
        /// <param name="menuChoice"></param>
        /// <param name="numberOfMenuChoices"></param>
        public static void MenuHandling(string menuChoice, int numberOfMenuChoices)
        {
            if (!int.TryParse(menuChoice, out int menuNumber) || menuNumber < 1 || menuNumber > numberOfMenuChoices)
            {
                ErrorMessage();
            }
        }

        /// <summary>
        /// Console.WriteLine med röd text.
        /// </summary>
        /// <param name="input"></param>
        public static void RedTextWL(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(input);
            Console.ResetColor();
        }

        /// <summary>
        /// Console.WriteLine med blå text.
        /// </summary>
        /// <param name="input"></param>
        public static void BlueTextWL(string input)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(input);
            Console.ResetColor();
        }

        /// <summary>
        /// Console.WriteLine med grön text.
        /// </summary>
        /// <param name="input"></param>
        public static void GreenTextWL(string input)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(input);
            Console.ResetColor();
        }

        /// <summary>
        /// Console.Write med grön text.
        /// </summary>
        /// <param name="input"></param>
        public static void GreenTextW(string input)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(input);
            Console.ResetColor();
        }

        /// <summary>
        /// Stänger ner programmet.
        /// </summary>
        public static void Exit()
        {
            Console.Write("Tack för att du använt databasen\n\n\n");
            Environment.Exit(0);
        }

        /// <summary>
        /// Skriver ut ett rött felmeddelande.
        /// </summary>
        public static void ErrorMessage()
        {
            RedTextWL("Fel inmatning, försök igen.....");
            Console.ReadLine();
        }
    }
}
