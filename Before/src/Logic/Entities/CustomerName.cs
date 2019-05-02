using System;
using CSharpFunctionalExtensions;

namespace Logic.Entities
{
    public class CustomerName : ValueObject<CustomerName>
    {
        private CustomerName(string value)
        {
            Value = value;
        }

        public static Result<CustomerName> Create(string customerName)
        {
            customerName = (customerName ?? string.Empty).Trim();

            if (customerName.Length == 0)
                return Result.Fail<CustomerName>("Customer name should not be empty");

            if (customerName.Length > 100)
                return Result.Fail<CustomerName>("Customer name should not be larger than 100 characters");
                

            return Result.Ok(new CustomerName(customerName));
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

        public static implicit operator string(CustomerName customerName) // string name = customerNameInstance
        {
            return customerName.Value;
        }

        public static explicit operator CustomerName(string customerName) // CustomerName cn = (CustomerName) "ime"
        {
            return CustomerName.Create(customerName).Value;
        }

    }
}