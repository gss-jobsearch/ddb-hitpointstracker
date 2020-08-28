using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace HitPointsTracker.Models
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class IntValuesValidationAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => false;

        private readonly ImmutableHashSet<int> _acceptable;

        public IntValuesValidationAttribute(params int[] acceptable)
        {
            _acceptable = acceptable.ToImmutableHashSet();
        }

        public override bool IsValid(object obj) =>
            (obj is int value) && _acceptable.Contains(value);
    }
}
