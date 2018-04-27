﻿using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBi.Core.Batch;

namespace NBi.Core.SqlServer.Smo
{
    class BatchRunCommand : IDecorationCommandImplementation
    {
        private readonly string connectionString;
        private readonly string fullPath;

        public BatchRunCommand(string fullPath, string connectionString)
        {
            this.connectionString = connectionString;
            this.fullPath = fullPath;
        }       

        public void Execute()
        {
            if (!File.Exists(fullPath))
                throw new ExternalDependencyNotFoundException(fullPath);

            var script = File.ReadAllText(fullPath);
            Trace.WriteLineIf(Extensibility.NBiTraceSwitch.TraceVerbose, script);

            var server = new Server();
            server.ConnectionContext.ConnectionString = connectionString;
            server.ConnectionContext.ExecuteNonQuery(script);
        }
    }
}
