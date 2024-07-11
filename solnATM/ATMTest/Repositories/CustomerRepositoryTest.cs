using ATMApplication.Exceptions;
using ATMApplication.Models;
using ATMApplication.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMTest.Repositories
{
    public class CustomerRepositoryTest
    {
        ATMContext context;
        IRepository<int, Customer> customerRepository;

        [SetUp]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ATMContext>().UseInMemoryDatabase("CustomerDB");
            context = new ATMContext(optionsBuilder.Options);
            customerRepository = new CustomerRepository(context);
        }

        [TearDown]
        public void TearDown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Test]
        public async Task AddCustomerSuccessTest()
        {
            // Action
            var result = await customerRepository.Add(new Customer
            {
                Id = 1,
                Name = "John Doe",
                Age = 30,
                Gender = "Male",
                Address = "123 Main St",
                Phone = "1234567890"
            });

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetByCustomerIdSuccessTest()
        {
            // Arrange
            int customerId = 1;
            await customerRepository.Add(new Customer
            {
                Id = customerId,
                Name = "John Doe",
                Age = 30,
                Gender = "Male",
                Address = "123 Main St",
                Phone = "1234567890"
            });

            // Action
            var result = await customerRepository.GetById(customerId);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetByCustomerIdFailTest()
        {
            // Arrange
            int customerId = 1;

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => customerRepository.GetById(customerId));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity not found!"));
        }

        [Test]
        public async Task GetAllCustomersSuccessTest()
        {
            // Arrange
            int customerId = 1;
            await customerRepository.Add(new Customer
            {
                Id = customerId,
                Name = "John Doe",
                Age = 30,
                Gender = "Male",
                Address = "123 Main St",
                Phone = "1234567890"
            });

            // Action
            var result = await customerRepository.GetAll();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteCustomerByIdSuccessTest()
        {
            // Arrange
            int customerId = 1;
            await customerRepository.Add(new Customer
            {
                Id = customerId,
                Name = "John Doe",
                Age = 30,
                Gender = "Male",
                Address = "123 Main St",
                Phone = "1234567890"
            });

            // Action
            var result = await customerRepository.Delete(customerId);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task DeleteCustomerByIdFailTest()
        {
            // Arrange
            int customerId = 1;

            // Action
            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => customerRepository.Delete(customerId));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Entity not found!"));
        }

        [Test]
        public async Task UpdateCustomerFailTest()
        {
            // Arrange
            int customerId = 1;

            // Action
            var exception = Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => customerRepository.Update(new Customer
            {
                Id = customerId,
                Name = "John Doe",
                Age = 30,
                Gender = "Male",
                Address = "123 Main St",
                Phone = "1234567890"
            }));

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public async Task UpdateCustomerSuccessTest()
        {
            // Arrange
            int customerId = 1;
            var customer1 = new Customer
            {
                Id = customerId,
                Name = "John Doe",
                Age = 30,
                Gender = "Male",
                Address = "123 Main St",
                Phone = "1234567890"
            };
            await customerRepository.Add(customer1);
            var customer2 = await customerRepository.GetById(customerId);
            customer2.Age = 35;

            // Action
            var result = await customerRepository.Update(customer2);

            // Assert
            Assert.That(result.Age, Is.EqualTo(35));
        }
    }
}
