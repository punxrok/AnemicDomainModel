using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;

namespace Logic.Entities
{
    public class Customer : Entity
    {
        

        private string _name;
        //[Required]
        //[MaxLength(100, ErrorMessage = "Name is too long")]
        public virtual CustomerName Name
        {
            get => (CustomerName)_name; //explicit operator
            set => _name = value; } //implicit operator

        private string _email;
        //[Required]
        //[RegularExpression(@"^(.+)@(.+)$", ErrorMessage = "Email is invalid")]
        public virtual CustomerEmail Email
        {
            get => CustomerEmail.Create(_email).Value;
            set => _email = value.Value;
        }

        //[JsonConverter(typeof(StringEnumConverter))]
        public virtual CustomerStatus Status { get; set; }

        private DateTime? _statusExpirationDate;

        public virtual ExpirationDate StatusExpirationDate
        {
            get => (ExpirationDate) _statusExpirationDate;
            set => _statusExpirationDate = value;
        }

        private decimal _moneySpent;

        public virtual Dollars MoneySpent
        {
            get => Dollars.Of(_moneySpent);
            set => _moneySpent = value;
        }

        public virtual IList<PurchasedMovie> PurchasedMovies { get; set; }
    }
}
