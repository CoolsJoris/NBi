﻿using NBi.Core.Scalar.Resolver;
using NBi.Core.Variable;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Testing.Unit.Core.Scalar.Resolver
{
    public class EnvironmentScalarResolverTest
    {
        [SetUp]
        public void CreateEnvironmentVariable() => Environment.SetEnvironmentVariable("NBiTesting", "the_value", EnvironmentVariableTarget.User);
        [TearDown]
        public void DeleteEnvironmentVariable() => Environment.SetEnvironmentVariable("NBiTesting", null, EnvironmentVariableTarget.User);

        [Test]
        public void Instantiate_GetValueObject_CorrectRead()
        {
            var args = new EnvironmentScalarResolverArgs("NBiTesting");
            var resolver = new EnvironmentScalarResolver<string>(args);

            var output = resolver.Execute();

            Assert.That(output, Is.EqualTo("the_value"));
        }
    }
}
