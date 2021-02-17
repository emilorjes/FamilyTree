using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;



namespace FamilyTreeEmil
{
    class SqlDatabase
    {
        public string ConnectionString { get; set; } = @"Data Source = .\SQLExpress; Integrated Security = true; database = {0}";
        public string DatabaseName { get; set; } = "FamilyTree";
        public List<Person> personList = new List<Person>();


        //============================================================================================================================================================================================
        private void ExecuteSql(string sql, params (string, string)[] parameters)
        {
            Debug.WriteLine(sql);
            var connectionString = string.Format(ConnectionString, DatabaseName);
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        Debug.WriteLine(parameter.Item1 + " Kommer att ha värdet " + parameter.Item2);
                        command.Parameters.AddWithValue(parameter.Item1, parameter.Item2);
                    }

                    foreach (SqlParameter parameter in command.Parameters)
                    {
                        if (parameter.Value == null)
                        {
                            parameter.Value = DBNull.Value;
                        }
                    }
                    command.ExecuteNonQuery();
                }
            }
        }

        //============================================================================================================================================================================================
        public void NewDatabase(Person person)
        {
            CreateDatabase("FamilyTree");
            CreateTable($"Family",
                    "id int PRIMARY KEY IDENTITY (1,1) NOT NULL, " +
                    "first_name nvarchar(50) NOT NULL, " +
                    "last_name nvarchar(50) NOT NULL, " +
                    "date_of_birth date NULL, " +
                    "date_of_death date NULL, " +
                    "mother_id int NULL, " +
                    "father_id int NULL");
            Menu(person);
        }

        //============================================================================================================================================================================================
        private void Menu(Person person)
        {
            do
            {
                Console.Clear();
                Console.WriteLine($"Välkommen till databasen {DatabaseName}! \n");
                Console.WriteLine("Vad vill du göra?\n");
                Console.WriteLine("1. Sök efter personer (ändra, ta bort mm)");
                Console.WriteLine("2. Lägg till person");
                Console.WriteLine("3. Exit program");
                Console.Write("\nMenyval: ");
                var choice = Console.ReadLine();
                MenuHandling(choice, 3);

                switch (choice)
                {
                    case "1":
                        SearchPersons();
                        break;
                    case "2":
                        AddPerson();
                        break;
                    case "3":
                        Exit();
                        break;
                   
                }
                Console.ReadLine();
            } while (true);
        }

        //============================================================================================================================================================================================
        private void CreateDatabase(string databaseName)
        {
            try
            {
                DatabaseName = "master";
                ExecuteSql($"CREATE DATABASE {databaseName}");
                DatabaseName = databaseName;
                Console.WriteLine($"Databas med namn {databaseName} har skapats!");
            }
            catch (Exception e)
            {
                DatabaseName = databaseName;
                Console.WriteLine(e.Message);
            }
        }

        //============================================================================================================================================================================================
        private void CreateTable(string tableName, string columns)
        {
            try
            {
                ExecuteSql($"CREATE TABLE {tableName} ({columns})");
                Console.WriteLine($"Tabell med namn {tableName} har skapats!\n");
                Console.Write("Tryck Enter för att fortsätta: ");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Write("\nTryck Enter för att fortsätta med den existerande databasen: ");
                Console.ReadKey();
            }
        }

        //============================================================================================================================================================================================
        private Person AddPerson()
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
            Console.WriteLine($"\n{person.FirstName} {person.LastName} lades till i databasen {DatabaseName}");
            return person;
        }

        //============================================================================================================================================================================================
        public void Insert(Person person)
        {
            var sql = "INSERT Family (first_name, last_name, date_of_birth) " +
                "VALUES (@fName, @lName, @dob)";
            var parameters = new (string, string)[]
            {
                ("@fName", person.FirstName),
                ("@lName", person.LastName),
                ("@dob", person.DateOfBirth.Value.ToShortDateString())
            };
            ExecuteSql(sql, parameters);




        }

        //============================================================================================================================================================================================
        private static void MenuHandling(string menuChoice, int numberOfMenuChoices)
        {
            if (!int.TryParse(menuChoice, out int menuNumber) || menuNumber < 1 || menuNumber > numberOfMenuChoices)
            {
                ErrorMessage();
            }
        }

        //============================================================================================================================================================================================
        private static void RedTextWr(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(input);
            Console.ResetColor();
        }

        //============================================================================================================================================================================================
        private static void Exit()
        {
            Console.Write("Tack för att du använt databasen\n\n\n");
            Environment.Exit(0);
        }
        //============================================================================================================================================================================================

        private static void ErrorMessage()
        {
            RedTextWr("Fel inmatning, försök igen.....");
            Console.ReadLine();
        }

        //============================================================================================================================================================================================
        public void SearchPersons()
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

                MenuHandling(choice, 3);
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
                        ErrorMessage();
                    }
                }

            }
        }

        //============================================================================================================================================================================================
        public List<Person> SelectAll(string filter = null, params (string, string)[] parameters)
        {
            var sql = "SELECT * FROM Family ";
            var dt = new DataTable();
            if (filter != null)
            {
                sql += filter;
                dt = GetDataTable(sql, parameters);
            }
            else
            {
                dt = GetDataTable(sql);
            }

            var persons = new List<Person>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    persons.Add(GetPerson(row));
                }
            }
            return persons;
        }

        //============================================================================================================================================================================================
        private DataTable GetDataTable(string sql, params (string, string)[] parameters)
        {
            var dt = new DataTable();
            var connectionString = string.Format(ConnectionString, DatabaseName);
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Item1, parameter.Item2);
                    }

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        //============================================================================================================================================================================================
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

        //============================================================================================================================================================================================
        public Person SearchById(int? id)
        {
            if (id.HasValue)
            {
                var sql = "SELECT * FROM Family WHERE id = @id";
                var dt = GetDataTable(sql, ("@id", id.Value.ToString()));
                if (dt.Rows.Count > 0)
                {
                    return GetPerson(dt.Rows[0]);
                }
            }
            return null;
        }

        //============================================================================================================================================================================================
        private Person GetPerson(DataRow row)
        {
            var person = new Person()
            {
                Id = (int)row["id"],
                FirstName = row["first_name"].ToString(),
                LastName = row["last_name"].ToString()
            };

            if ((row["date_of_birth"] is DBNull) == false)
            {
                person.DateOfBirth = (DateTime)row["date_of_birth"];
            }

            if ((row["date_of_death"] is DBNull) == false)
            {
                person.DateOfDeath = (DateTime)row["date_of_death"];
            }

            if ((row["mother_id"] is DBNull) == false)
            {
                person.MotherId = (int)row["mother_id"];
            }

            if ((row["father_id"] is DBNull) == false)
            {
                person.FatherId = (int)row["father_id"];
            }

            return person;
        }

        private void SelectPerson(Person person)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Vad vill du göra?");
                Console.WriteLine("1. Uppdatera person");
                Console.WriteLine("2. Visa släkt");
                Console.WriteLine("3. Ta bort person");
                Console.WriteLine("4. Backa");
                Console.Write("\nMenyval: ");
                var choice = Console.ReadLine();
                MenuHandling(choice, 4);
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
                        Exit();
                        break;
                }  
            }
        }

        public bool DeletePerson(Person person)
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

        public void Delete(Person person)
        {
            var sql = "DELETE FROM Family WHERE id = @id";
            ExecuteSql(sql, ("@id", person.Id.ToString()));
        }

        public void Update(Person person)
        {
            var sql = "UPDATE Family SET first_name = @fName, " +
                "last_name = @lName, date_of_birth = @dob, " +
                "date_of_death = @dod, mother_id = @mId, father_id = @fId " +
                "WHERE id = @id";

            string dob;
            if (person.DateOfBirth.HasValue)
            {
                dob = person.DateOfBirth.Value.ToShortDateString();
            }
            else
            {
                dob = null;
            }

            string dod;
            if (person.DateOfDeath.HasValue)
            {
                dod = person.DateOfDeath.Value.ToShortDateString();
            }
            else
            {
                dod = null;
            }

            string mId;
            if (person.MotherId.HasValue)
            {
                mId = person.MotherId.Value.ToString();
            }
            else
            {
                mId = null;
            }

            string fId;
            if (person.FatherId.HasValue)
            {
                fId = person.FatherId.Value.ToString();
            }
            else
            {
                fId = null;
            }

            var parameters = new (string, string)[]
            {
                ("@id", person.Id.ToString()),
                ("@fName", person.FirstName),
                ("@lname", person.LastName),
                ("@dob", dob),
                ("@dod", dod),
                ("@mId", mId),
                ("@fId", fId)
            };
            ExecuteSql(sql, parameters);
        }

        public void UpdatePerson(Person person)
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
                MenuHandling(choice, 7);
              
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

        public List<Person> GetParents(Person person)
        {
            var parents = new List<Person>();
            var mother = SearchById(person.MotherId);
            if (mother != null)
            {
                parents.Add(mother);
            }
            var father = SearchById(person.FatherId);
            if (father != null)
            {
                parents.Add(father);
            }
            return parents;
        }

        public Person GetParent(string type)
        {
            Console.Write($"Skriv in namn på {type}: ");
            var name = Console.ReadLine();

            var sql = "SELECT * FROM Family WHERE first_name LIKE @name";
            var dt = GetDataTable(sql, ("@name", $"%{name}%"));
            if (dt.Rows.Count > 0)
            {
                var persons = new List<Person>();
                foreach (DataRow row in dt.Rows)
                {
                    persons.Add(GetPerson(row));
                }

                DisplayPersons(persons);
                var option = ChoosePerson(persons.Count);
                if (option > 0)
                {
                    return persons[option - 1];
                }
            }

            Console.WriteLine($"{name} verkar inte existera i databasen!");
            Console.WriteLine($"Vill du skapa {name}? ");
            Console.Write($"j(ja) / n(nej): ");
            var choice = Console.ReadLine();
            if (choice.ToLower() == "j" || choice.ToLower() == "ja")
            {
                var db = new SqlDatabase();
                return db.AddPerson();
            }
            return null;
        }

        private void DisplayPersons(List<Person> persons)
        {
            var ctr = 1;
            foreach (var person in persons)
            {
                var info = $"{ctr++}. {person.FirstName} {person.LastName} ";
                if (person.DateOfBirth != null)
                {
                    info += person.DateOfBirth.Value.ToString("d") + " ";
                }
                if (person.DateOfDeath != null)
                {
                    info += person.DateOfDeath.Value.ToString("d") + " ";
                }
                Console.WriteLine(info);
            }
            Console.WriteLine("0. Ingen av ovanstående");
        }

        private int ChoosePerson(int count)
        {
            while (true)
            {
                Console.Write("Vilken person vill du välja? ");
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice > 0 && choice >= count)
                    {
                        return choice;
                    }
                    else if (choice == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        ErrorMessage();
                    }
                }
                else
                {
                    ErrorMessage();
                }
            }
        }

        private void ShowRelatives(Person person)
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
                    ErrorMessage();
                }
            }
            else
            {
                Console.WriteLine($"{person.FirstName} {person.LastName} har ingen släkt.");
                
            }
        }

        public List<Person> GetSiblings(Person person)
        {
            var sql = "SELECT * FROM Family WHERE mother_id = @mId OR father_id = @fId";
            var dt = GetDataTable(sql, ("@mId", person.MotherId.ToString()),
                ("@fId", person.FatherId.ToString()));
            var siblings = new List<Person>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    siblings.Add(GetPerson(row));
                }
            }
            return siblings.Where(s => s.Id != person.Id).ToList();
        }

        public int GetLastAddedId()
        {
            var sql = "SELECT id FROM Family";
            var dt = GetDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                var ids = new List<int>();
                foreach (DataRow row in dt.Rows)
                {
                    ids.Add((int)row["id"]);
                }
                return ids.Last();
            }
            return 0;
        }
    }
}
