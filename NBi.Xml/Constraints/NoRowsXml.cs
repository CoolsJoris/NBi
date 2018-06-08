﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using NBi.Core;
using NBi.Core.ResultSet;
using NBi.Core.Scalar.Comparer;
using NBi.Xml.Items;
using NBi.Xml.Items.ResultSet;
using NBi.Xml.Settings;
using NBi.Xml.Constraints.Comparer;
using NBi.Xml.Items.Calculation;
using NBi.Core.Evaluate;
using System;
using NBi.Xml.Items.Calculation.Predication;

namespace NBi.Xml.Constraints
{
    public class NoRowsXml : AbstractConstraintXml
    {
        [XmlIgnore()]
        public List<IColumnAlias> Aliases
        {
            get
            {
                return InternalAliases.ToList<IColumnAlias>();
            }
        }

        [XmlElement("expression")]
        public List<ExpressionXml> Expressions { get; set; }

        [XmlElement("alias")]
        public List<AliasXml> InternalAliases
        {
            get { return internalAliases; }
            set { internalAliases = value; }
        }

        [XmlIgnore]
        [Obsolete("Use InternalAlias in place of InternalAliasOld")]
        public List<AliasXml> InternalAliasesOld
        {
            get { return internalAliases; }
            set { internalAliases = value; }
        }

        private List<AliasXml> internalAliases;

        [XmlElement("predicate")]
        public SinglePredicationXml Predication { get; set; }

        [XmlElement("combination")]
        public CombinationPredicationXml Combination { get; set; }

        public NoRowsXml()
        {
            internalAliases = new List<AliasXml>();
            Expressions = new List<ExpressionXml>();
        }
    }
}
