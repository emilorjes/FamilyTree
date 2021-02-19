using System;

namespace FamilyTreeEmil
{
    class Program
    {
        /// <summary>
        /// Skapar upp ny databas.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var db = new Database();
            db.NewDatabase();

        }
    }
}
