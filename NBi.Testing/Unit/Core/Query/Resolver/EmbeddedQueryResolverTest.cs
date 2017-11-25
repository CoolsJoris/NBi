﻿using NBi.Core.Query;
using NBi.Core.Query.Resolver;
using NBi.Xml.Items;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Testing.Unit.Core.Query.Resolver
{
    [TestFixture]
    public class EmbeddedQueryResolverTest
    {
        private EmbeddedQueryResolverArgs BuildArgs()
        {
            return new EmbeddedQueryResolverArgs(
                "select * from myTable;",
                ConnectionStringReader.GetSqlClient(),
                new List<IQueryParameter>() { new QueryParameterXml() { Name="param", StringValue="10" } },
                new List<IQueryTemplateVariable>() { new QueryTemplateVariableXml() { Name = "operator", Value = "not in" } },
                10);
        }

        [Test]
        public void Execute_Args_CommandInstantiated()
        {
            var resolver = new EmbeddedQueryResolver(BuildArgs());
            var cmd = resolver.Execute();

            Assert.That(cmd, Is.Not.Null);
        }

        [Test]
        public void Execute_Args_ConnectionStringAssigned()
        {
            var resolver = new EmbeddedQueryResolver(BuildArgs());
            var cmd = resolver.Execute();

            Assert.That(cmd.Connection.ConnectionString, Is.Not.Null.And.Not.Empty);
            Assert.That(cmd.Connection.ConnectionString, Is.EqualTo(ConnectionStringReader.GetSqlClient()));
        }

        [Test]
        public void Execute_Args_CommandTextAssigned()
        {
            var resolver = new EmbeddedQueryResolver(BuildArgs());
            var cmd = resolver.Execute();

            Assert.That(cmd.CommandText, Is.EqualTo("select * from myTable;"));
        }

        [Test]
        public void Execute_Args_ParametersAssigned()
        {
            var resolver = new EmbeddedQueryResolver(BuildArgs());
            var cmd = resolver.Execute();

            Assert.That(cmd.Parameters, Has.Count.EqualTo(1));
        }
        
    }
}
