﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyTreeEmil
{
    class Person
    {
        public int Id { get; set; } = 0;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; } = null;
        public DateTime? DateOfDeath { get; set; } = null;
        public int? MotherId { get; set; } = null;
        public int? FatherId { get; set; } = null;

    }
}
