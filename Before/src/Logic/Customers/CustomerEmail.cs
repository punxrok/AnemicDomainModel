using CSharpFunctionalExtensions;
using System;
using System.Text.RegularExpressions;

namespace Logic.Entities
{
    public class CustomerEmail : ValueObject<CustomerName>
    {
        public CustomerEmail(string value)
        {
            Value = value;
        }

        public static Result<CustomerEmail> Create(string email)
        {
            email = (email ?? string.Empty).Trim();

            if (email.Length == 0)
                return Result.Fail<CustomerEmail>("Email should not be empty");

            if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
                return Result.Fail<CustomerEmail>("Email is invalid");        

            return Result.Ok(new CustomerEmail(email));
        }
        public string Value { get; set; }
        protected override bool EqualsCore(CustomerName other)
        {
            return Value.Equals(other.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }

        public static implicit operator string(CustomerEmail customerName) // string name = customerEmailInstance
        {
            return customerName.Value;
        }

        public static explicit operator CustomerEmail(string customerName) // CustomerEmail cn = (CustomerEmail) "ime"
        {
            return CustomerEmail.Create(customerName).Value;
        }
    }
}