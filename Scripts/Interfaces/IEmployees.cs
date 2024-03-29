﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces
{
    internal interface IEmployees
    {
        public int employeeCapacity { get; set; }
        public List<Person> employees { get; set; }

        public bool AddEmployee(Person p);
        public bool RemoveEmployee(Person p);
        public void Calculate();
    }
}
