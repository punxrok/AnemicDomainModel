using CSharpFunctionalExtensions;
using Logic.Dtos;
using Logic.Entities;
using Logic.Repositories;
using Logic.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : BaseController
    {
        private readonly MovieRepository _movieRepository;
        private readonly CustomerRepository _customerRepository;

        public CustomersController(UnitOfWork unitOfWork, MovieRepository movieRepository, CustomerRepository customerRepository)
        : base(unitOfWork)
        {
            _customerRepository = customerRepository;
            _movieRepository = movieRepository;
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(long id)
        {
            Customer customer = _customerRepository.GetById(id);
            if (customer == null)
                return NotFound();


            var customerDto = new CustomerDto
            {
                Id = customer.Id,
                Email = customer.Email.Value,
                MoneySpent = customer.MoneySpent,
                Name = customer.Name.Value,
                Status = customer.Status.Type.ToString(),
                StatusExpirationDate = customer.Status.ExpirationDate,
                PurchasedMovies = customer.PurchasedMovies.Select(s => new PurchasedMovieDto()
                {
                    ExpirationDate = s.ExpirationDate,
                    Price = s.Price,
                    PurchaseDate = s.PurchaseDate,

                    Movie = new MovieDto()
                    {
                        Id = s.Movie.Id,
                        Name = s.Movie.Name
                    }
                }).ToList()
            };

            return Ok(customerDto);
        }

        [HttpGet]
        public IActionResult GetList()
        {
            IReadOnlyList<Customer> customers = _customerRepository.GetList();
            var dtos = customers.Select(c => new CustomerInListDto
            {
                Id = c.Id,
                Name = c.Name.Value,
                Email = c.Email.Value,
                Status = c.Status.Type.ToString(),
                StatusExpirationDate = c.Status.ExpirationDate,
                MoneySpent = c.MoneySpent
            }).ToList();

            return Ok(dtos);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateCustomerDto item)
        {
            //try
            //{
            var customerNameOrError = CustomerName.Create(item.Name);
            var customerEmailOrError = CustomerEmail.Create(item.Email);

            var result = Result.Combine(customerEmailOrError, customerEmailOrError);

            if (result.IsFailure)
            {
                return Error(result.Error);
            }

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (_customerRepository.GetByEmail(customerEmailOrError.Value) != null)
            {
                return Error("Email is already in use: " + item.Email);
            }


            var customer = new Customer(customerNameOrError.Value, customerEmailOrError.Value);
            _customerRepository.Add(customer);
            //_customerRepository.SaveChanges();

            return Ok();
            //}
            //catch (Exception e)
            //{
            //    return StatusCode(500, new { error = e.Message });
            //}
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(long id, [FromBody] UpdateCustomerDto item)
        {
            //try
            //{
            var customerNameOrError = CustomerName.Create(item.Name);
            if (customerNameOrError.IsFailure)
            {
                return Error(customerNameOrError.Error);
            }

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            Customer customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return Error("Invalid customer id: " + id);
            }

            customer.Name = customerNameOrError.Value;
            //_customerRepository.SaveChanges();

            return Ok();
            //}
            //catch (Exception e)
            //{
            //    return StatusCode(500, new { error = e.Message });
            //}
        }

        [HttpPost]
        [Route("{id}/movies")]
        public IActionResult PurchaseMovie(long id, [FromBody] long movieId)
        {
            //try
            //{
            Movie movie = _movieRepository.GetById(movieId);
            if (movie == null)
            {
                return Error("Invalid movie id: " + movieId);
            }

            Customer customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return Error("Invalid customer id: " + id);
            }

            if (customer.HasPurchasedMovie(movie))
            {
                return Error("The movie is already purchased: " + movie.Name);
            }

            customer.PurchaseMovie(movie);

            //_customerRepository.SaveChanges();

            return Ok();
            //}
            //catch (Exception e)
            //{
            //    return StatusCode(500, new { error = e.Message });
            //}
        }

        [HttpPost]
        [Route("{id}/promotion")]
        public IActionResult PromoteCustomer(long id)
        {
            //try
            //{
            Customer customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return Error("Invalid customer id: " + id);
            }

            //if (customer.Status.IsAdvanced)
            //{
            //    return Error("The customer already has the Advanced status");
            //}

            var result = customer.CanPromote();
            if (result.IsFailure)
            {
                return Error(result.Error);
            }

            customer.Promote();
            //_customerRepository.SaveChanges();

            return Ok();
            //}
            //catch (Exception e)
            //{
            //    return StatusCode(500, new { error = e.Message });
            //}
        }
    }
}
