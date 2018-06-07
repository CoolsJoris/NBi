﻿using NBi.Core.Calculation.Grouping.ColumnBased;
using NBi.Core.ResultSet;
using NBi.Core.ResultSet.Resolver;
using NBi.Core.Scalar.Comparer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NBi.Core.ResultSet.SettingsIndexResultSet;

namespace NBi.Testing.Unit.Core.Calculation.Grouping.ColumnBased
{
    public class IndexByColumnGroupingTest
    {
        [Test]
        public void Execute_SingleColumn_TwoGroups()
        {
            var args = new ObjectsResultSetResolverArgs(new[] { new object[] { "alpha", 1 }, new object[] { "alpha", 2 }, new object[] { "beta", 3 }, new object[] { "alpha", 4 } });
            var resolver = new ObjectsResultSetResolver(args);
            var rs = resolver.Execute();

            var settings = new SettingsIndexResultSet(KeysChoice.First, ValuesChoice.None, NumericAbsoluteTolerance.None);
            var grouping = new IndexByColumnGrouping(settings);

            var result = grouping.Execute(rs);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[new KeyCollection(new[] { "alpha" })].Rows, Has.Count.EqualTo(3));
            Assert.That(result[new KeyCollection(new[] { "beta" })].Rows, Has.Count.EqualTo(1));
        }

        [Test]
        public void Execute_TwoColumns_ThreeGroups()
        {
            var args = new ObjectsResultSetResolverArgs(new[] { new object[] { "alpha", "1", 10 }, new object[] { "alpha", "1", 20 }, new object[] { "beta", "2", 30 }, new object[] { "alpha", "2", 40 } });
            var resolver = new ObjectsResultSetResolver(args);
            var rs = resolver.Execute();

            var settings = new SettingsIndexResultSet(KeysChoice.AllExpectLast, ValuesChoice.None, NumericAbsoluteTolerance.None);
            var grouping = new IndexByColumnGrouping(settings);

            var result = grouping.Execute(rs);
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[new KeyCollection(new object[] { "alpha", "1" })].Rows, Has.Count.EqualTo(2));
            Assert.That(result[new KeyCollection(new object[] { "alpha", "2" })].Rows, Has.Count.EqualTo(1));
            Assert.That(result[new KeyCollection(new object[] { "beta", "2" })].Rows, Has.Count.EqualTo(1));
        }

        [Test]
        public void Execute_TwoCustomColumns_ThreeGroups()
        {
            var args = new ObjectsResultSetResolverArgs(new[] { new object[] { "alpha", 1d, 10 }, new object[] { "alpha", 1, 20 }, new object[] { "beta", 2, 30 }, new object[] { "alpha", 2, 40 } });
            var resolver = new ObjectsResultSetResolver(args);
            var rs = resolver.Execute();

            var settings = new SettingsIndexResultSet(new List<IColumnDefinition>()
                {
                    new Column() { Index = 0, Role = ColumnRole.Key, Type = ColumnType.Text },
                    new Column() { Index = 1, Role = ColumnRole.Key, Type = ColumnType.Numeric },
                }
            );
            var grouping = new IndexByColumnGrouping(settings);

            var result = grouping.Execute(rs);
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[new KeyCollection(new object[] { "alpha", 1m })].Rows, Has.Count.EqualTo(2));
            Assert.That(result[new KeyCollection(new object[] { "alpha", 2m })].Rows, Has.Count.EqualTo(1));
            Assert.That(result[new KeyCollection(new object[] { "beta", 2m })].Rows, Has.Count.EqualTo(1));
        }
    }
}
