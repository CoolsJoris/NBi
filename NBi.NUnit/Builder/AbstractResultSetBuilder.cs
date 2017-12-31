﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NBi.Core.Query;
using NBi.Xml.Constraints;
using NBi.Xml.Items;
using NBi.Xml.Systems;
using NBi.Core.ResultSet;
using NBi.Core.ResultSet.Resolver;
using NBi.Core.ResultSet.Alteration;
using NBi.Core.Evaluate;
using NBi.Core.Calculation;
using NBi.NUnit.Builder.Helper;
using NBi.Core.Query.Resolver;
using NBi.Core.Query.Command;

namespace NBi.NUnit.Builder
{
    abstract class AbstractResultSetBuilder : AbstractTestCaseBuilder
    {
        protected AbstractSystemUnderTestXml SystemUnderTestXml { get; set; }

        protected override void BaseSetup(AbstractSystemUnderTestXml sutXml, AbstractConstraintXml ctrXml)
        {
            if (!(sutXml is ExecutionXml || sutXml is ResultSetSystemXml))
                throw new ArgumentException("System-under-test must be a 'ExecutionXml' or 'ResultSetXml'");

            SystemUnderTestXml = sutXml;
        }

        protected override void BaseBuild()
        {
            if (SystemUnderTestXml is ExecutionXml)
                SystemUnderTest = InstantiateSystemUnderTest((ExecutionXml)SystemUnderTestXml);
            else
                SystemUnderTest = InstantiateSystemUnderTest((ResultSetSystemXml)SystemUnderTestXml);
        }

        protected virtual IResultSetService InstantiateSystemUnderTest(ExecutionXml executionXml)
        {
            var commandFactory = new CommandProvider();

            var argsBuilder = new QueryResolverArgsBuilder(ServiceLocator);

            var connectionString = executionXml.Item.GetConnectionString();
            var statement = (executionXml.Item as QueryableXml).GetQuery();

            IEnumerable<IQueryParameter> parameters = null;
            IEnumerable<IQueryTemplateVariable> variables = null;
            int timeout = 0;
            var commandType = System.Data.CommandType.Text;

            if (executionXml.BaseItem is QueryXml)
            {
                parameters = argsBuilder.BuildParameters(((QueryXml)executionXml.BaseItem).GetParameters());
                variables = ((QueryXml)executionXml.BaseItem).GetVariables();
                timeout = ((QueryXml)executionXml.BaseItem).Timeout;
            }
            if (executionXml.BaseItem is ReportXml)
            {
                parameters = argsBuilder.BuildParameters(((ReportXml)executionXml.BaseItem).GetParameters());
            }

            if (executionXml.BaseItem is ReportXml)
            {
                commandType = ((ReportXml)executionXml.BaseItem).GetCommandType();
            }

            var queryArgs = new QueryResolverArgs(statement, connectionString, parameters, variables, new TimeSpan(0, 0, timeout), commandType);
            var args = new QueryResultSetResolverArgs(queryArgs);
            var factory = ServiceLocator.GetResultSetResolverFactory();
            var resolver = factory.Instantiate(args);

            var builder = new ResultSetServiceBuilder();
            builder.Setup(resolver);
            var service = builder.GetService();

            return service;
        }

        protected virtual object InstantiateSystemUnderTest(ResultSetSystemXml resultSetXml)
        {
            var builder = new ResultSetServiceBuilder();
            builder.Setup(InstantiateResolver(resultSetXml));
            builder.Setup(InstantiateAlterations(resultSetXml));
            return builder.GetService();
        }

        protected virtual IResultSetResolver InstantiateResolver(ResultSetSystemXml resultSetXml)
        {
            var argsBuilder = new ResultSetResolverArgsBuilder(ServiceLocator);
            argsBuilder.Setup(resultSetXml);
            argsBuilder.Setup(resultSetXml.Settings);
            argsBuilder.Setup(base.Variables);
            argsBuilder.Build();

            var factory = ServiceLocator.GetResultSetResolverFactory();
            var resolver = factory.Instantiate(argsBuilder.GetArgs());
            return resolver;
        }

        private IEnumerable<Alter> InstantiateAlterations(ResultSetSystemXml resultSetXml)
        {
            if (resultSetXml.Alteration == null)
                yield break;

            if (resultSetXml.Alteration.Filters != null)
            {
                foreach (var filterXml in resultSetXml.Alteration.Filters)
                {
                    var expressions = new List<IColumnExpression>();
                    if (filterXml.Expression != null)
                        expressions.Add(filterXml.Expression);

                    var factory = new PredicateFilterFactory();
                    if (filterXml.Predication != null)
                        yield return factory.Instantiate
                                    (
                                        filterXml.Aliases
                                        , expressions
                                        , filterXml.Predication
                                    ).Apply;
                    if (filterXml.Combination != null)
                        yield return factory.Instantiate
                                    (
                                        filterXml.Aliases
                                        , expressions
                                        , filterXml.Combination.Operator
                                        , filterXml.Combination.Predicates
                                    ).Apply;
                }
            }
        }

    }
}
