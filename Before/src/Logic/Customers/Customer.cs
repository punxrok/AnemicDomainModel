using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;

namespace Logic.Entities
{
    public class Customer : Entity
    {
        protected Customer()
        {
            _purchasedMovies = new List<PurchasedMovie>();
        }

        public Customer(CustomerName name, CustomerEmail email) : this()
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            MoneySpent = Dollars.Of(0);
            Status = CustomerStatus.Regular;
        }

        private string _name;
        //[Required]
        //[MaxLength(100, ErrorMessage = "Name is too long")]
        public virtual CustomerName Name
        {
            get => (CustomerName)_name; //explicit operator
            set => _name = value;
        } //implicit operator

        private readonly string _email;
        //[Required]
        //[RegularExpression(@"^(.+)@(.+)$", ErrorMessage = "Email is invalid")]
        public virtual CustomerEmail Email
        {
            get => CustomerEmail.Create(_email).Value;
            //protected set => _email = value.Value;
        }

        //[JsonConverter(typeof(StringEnumConverter))]
        public virtual CustomerStatus Status { get; protected set; }


        private decimal _moneySpent;

        public virtual Dollars MoneySpent
        {
            get => Dollars.Of(_moneySpent);
            protected set => _moneySpent = value;
        }

        private readonly IList<PurchasedMovie> _purchasedMovies;
        public virtual IReadOnlyList<PurchasedMovie> PurchasedMovies
        {
            get => _purchasedMovies.ToList();
        }

        public virtual bool HasPurchasedMovie(Movie movie)
        {
            return PurchasedMovies.Any(x => x.Movie == movie && x.ExpirationDate.IsExpired);
        }
        public virtual void PurchaseMovie(Movie movie)
        {
            if (HasPurchasedMovie(movie))
                throw new Exception();

            ExpirationDate expirationDate = movie.GetExpirationDate();
            Dollars price = movie.CalculatePrice(Status);

            var purchasedMovie = new PurchasedMovie(
                movie,
                this,
                price,
                expirationDate
            );

            _purchasedMovies.Add(purchasedMovie);
            this.MoneySpent += price;
        }

        public virtual Result CanPromote()
        {
            if (Status.IsAdvanced)
                return Result.Fail("the customer already has the advanced status");

            // at least 2 active movies during the last 30 days
            if (PurchasedMovies.Count(x => x.ExpirationDate == ExpirationDate.Infinite || x.ExpirationDate.Date >= DateTime.UtcNow.AddDays(-30)) < 2)
                return Result.Fail("the customer has to have at least 2 active movies during the last 30 days");

            // at least 100 dollars spent during the last year
            if (PurchasedMovies.Where(x => x.PurchaseDate > DateTime.UtcNow.AddYears(-1)).Sum(x => x.Price) < 100m)
                return Result.Fail("the customer had to spent at least 100 dollars  during the last year");

            return Result.Ok();
        }
        public virtual void Promote()
        {
           if(CanPromote().IsFailure) throw new Exception();

            Status = Status.Promote();
        }
    }
}
