using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Logic.Dtos;
using Logic.Entities;
using Logic.Repositories;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly MovieRepository _movieRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly CustomerService _customerService;

        public CustomersController(MovieRepository movieRepository, CustomerRepository customerRepository, CustomerService customerService)
        {
            _customerRepository = customerRepository;
            _movieRepository = movieRepository;
            _customerService = customerService;
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
                Status = customer.Status.ToString(),
                StatusExpirationDate = customer.StatusExpirationDate,
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

            return Json(customerDto);
        }

        [HttpGet]
        public JsonResult GetList()
        {
            IReadOnlyList<Customer> customers = _customerRepository.GetList();
            return Json(customers.Select(c => new CustomerInListDto
            {
                Id = c.Id,
                Name = c.Name.Value,
                Email = c.Email.Value,
                Status = c.Status.ToString(),
                StatusExpirationDate = c.StatusExpirationDate,
                MoneySpent = c.MoneySpent
            }).ToList());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateCustomerDto item)
        {
            try
            {
                var customerNameOrError = CustomerName.Create(item.Name);
                var customerEmailOrError = CustomerEmail.Create(item.Email);

                var result = Result.Combine(customerEmailOrError,customerEmailOrError);

                if (result.IsFailure)
                {
                    return BadRequest(result.Error);
                }

                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}

                if (_customerRepository.GetByEmail(customerEmailOrError.Value) != null)
                {
                    return BadRequest("Email is already in use: " + item.Email);
                }

                
                var customer = new Customer
                {
                    Name = customerNameOrError.Value,
                    Email = customerEmailOrError.Value,
                    MoneySpent = 0,
                    Status = CustomerStatus.Regular,
                    StatusExpirationDate = null
                };

                _customerRepository.Add(customer);
                _customerRepository.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(long id, [FromBody] UpdateCustomerDto item)
        {
            try
            {
                var customerNameOrError = CustomerName.Create(item.Name);
                if (customerNameOrError.IsFailure)
                {
                    return BadRequest(customerNameOrError.Error);
                }

                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}

                Customer customer = _customerRepository.GetById(id);
                if (customer == null)
                {
                    return BadRequest("Invalid customer id: " + id);
                }

                customer.Name = customerNameOrError.Value;
                _customerRepository.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpPost]
        [Route("{id}/movies")]
        public IActionResult PurchaseMovie(long id, [FromBody] long movieId)
        {
            try
            {
                Movie movie = _movieRepository.GetById(movieId);
                if (movie == null)
                {
                    return BadRequest("Invalid movie id: " + movieId);
                }

                Customer customer = _customerRepository.GetById(id);
                if (customer == null)
                {
                    return BadRequest("Invalid customer id: " + id);
                }

                if (customer.PurchasedMovies.Any(x => x.MovieId == movie.Id && (x.ExpirationDate == null || x.ExpirationDate.Value >= DateTime.UtcNow)))
                {
                    return BadRequest("The movie is already purchased: " + movie.Name);
                }

                _customerService.PurchaseMovie(customer, movie);

                _customerRepository.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpPost]
        [Route("{id}/promotion")]
        public IActionResult PromoteCustomer(long id)
        {
            try
            {
                Customer customer = _customerRepository.GetById(id);
                if (customer == null)
                {
                    return BadRequest("Invalid customer id: " + id);
                }

                if (customer.Status == CustomerStatus.Advanced && (customer.StatusExpirationDate == null || customer.StatusExpirationDate.Value < DateTime.UtcNow))
                {
                    return BadRequest("The customer already has the Advanced status");
                }

                bool success = _customerService.PromoteCustomer(customer);
                if (!success)
                {
                    return BadRequest("Cannot promote the customer");
                }

                _customerRepository.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }
    }
}
