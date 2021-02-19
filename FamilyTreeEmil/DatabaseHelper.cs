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
    class DatabaseHelper
    {
        public static string ConnectionString { get; set; } = @"Data Source = .\SQLExpress; Integrated Security = true; database = {0}";
        public static string DatabaseName { get; set; } = "FamilyTree";

        public static void ExecuteSql(string sql, params (string, string)[] parameters)
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


        public static void CreateDatabase(string databaseName)
        {
            try
            {
                DatabaseName = "master";
                ExecuteSql($"CREATE DATABASE {databaseName}");
                DatabaseName = databaseName;
                Tools.GreenTextWl($"Databas med namn {databaseName} har skapats!");
            }
            catch (Exception)
            {
                DatabaseName = databaseName;
                Tools.RedTextWr($"En databas med namnet {databaseName} är redan skapad!");
            }
        }

        public static void CreateTable(string tableName, string columns)
        {
            try
            {
                ExecuteSql($"CREATE TABLE {tableName} ({columns})");
                Tools.GreenTextW($"Tabell med namn {tableName} har skapats!\n");
                Console.Write("Tryck Enter för att fortsätta: ");
                SimpsonsFamily.InsertSimpsons();
                Console.ReadKey();
            }
            catch (Exception)
            {
                Tools.RedTextWr($"En tabell med namnet {tableName} är redan skapad!");
                Console.Write("\nTryck Enter för att fortsätta med den existerande databasen: ");
                Console.ReadKey();
            }
        }

        public void Insert(Person person)
        {
            var sql = "INSERT Family (first_name, last_name, date_of_birth) " +
                "VALUES (@fName, @lName, @dob)";
            var parameters = new (string, string)[]
            {
                ("@fName", person.FirstName),
                ("@lName", person.LastName),
                ("@dob", person.DateOfBirth.Value.ToString())
            };
            ExecuteSql(sql, parameters);

        }


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
                person.DateOfBirth = (int)row["date_of_birth"];
            }

            if ((row["date_of_death"] is DBNull) == false)
            {
                person.DateOfDeath = (int)row["date_of_death"];
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
                dob = person.DateOfBirth.Value.ToString();
            }
            else
            {
                dob = null;
            }

            string dod;
            if (person.DateOfDeath.HasValue)
            {
                dod = person.DateOfDeath.Value.ToString();
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
                var db = new Database();
                return db.AddPerson();
            }
            return null;
        }

        public static void DisplayPersons(List<Person> persons)
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
                        Tools.ErrorMessage();
                    }
                }
                else
                {
                    Tools.ErrorMessage();
                }
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
