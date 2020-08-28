using System;
using System.ComponentModel.DataAnnotations;

namespace HitPointsTracker.Models
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class IntMinimumValidationAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => false;

        public int MinValue { get; }

        public IntMinimumValidationAttribute(int minValue) =>
            MinValue = minValue;

        public override bool IsValid(object obj) =>
		    (obj is int value) && (value >= MinValue);
    }
}
