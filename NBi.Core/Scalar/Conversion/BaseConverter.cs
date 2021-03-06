﻿using NBi.Core.Calculation;
using NBi.Core.Calculation.Predicate;
using NBi.Core.ResultSet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Scalar.Conversion
{
    abstract class BaseConverter<T, U> : IConverter
    {
        public object DefaultValue { get; }

        public Type DestinationType => typeof(U);

        private readonly IPredicate predicate;
        private readonly CultureInfo cultureInfo;

        public BaseConverter(CultureInfo cultureInfo, object defaultValue)
        {
            var info = GetPredicateInfo(cultureInfo);
            var predicateFactory = new PredicateFactory();
            predicate = predicateFactory.Instantiate(info);

            DefaultValue = defaultValue;
            this.cultureInfo = cultureInfo;
        }

        protected abstract IPredicateInfo GetPredicateInfo(CultureInfo cultureInfo);

        public object Execute(object x)
        {
            if (predicate.Execute(x) && x is T)
                return OnExecute((T)x, cultureInfo);
            else
                return DefaultValue;
        }

        protected abstract U OnExecute(T x, CultureInfo cultureInfo);
    }
}
