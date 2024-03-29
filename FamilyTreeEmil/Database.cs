﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FamilyTreeEmil
{
    class Database
    {
        /// <summary>
        /// Innehåller metoder för att skapa databas / tabell, och meny.
        /// </summary>
        public void NewDatabase()
        {
            DatabaseHelper.CreateDatabase("FamilyTree");
            DatabaseHelper.CreateTable($"Family",
                    "id int PRIMARY KEY IDENTITY (1,1) NOT NULL, " +
                    "first_name nvarchar(50) NOT NULL, " +
                    "last_name nvarchar(50) NOT NULL, " +
                    "date_of_birth int NULL, " +
                    "date_of_death int NULL, " +
                    "mother_id int NULL, " +
                    "father_id int NULL");
            MainMenu();
        }

        /// <summary>
        /// Huvudmeny som innehåller metoder för att söka, lägga till och lista personer i databasen.
        /// </summary>
        private void MainMenu()
        {
            do
            {
                Console.Clear();
                Console.WriteLine($"Välkommen till databasen {DatabaseHelper.DatabaseName}! \n");
                Console.WriteLine("Vad vill du göra?\n");
                Console.WriteLine("1. Sök efter personer (ändra, ta bort mm)");
                Console.WriteLine("2. Lägg till person");
                Console.WriteLine("3. Visa alla personer");
                Console.WriteLine("4. Exit program");
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
                        ListAll();
                        break;
                    case "4":
                        Tools.Exit();
                        break;
                }
                Console.ReadLine();
            } while (true);
        }

        /// <summary>
        /// Tar in användar input och lägger till person i databasen.
        /// </summary>
        /// <returns></returns>
        public Person AddPerson()
        {
            var person = new Person();
            bool keepMenuGo = true;
            while (keepMenuGo)
            {
                Console.Write("\nFörnamn: ");
                person.FirstName = Console.ReadLine();
                Console.Write("Efternamn: ");
                person.LastName = Console.ReadLine();
                Console.Write("Födelseår: ");
                person.DateOfBirth = Convert.ToInt32(Console.ReadLine());

                if (person.FirstName == "" || person.LastName == "")
                {
                    Tools.RedTextWL("\nDu måste fylla i förnamn, efternamn!\n");
                }
                else
                {
                    var db = new DatabaseHelper();
                    db.Insert(person);
                    person.Id = db.GetLastAddedId();
                    Tools.GreenTextWL($"\n{person.FirstName} {person.LastName} lades till i databasen {DatabaseHelper.DatabaseName}");
                    keepMenuGo = false;
                }
            }
            return person;
        }

        /// <summary>
        /// Går att söka på personer genom deras första bokstav eller födelseår.
        /// </summary>
        private void SearchPersons()
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
                var db = new DatabaseHelper();
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
                    Tools.GreenTextW("Välj ett id: ");
                    if (int.TryParse(Console.ReadLine(), out int option))
                    {
                        var person = persons.Where(p => p.Id == option).FirstOrDefault();
                        SelectPerson(person);
                    }
                }
                else
                {
                    Tools.RedTextWL("Det finns inga person som matchar din sökning!");
                }
            }
        }

        /// <summary>
        /// Listar alla personerna i databasen.
        /// </summary>
        public void ListAll()
        {
            var db = new DatabaseHelper();
            var persons = new List<Person>();

            persons = db.SelectAll();

            Console.WriteLine();
            if (persons.Count > 0)
            {
                foreach (var person in persons)
                {
                    ShowInfo(person);
                }
                Tools.GreenTextW("Välj ett id: ");
                if (int.TryParse(Console.ReadLine(), out int option))
                {
                    var person = persons.Where(p => p.Id == option).FirstOrDefault();
                    SelectPerson(person);
                }
            }
        }

        /// <summary>
        /// Visar information på personen.
        /// </summary>
        /// <param name="person"></param>
        private void ShowInfo(Person person)
        {
            var db = new DatabaseHelper();
            Tools.GreenTextWL($"Id: {person.Id}");
            Console.WriteLine($"Namn: {person.FirstName} {person.LastName}");

            Console.Write("Födelsedatum: ");
            if (person.DateOfBirth.HasValue)
            {
                Console.Write(person.DateOfBirth.Value);
            }
            Console.WriteLine();

            Console.Write("Dödsdatum: ");
            if (person.DateOfDeath.HasValue)
            {
                Console.Write(person.DateOfDeath.Value);
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

        /// <summary>
        /// Uppdatera, visa släkt eller ta bort person.
        /// </summary>
        /// <param name="person"></param>
        private void SelectPerson(Person person)
        {
            bool keepMenuGo = true;
            while (keepMenuGo)
            {
                Console.Clear();
                ShowInfo(person);
                Console.WriteLine("\nVad vill du göra?");
                Console.WriteLine("1. Uppdatera person");
                Console.WriteLine("2. Visa släkt");
                Console.WriteLine("3. Ta bort person");
                Console.WriteLine("4. Backa");
                Console.Write("\nMenyval: ");

                var choice = Console.ReadLine();
                Console.WriteLine();
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
                        DeletePerson(person);
                        keepMenuGo = false;
                        break;
                    case "4":
                        keepMenuGo = false;
                        break;
                }
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Tar bort person från databasen.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        private bool DeletePerson(Person person)
        {
            Console.WriteLine();
            Console.WriteLine($"Vill du to bort {person.FirstName} {person.LastName}? ");
            Console.Write($"j(ja) / n(nej): ");
            var choice = Console.ReadLine();
            if (choice.ToLower() == "j" || choice.ToLower() == "ja")
            {
                var db = new DatabaseHelper();
                db.Delete(person);
                Tools.RedTextWL($"\n{person.FirstName} {person.LastName} togs bort!");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Uppdaterar personens information beroende på vad användaren skriver in.
        /// </summary>
        /// <param name="person"></param>
        private void UpdatePerson(Person person)
        {
            bool keepMenuGo = true;
            while (keepMenuGo)
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

                var db = new DatabaseHelper();
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
                        person.DateOfBirth = Convert.ToInt32(Console.ReadLine());
                        break;
                    case "4":
                        Console.Write("Skriv in dödsdatum: ");
                        person.DateOfDeath = Convert.ToInt32(Console.ReadLine());
                        break;
                    case "5":
                        var mother = db.GetParent("Mamma");
                        if (mother != null)
                        {
                            person.MotherId = mother.Id;
                        }
                        break;
                    case "6":
                        var father = db.GetParent("Pappa");
                        if (father != null)
                        {
                            person.FatherId = father.Id;
                        }
                        break;
                    case "7":
                        keepMenuGo = false;
                        break;
                }
                db.Update(person);
            }
        }

        /// <summary>
        /// Visar släkten till det person ID användaren valt.
        /// </summary>
        /// <param name="person"></param>
        private void ShowRelatives(Person person)
        {
            var db = new DatabaseHelper();
            var parents = db.GetParents(person);
            if (parents.Count > 0)
            {
                Tools.BlueTextWL("Föräldrar:");
                foreach (var parent in parents)
                {
                    ShowInfo(parent);
                }
                Console.WriteLine();
            }

            var siblings = db.GetSiblings(person);
            if (siblings.Count > 0)
            {
                Tools.BlueTextWL("Syskon:");
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
                Tools.GreenTextW("Välj ett id: ");
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
