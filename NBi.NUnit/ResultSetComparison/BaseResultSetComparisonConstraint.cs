﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using NBi.Core;
using NBi.Core.ResultSet;
using NUnitCtr = NUnit.Framework.Constraints;
using NBi.Framework.FailureMessage;
using NBi.Framework;
using NBi.Core.Xml;
using NBi.Core.Transformation;
using NBi.Core.ResultSet.Analyzer;
using NBi.Core.ResultSet.Service;

namespace NBi.NUnit.ResultSetComparison
{
    public abstract class BaseResultSetComparisonConstraint : NBiConstraint
    {
        
        
        protected IResultSetService expect;

        protected bool parallelizeQueries = false;
        protected CsvProfile csvProfile;

        protected ResultSet expectedResultSet;
        protected ResultSet actualResultSet;

        protected ResultSetCompareResult result;
        private DataRowsMessage failure;
        protected DataRowsMessage Failure
        {
            get
            {
                if (failure == null)
                    failure = BuildFailure();
                return failure;
            }
        }

        protected virtual DataRowsMessage BuildFailure()
        {
            var msg = new DataRowsMessage(Engine.Style, Configuration.FailureReportProfile);
            msg.BuildComparaison(expectedResultSet.Rows.Cast<DataRow>(), actualResultSet.Rows.Cast<DataRow>(), result);
            return msg;
        }
     
        /// <summary>
        /// Engine dedicated to ResultSet comparaison
        /// </summary>
        protected IResultSetComparer _engine;
        protected internal virtual IResultSetComparer Engine
        {
            get
            {
                if(_engine==null)
                    _engine = new ResultSetComparerByIndex(AnalyzersFactory.EqualTo(), null);
                return _engine;
            }
            set
            {
                if(value==null)
                    throw new ArgumentNullException();
                _engine = value;
            }
        }

        public TransformationProvider TransformationProvider { get; protected set; }

                
        
        public BaseResultSetComparisonConstraint(IResultSetService value)
        {
            this.expect = value;
        }

        
        public BaseResultSetComparisonConstraint Using(IResultSetComparer engine)
        {
            this.Engine = engine;
            return this;
        }

        public BaseResultSetComparisonConstraint Using(ISettingsResultSetComparison settings)
        {
            this.Engine.Settings = settings;
            return this;
        }

        public BaseResultSetComparisonConstraint Using(TransformationProvider transformationProvider)
        {
            this.TransformationProvider = transformationProvider;
            return this;
        }

        public BaseResultSetComparisonConstraint Parallel()
        {
            this.parallelizeQueries = true;
            return this;
        }

        public BaseResultSetComparisonConstraint Sequential()
        {
            this.parallelizeQueries = false;
            return this;
        }

        public BaseResultSetComparisonConstraint CsvProfile(CsvProfile profile)
        {
            this.csvProfile = profile;
            return this;
        }


        /// <summary>
        /// Handle an IDbCommand and compare it to a predefined resultset
        /// </summary>
        /// <param name="actual">An OleDbCommand, SqlCommand or AdomdCommand</param>
        /// <returns>true, if the result of query execution is exactly identical to the content of the resultset</returns>
        public override bool Matches(object actual)
        {
            if (actual is IDbCommand)
                return Process((IDbCommand)actual);
            else if (actual is ResultSet)
                return doMatch((ResultSet)actual);
            else if (actual is string)
            {
                var rsFactory = new ResultSetServiceFactory();
                var service = rsFactory.Instantiate(actual, null);
                return Matches(service.Execute());
            }
            else
                throw new ArgumentException();
        }

        protected bool doMatch(ResultSet actual)
        {
            actualResultSet = actual;

            //This is needed if we don't use //ism
            if (expectedResultSet ==  null)
                expectedResultSet = GetResultSet(expect);

            if (TransformationProvider != null)
                TransformationProvider.Transform(expectedResultSet);

            result = Engine.Compare(actualResultSet, expectedResultSet);

            return result.Difference == ResultSetDifferenceType.None;
        }

        /// <summary>
        /// Handle an IDbCommand (Query and ConnectionString) and check it with the expectation (Another IDbCommand or a ResultSet)
        /// </summary>
        /// <param name="actual">IDbCommand</param>
        /// <returns></returns>
        public bool Process(IDbCommand actual)
        {
            ResultSet rsActual = null;
            if (parallelizeQueries)
            {
                rsActual = ProcessParallel(actual);
            }
            else
                rsActual = GetResultSet(actual);
            
            return this.Matches(rsActual);
        }

        public ResultSet ProcessParallel(IDbCommand actual)
        {
            Trace.WriteLineIf(NBiTraceSwitch.TraceVerbose, string.Format("Queries exectued in parallel."));
            
            ResultSet rsActual = null;
            System.Threading.Tasks.Parallel.Invoke(
                () => {
                        rsActual = GetResultSet(actual);
                      },
                () => {
                        expectedResultSet = expect.Execute();
                }
            );
            
            return rsActual;
        }

        protected ResultSet GetResultSet(Object obj)
        {
            var factory = new ResultSetServiceFactory();
            var service = factory.Instantiate(obj, null);
            return service.Execute();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteDescriptionTo(NUnitCtr.MessageWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine(Failure.RenderExpected());
        }

        public override void WriteActualValueTo(NUnitCtr.MessageWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine(Failure.RenderActual());
        }

        public override void WriteMessageTo(NUnitCtr.MessageWriter writer)
        {
            writer.WritePredicate("Execution of the query doesn't match the expected result");
            writer.WriteLine();
            writer.WriteLine();
            base.WriteMessageTo(writer);
            writer.WriteLine();
            writer.WriteLine(Failure.RenderCompared());
        }

        private void doPersist(ResultSet resultSet, string path)
        {
            var writer = new ResultSetCsvWriter(System.IO.Path.GetDirectoryName(path));
            writer.Write(System.IO.Path.GetFileName(path), resultSet);
        }

        internal bool IsParallelizeQueries()
        {
            return parallelizeQueries;
        }
    }
}
