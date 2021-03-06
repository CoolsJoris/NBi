﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Core.Transformation.Transformer.Native
{
    class UtcToLocal : INativeTransformation
    {
        public string TimeZoneLabel { get; }

        public UtcToLocal(string timeZoneLabel)
        {
            TimeZoneLabel = timeZoneLabel;
        }

        public object Evaluate(object value)
        {
            if (value == null)
                return null;
            else if (value is DateTime)
                return EvaluateDateTime((DateTime)value);
            else
                throw new NotImplementedException();
        }

        protected virtual object EvaluateDateTime(DateTime value) =>
            TimeZoneInfo.ConvertTimeFromUtc(value, InstantiateTimeZoneInfo(TimeZoneLabel));

        protected TimeZoneInfo InstantiateTimeZoneInfo(string label)
        {
            var zones = TimeZoneInfo.GetSystemTimeZones();
            var zone = zones.SingleOrDefault(z => z.Id == label)
                ?? zones.SingleOrDefault(z => Tokenize(z.DisplayName).Contains(label.Replace(" ", "")));

            return zone ?? throw new ArgumentOutOfRangeException($"TimeZone '{label}' is not existing on this computer.");
        }

        private string[] Tokenize(string label) =>
            label.Replace("(", ",")
            .Replace(")", ",")
            .Replace(":", ",")
            .Replace(" ", "")
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        
    }
}
