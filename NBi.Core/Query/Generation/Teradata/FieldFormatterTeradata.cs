﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Query.Generation.Teradata
{
    class FieldFormatterTeradata : BaseFieldFormatter
    {
        public FieldFormatterTeradata()
        : base('"', '"') { }
    }
}
