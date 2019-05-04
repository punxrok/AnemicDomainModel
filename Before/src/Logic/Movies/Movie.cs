using System;

namespace Logic.Entities
{
    public abstract class Movie : Entity
    {
        public virtual string Name { get; protected set; }
        //[JsonIgnore]
        protected virtual LicensingModel LicensingModel { get; set; }

        public abstract ExpirationDate GetExpirationDate();

        public virtual Dollars CalculatePrice(CustomerStatus status)
        {
            decimal modifier = 1 - status.GetDiscount();

            return GetBasePrice() * modifier;
        }

        protected abstract Dollars GetBasePrice();

    }

    public class LifeLongMovie : Movie
    {
        public override ExpirationDate GetExpirationDate()
        {
            return ExpirationDate.Infinite;
        }

        protected override Dollars GetBasePrice()
        {
            return Dollars.Of(8);
        }
    }


    public class TwoDaysMovie : Movie
    {
        public override ExpirationDate GetExpirationDate()
        {
            return (ExpirationDate)DateTime.UtcNow.AddDays(2);
        }

        protected override Dollars GetBasePrice()
        {
            return Dollars.Of(4);
        }
    }
}
