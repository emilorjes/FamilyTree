using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyTreeEmil
{
    class Tools
    {
        public static void MenuHandling(string menuChoice, int numberOfMenuChoices)
        {
            if (!int.TryParse(menuChoice, out int menuNumber) || menuNumber < 1 || menuNumber > numberOfMenuChoices)
            {
                ErrorMessage();
            }
        }

        public static void RedTextWr(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(input);
            Console.ResetColor();
        }
        public static void BlueTextWr(string input)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(input);
            Console.ResetColor();
        }

        public static void GreenTextWl(string input)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(input);
            Console.ResetColor();
        }

        public static void GreenTextW(string input)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(input);
            Console.ResetColor();
        }

        public static void Exit()
        {
            Console.Write("Tack för att du använt databasen\n\n\n");
            Environment.Exit(0);
        }

        public static void ErrorMessage()
        {
            RedTextWr("Fel inmatning, försök igen.....");
            Console.ReadLine();
        }
    }
}
