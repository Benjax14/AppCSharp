﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService
{
    public class Enterprise
    {
        public string Name { get; set; }
        public string Rut { get; set; }

        public Enterprise(string name, string rut)
        {

            Name = name;
            Rut = rut;

        }

    }
}
