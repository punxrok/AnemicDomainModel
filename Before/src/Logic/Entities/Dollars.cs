using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Logic.Entities
{
    public class Dollars : ValueObject<Dollars>
    {
        private const decimal MaxDollarAmount = 1_000_000;
        public decimal Value { get; }

        private Dollars(decimal value)
        {
            Value = value;
        }

        public static Result<Dollars> Create(decimal dollarAmount)
        {
            if (dollarAmount <= 0)
            {
                return Result.Fail<Dollars>("Dollar amount cannot be negative");
            }

            if (dollarAmount > MaxDollarAmount)
            {
                return Result.Fail<Dollars>($"Dollar amount cannot be greater than {MaxDollarAmount}");
            }

            if (dollarAmount % 0.01m > 0)
            {
                return Result.Fail<Dollars>("Dollar amount cannot contain a part of penny");
            }

            return Result.Ok(new Dollars(dollarAmount));
        }

        protected override bool EqualsCore(Dollars other)
        {
            return this.Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }

        public static implicit operator decimal(Dollars dollars) // decimal name = dollarsInstance
        {
            return dollars.Value;
        }

        //public static explicit operator Dollars(decimal dollars) // Dollars cn = (Dollars) "ime"
        //{
        //    return Dollars.Create(dollars).Value;
        //}

        public static Dollars operator *(Dollars dollars, decimal multiplier) // decimal name = dollarsInstance
        {
            return new Dollars(dollars.Value * multiplier);
        }

        public static Dollars operator +(Dollars dollars, Dollars dollars2) // decimal name = dollarsInstance
        {
            return new Dollars(dollars.Value + dollars2.Value);
        }

        public static Dollars Of(decimal dollars)
        {
            return Dollars.Create(dollars).Value;
        }
    }
}
