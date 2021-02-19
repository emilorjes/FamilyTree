using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyTreeEmil
{
    class SimpsonsFamily
    {
        /// <summary>
        /// För in Simpsons karaktärer i Family tabellen.
        /// </summary>
        public static void InsertSimpsons()
        {
            DatabaseHelper.ExecuteSql(
            @"INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Abraham','Simpson', 1910, NULL, NULL, NULL);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Mona','Simpson', 1912, NULL, NULL, NULL);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Clancy','Bouvier', 1908, NULL, NULL, NULL);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Jacqueline','Gurney', 1902, NULL, NULL, NULL);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Herbert','Powell', 1940, NULL, NULL, 1);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Homer','Simpson', 1943, NULL, 2, 1);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Patricia','Bouvier', 1942, NULL, 4, 3);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Selma','Bouvier', 1942, NULL, 4, 3);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Marge','Simpson', 1945, NULL, 4, 3);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Ling','Bouvier', 1980, NULL, 8, NULL);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Bart','Simpson', 1969, NULL, 9, 6);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Lisa','Simpson', 1971, NULL, 9, 6);
            INSERT INTO Family (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id) VALUES ('Maggie','Simpson', 1977, NULL, 9, 6);

            ");
        }




    }
}
