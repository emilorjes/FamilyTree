using System;

namespace FamilyTreeEmil
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new SqlDatabase();
            var person = new Person();

            db.NewDatabase(person);
            
        }
    }
}
