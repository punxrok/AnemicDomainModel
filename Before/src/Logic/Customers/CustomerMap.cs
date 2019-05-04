using System;
using FluentNHibernate.Mapping;
using Logic.Entities;

namespace Logic.Mappings
{
    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Id(x => x.Id);

            Map(x => x.Name).CustomType<string>().Access.CamelCaseField(Prefix.Underscore);
            Map(x => x.Email).CustomType<string>().Access.CamelCaseField(Prefix.Underscore); //private string _email
            //Map(x => x.Status).CustomType<int>();
            //Map(x => x.StatusExpirationDate).CustomType<DateTime?>().Access.CamelCaseField(Prefix.Underscore).Nullable(); //private DateTime? _statusExpirationDate
            Map(x => x.MoneySpent).CustomType<decimal>().Access.CamelCaseField(Prefix.Underscore); //private decimal _moneySpent

            Component(x => x.Status, y =>
            {
                y.Map(x => x.Type, "Status").CustomType<int>();
                y.Map(x => x.ExpirationDate, "StatusExpirationDate").CustomType<DateTime?>()
                    .Access.CamelCaseField(Prefix.Underscore)
                    .Nullable(); //private DateTime? _statusExpirationDate
            });

            HasMany(x => x.PurchasedMovies).Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
