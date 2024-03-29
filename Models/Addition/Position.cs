﻿using System;
using Employees.Classes;
using Employees.Models;

namespace DataModels
{
    public partial class Position : ICloneable, ISearchable
    {
        public object Clone() => MemberwiseClone();

        public bool Search(string search) => search.IsEmpty() || Name.Search(search);
    }
}