using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Logic.Entities
{
    public class ExpirationDate : ValueObject<ExpirationDate>
    {
        public static readonly ExpirationDate Infinite = new ExpirationDate(null);
        public DateTime? Date { get; set; }
        public bool IsExpired => Date != Infinite || Date < DateTime.UtcNow;

        private ExpirationDate(DateTime? date)
        {
            Date = date;
        }

        public static Result<ExpirationDate> Create(DateTime date)
        {
            return Result.Ok(new ExpirationDate(date));
        }

        
        protected override bool EqualsCore(ExpirationDate other)
        {
            return this.Date == other.Date;
        }

        protected override int GetHashCodeCore()
        {
            return Date.GetHashCode();
        }

        public static implicit operator DateTime?(ExpirationDate date) // string name = customerEmailInstance
        {
            return date.Date;
        }

        public static explicit operator ExpirationDate(DateTime? date) // CustomerEmail cn = (CustomerEmail) "ime"
        {
            return date.HasValue
                ? ExpirationDate.Create(date.Value).Value
                : ExpirationDate.Infinite;
        }
    }
}
