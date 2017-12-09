﻿using NBi.Core.Calculation;
using NBi.Core.Calculation.Predicate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NBi.Xml.Constraints.Comparer
{
    public class ModuloXml : PredicateXml, ITwoOperandsXml
    {
        [XmlAttribute("second-operand")]
        public string SecondOperand { get; set; }

        internal override ComparerType ComparerType { get => ComparerType.Modulo; }
    }
}
