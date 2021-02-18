﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FamilyTreeEmil
{
    class DatabaseLogic
    {
        public static void NewDatabase()
        {
            SqlDatabase.CreateDatabase("FamilyTree");
            SqlDatabase.CreateTable($"Family",
                    "id int PRIMARY KEY IDENTITY (1,1) NOT NULL, " +
                    "first_name nvarchar(50) NOT NULL, " +
                    "last_name nvarchar(50) NOT NULL, " +
                    "date_of_birth date NULL, " +
                    "date_of_death date NULL, " +
                    "mother_id int NULL, " +
                    "father_id int NULL");
            Menu();
        }

        private static void Menu()
        {
            do
            {
                Console.Clear();
                Console.WriteLine($"Välkommen till databasen {SqlDatabase.DatabaseName}! \n");
                Console.WriteLine("Vad vill du göra?\n");
                Console.WriteLine("1. Sök efter personer (ändra, ta bort mm)");
                Console.WriteLine("2. Lägg till person");
                Console.WriteLine("3. Exit program");
                Console.Write("\nMenyval: ");
                var choice = Console.ReadLine();
                Tools.MenuHandling(choice, 3);

                switch (choice)
                {
                    case "1":
                        SearchPersons();
                        break;
                    case "2":
                        AddPerson();
                        break;
                    case "3":
                       Tools.Exit();
                        break;
                }
                Console.ReadLine();
            } while (true);
        }

        public static Person AddPerson()
        {
            Console.WriteLine();
            var person = new Person();
            Console.Write("Förnamn: ");
            person.FirstName = Console.ReadLine();
            Console.Write("Efternamn: ");
            person.LastName = Console.ReadLine();
            Console.Write("Födelsedatum (yyyy-mm-dd): ");
            try
            {
                person.DateOfBirth = Convert.ToDateTime(Console.ReadLine());
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

            var db = new SqlDatabase();
            db.Insert(person);
            person.Id = db.GetLastAddedId();
            Console.WriteLine($"\n{person.FirstName} {person.LastName} lades till i databasen {SqlDatabase.DatabaseName}");
            return person;
        }

        public static void SearchPersons()
        {
            bool keepMenuGo = true;
            while (keepMenuGo)
            {
                Console.Clear();
                Console.WriteLine("Vad vill du söka på?\n ");
                Console.WriteLine("1. Personer som börjar på en viss bokstav");
                Console.WriteLine("2. Personer födda ett visst år");
                Console.WriteLine("3. Backa till menyn");
                Console.Write("\nVal: ");
                var choice = Console.ReadLine();
                Console.WriteLine();
                string sql = null;
                var db = new SqlDatabase();
                var persons = new List<Person>();

                Tools.MenuHandling(choice, 3);
                switch (choice)
                {
                    case "1":
                        Console.Write("Skriv in en bokstav: ");
                        var letter = Console.ReadLine();
                        sql = "WHERE first_name LIKE @letter";
                        persons = db.SelectAll(sql, ("@letter", $"{letter}%"));
                        break;
                    case "2":
                        Console.Write("Skriv in ett årtal: ");
                        var year = Console.ReadLine();
                        sql = "WHERE date_of_birth LIKE @year";
                        persons = db.SelectAll(sql, ("@year", $"{year}%"));
                        break;
                    case "3":
                        keepMenuGo = false;
                        break;

                }


                Console.WriteLine();
                if (persons.Count > 0)
                {
                    foreach (var person in persons)
                    {
                        ShowInfo(person);
                    }
                    Console.Write("Välj ett id: ");
                    if (int.TryParse(Console.ReadLine(), out int option))
                    {
                        var person = persons.Where(p => p.Id == option).FirstOrDefault();
                        SelectPerson(person);
                    }
                    else
                    {
                        Tools.ErrorMessage();
                    }
                }

            }
        }

        private static void ShowInfo(Person person)
        {
            var db = new SqlDatabase();
            Console.WriteLine($"Id: {person.Id}");
            Console.WriteLine($"Namn: {person.FirstName} {person.LastName}");

            Console.Write("Födelsedatum: ");
            if (person.DateOfBirth.HasValue)
            {
                Console.Write(person.DateOfBirth.Value.ToShortDateString());
            }
            Console.WriteLine();

            Console.Write("Dödsdatum: ");
            if (person.DateOfDeath.HasValue)
            {
                Console.Write(person.DateOfDeath.Value.ToShortDateString());
            }
            Console.WriteLine();

            Console.Write("Mamma: ");
            if (person.MotherId.HasValue)
            {
                var mother = db.SearchById(person.MotherId.Value);
                Console.Write(mother.FirstName + " " + mother.LastName);
            }
            Console.WriteLine();

            Console.Write("Pappa: ");
            if (person.FatherId.HasValue)
            {
                var father = db.SearchById(person.FatherId.Value);
                Console.Write(father.FirstName + " " + father.LastName);
            }
            Console.WriteLine("\n");
        }

        private static void SelectPerson(Person person)
        {
            bool keepMenuGo = true;
            while (keepMenuGo)
            {
                Console.Clear();
                Console.WriteLine("Vad vill du göra?");
                Console.WriteLine("1. Uppdatera person");
                Console.WriteLine("2. Visa släkt");
                Console.WriteLine("3. Ta bort person");
                Console.WriteLine("4. Backa");
                Console.Write("\nMenyval: ");
                var choice = Console.ReadLine();
                Tools.MenuHandling(choice, 4);
                switch (choice)
                {
                    case "1":
                        UpdatePerson(person);
                        break;
                    case "2":
                        ShowRelatives(person);
                        break;
                    case "3":
                        if (DeletePerson(person))
                        {
                            break;
                        }
                        break;
                    case "4":
                        keepMenuGo = false;
                        break;
                }
            }
        }

        public static bool DeletePerson(Person person)
        {
            Console.WriteLine();
            Console.WriteLine($"Vill du to bort {person.FirstName} {person.LastName}? ");
            Console.Write($"j(ja) / n(nej): ");
            var choice = Console.ReadLine();
            if (choice.ToLower() == "j" || choice.ToLower() == "ja")
            {
                var db = new SqlDatabase();
                db.Delete(person);
                Console.WriteLine($"{person.FirstName} {person.LastName} togs bort!");
                return true;
            }
            return false;
        }

        public static void UpdatePerson(Person person)
        {
            while (true)
            {
                Console.Clear();
                ShowInfo(person);
                Console.WriteLine("Vad vill du ändra?");
                Console.WriteLine("1. Förnamn");
                Console.WriteLine("2. Efternamn");
                Console.WriteLine("3. Födelsedatum");
                Console.WriteLine("4. Dödsdatum");
                Console.WriteLine("5. Mamma");
                Console.WriteLine("6. Pappa");
                Console.WriteLine("7. Backa");
                Console.Write("\nMenyval: ");
                var choice = Console.ReadLine();
                Tools.MenuHandling(choice, 7);

                var db = new SqlDatabase();
                switch (choice)
                {
                    case "1":
                        Console.Write("Skriv in förnamn: ");
                        person.FirstName = Console.ReadLine();
                        break;
                    case "2":
                        Console.Write("Skriv in efternamn: ");
                        person.LastName = Console.ReadLine();
                        break;
                    case "3":
                        Console.Write("Skriv in födelsedatum: ");
                        person.DateOfBirth = Convert.ToDateTime(Console.ReadLine());
                        break;
                    case "4":
                        Console.Write("Skriv in dödsdatum: ");
                        person.DateOfDeath = Convert.ToDateTime(Console.ReadLine());
                        break;
                    case "5":
                        var mother = db.GetParent("mother");
                        if (mother != null)
                        {
                            person.MotherId = mother.Id;
                        }
                        break;
                    case "6":
                        var father = db.GetParent("father");
                        if (father != null)
                        {
                            person.FatherId = father.Id;
                        }
                        break;
                    case "7":
                        break;

                }
                db.Update(person);

            }

        }

        private static void ShowRelatives(Person person)
        {
            var db = new SqlDatabase();
            var parents = db.GetParents(person);
            if (parents.Count > 0)
            {
                Console.WriteLine("Föräldrar:");
                foreach (var parent in parents)
                {
                    ShowInfo(parent);
                }
                Console.WriteLine();
            }

            var siblings = db.GetSiblings(person);
            if (siblings.Count > 0)
            {
                Console.WriteLine("Syskon:");
                foreach (var sibling in siblings)
                {
                    ShowInfo(sibling);
                }
                Console.WriteLine();
            }

            var relatives = parents;
            relatives.AddRange(siblings);
            if (relatives.Count > 0)
            {
                Console.Write("Välj ett id: ");
                if (int.TryParse(Console.ReadLine(), out int option))
                {
                    var relative = relatives.Where(p => p.Id == option).FirstOrDefault();
                    SelectPerson(relative);
                }
                else
                {
                    Tools.ErrorMessage();
                }
            }
            else
            {
                Console.WriteLine($"{person.FirstName} {person.LastName} har ingen släkt.");

            }
        }




    }
}
