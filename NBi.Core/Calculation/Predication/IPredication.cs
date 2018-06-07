﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Calculation.Predication
{
    public interface IPredication
    {
        bool Execute(DataRow row);
        string Describe();
    }
}
