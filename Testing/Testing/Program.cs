﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            i += i++;
            i += ++i;
            i++;
            Console.WriteLine(i);
        }
    }
}
