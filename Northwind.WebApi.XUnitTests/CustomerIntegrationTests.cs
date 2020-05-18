using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NorthwindWebApi;
using NorthwindWebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Northwind.WebApi.XUnitTests
{
    public class CustomerIntegrationTests
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;
        private const string STORE = "appsettings.json";
        private const string DIR = "..\\..\\..\\..\\";
        private const string APP = "NorthwindWebApi\\";
        private const string URL = "http://localhost/api/customers";
        private string projectPath = string.Empty;
        private string projectDir = string.Empty;

        public CustomerIntegrationTests()
        {
            projectPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, DIR));
            projectDir = Path.Combine(projectPath, APP);

            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseContentRoot(projectDir)
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(projectDir)
                    .AddJsonFile(STORE)
                    .Build()
                )
                .UseStartup<Startup>());

            // Add config for client
            _client = _server.CreateClient();
            _client.BaseAddress = new Uri("https://localhost");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #region Methods(Public)
        [Fact]
        public void CustomerHttpGet_AllCustomers_200OK()
        {
            //Arrange
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/customers/");

            //Act
            HttpResponseMessage response = _client.SendAsync(request).Result;

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("ALFKI")]
        public void CustomerHttpGet_CustomerById_200OK(string id)
        {
            //Arrange
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/customers/{id}");

            //Act
            HttpResponseMessage response = _client.SendAsync(request).Result;

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("ALFKI")]
        public void CustomerHttpPut_CustomerById_204NoContent(string id)
        {
            //Arrange
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/customers/{id}");
            HttpResponseMessage response = _client.SendAsync(request).Result;
            request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/customers/{id}")
            {
                Content = response.Content
            };

            //Act
            HttpResponseMessage responsePut = _client.SendAsync(request).Result;

            //Assert
            Assert.Equal(HttpStatusCode.NoContent, responsePut.StatusCode);
        }

        [Theory]
        [InlineData("ZUCCA")]
        public async Task CustomerHttpPost_CreateCustomer_201NoContent(string id)
        {
            //Arrange
            HttpResponseMessage responseGet = GetCustomer(id);

            //Act
            if (HttpStatusCode.OK == responseGet.StatusCode)
            {
                HttpResponseMessage responseDelete = DeleteCustomer(id);
                HttpResponseMessage responsePost = await CreateCustomer();
                DeleteCustomer(id);

                //Assert
                Assert.Equal(HttpStatusCode.OK, responseDelete.StatusCode);
                Assert.Equal(HttpStatusCode.Created, responsePost.StatusCode);
            }
            else
            {
                HttpResponseMessage responsePost = await CreateCustomer();
                HttpResponseMessage responseDelete = DeleteCustomer(id);

                //Assert
                Assert.Equal(HttpStatusCode.OK, responseDelete.StatusCode);
                Assert.Equal(HttpStatusCode.Created, responsePost.StatusCode);
            }
        }

        [Fact]
        public async void CustomerHttpPost_CreateCustomerError_400BadRequest()
        {
            //Arrange
            Customer customer = new Customer
            {
                CustomerId = "ARGOS",
                CompanyName = "ARGONAUT GYROS  1892234567893123456789412345678951234567896",
                ContactName = "Fiore",
                ContactTitle = "CEO",
                Address = "123 Kolokythokeftedes",
                City = "Tallahassee",
                Region = null,
                PostalCode = "32301",
                Country = "USA",
                Phone = "555-555-5555",
                Fax = "555-555-5555",
                Orders = null
            };

            var json = JsonConvert.SerializeObject(customer);

            //Act
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(URL, data);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("ZUCCA")]
        public async Task CustomerHttpDelete_CustomerById_200OKAsync(string id)
        {
            //Arrange
            HttpResponseMessage responseGet = GetCustomer(id);

            //Act
            if (HttpStatusCode.OK == responseGet.StatusCode)
            {
                HttpResponseMessage responseDelete = DeleteCustomer(id);

                //Assert
                Assert.Equal(HttpStatusCode.OK, responseDelete.StatusCode);
            }
            else
            {
                HttpResponseMessage responsePost = await CreateCustomer();
                HttpResponseMessage responseDelete = DeleteCustomer(id);

                //Assert
                Assert.Equal(HttpStatusCode.Created, responsePost.StatusCode);
                Assert.Equal(HttpStatusCode.OK, responseDelete.StatusCode);
            };
        }
        #endregion

        #region Methods(Private)
        private HttpResponseMessage GetCustomer(string id)
        {
            HttpRequestMessage requestGet = new HttpRequestMessage(new HttpMethod("GET"), $"/api/customers/{id}");
            HttpResponseMessage responseGet = _client.SendAsync(requestGet).Result;
            return responseGet;
        }

        private async Task<HttpResponseMessage> CreateCustomer()
        {
            Customer customerPost = new Customer
            {
                CustomerId = "ZUCCA",
                CompanyName = "Pumpkin Squash",
                ContactName = "Fiore",
                ContactTitle = "CEO",
                Address = "123 Winter Squash",
                City = "Milan",
                Region = null,
                PostalCode = "20121",
                Country = "Italy",
                Phone = "55 55-5555",
                Fax = "55 55-555555555",
                Orders = null
            };

            string json = JsonConvert.SerializeObject(customerPost);

            StringContent dataPost = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage responsePost = await _client.PostAsync(URL, dataPost);

            return responsePost;
        }

        private HttpResponseMessage DeleteCustomer(string id)
        {
            HttpRequestMessage requestDelete = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/customers/{id}");
            HttpResponseMessage responseDelete = _client.SendAsync(requestDelete).Result;

            return responseDelete;
        }
        #endregion
    }
}
