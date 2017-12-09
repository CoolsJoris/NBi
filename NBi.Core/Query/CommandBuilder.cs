﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AnalysisServices.AdomdClient;

namespace NBi.Core.Query
{
    public class CommandBuilder
    {
        public IDbCommand Build(string connectionString, string query, IEnumerable<IQueryParameter> parameters)
        {
            return Build(connectionString, query, parameters, null);
        }

        public IDbCommand Build(string connectionString, string query, IEnumerable<IQueryTemplateVariable> variables)
        {
            return Build(connectionString, query, null, variables);
        }

        public IDbCommand Build(string connectionString, string query, IEnumerable<IQueryParameter> parameters, IEnumerable<IQueryTemplateVariable> variables)
        {
            var conn = new ConnectionFactory().Get(connectionString);
            var cmd = conn.CreateCommand();

            if (variables != null && variables.Count() > 0)
                query = ApplyVariablesToTemplate(query, variables);
                       
            cmd.CommandText = query;

            if (parameters!=null && parameters.Count()>0)
            {
                foreach (var p in parameters)
                {
                    var param = cmd.CreateParameter();

                    if (cmd is AdomdCommand && p.Name.StartsWith("@"))
                        param.ParameterName = p.Name.Substring(1, p.Name.Length - 1);
                    else if (cmd is SqlCommand && !p.Name.StartsWith("@") && char.IsLetter(p.Name[0]))
                        param.ParameterName = "@" + p.Name;
                    else
                        param.ParameterName = p.Name;

                    param.Value = p.GetValue();
                    var dbType = new DbTypeBuilder().Build(p.SqlType);
                    if (dbType != null)
                    {
                        param.Direction = ParameterDirection.Input;
                        param.DbType = dbType.Value;
                        param.Size = dbType.Size;
                        param.Precision = dbType.Precision;
                    }
                    cmd.Parameters.Add(param);
                }
            }

            return cmd;
        }

        public IDbCommand Build(string connectionString, string text, CommandType commandType, IEnumerable<IQueryParameter> parameters, IEnumerable<IQueryTemplateVariable> variables, int timeout)
        {
            var cmd = Build(connectionString, text, parameters, variables, timeout);
            cmd.CommandType = commandType;
            return cmd;
        }

        public IDbCommand Build(string connectionString, string query, IEnumerable<IQueryParameter> parameters, IEnumerable<IQueryTemplateVariable> variables, int timeout)
        {
            var cmd = Build(connectionString, query, parameters, variables);
            var commandTimeout = Math.Max(0, (int)Math.Ceiling(timeout / 1000.0));
            cmd.CommandTimeout = commandTimeout;
            return cmd;
        }

        private string ApplyVariablesToTemplate(string template, IEnumerable<IQueryTemplateVariable> variables)
        {
            var templateEngine = new StringTemplateEngine(template, variables);
            return templateEngine.Build();
        }

    }
}
