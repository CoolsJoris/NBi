﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Query.Command
{
    class Command : ICommand
    {
        public object Implementation { get; }
        public object Session { get; }

        public Command(IDbConnection connection, IDbCommand command)
        {
            Session = connection;
            Implementation = command;
        }
    }
}
