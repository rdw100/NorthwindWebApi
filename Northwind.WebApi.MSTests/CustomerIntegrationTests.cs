using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindWebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.WebApi.MsTests
{
    [TestClass]
    public class CustomerIntegrationTests
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;
        private const string STORE = "appsettings.json";
        private const string DIR = "..\\..\\..\\..\\";
        private const string APP = "NorthwindWebApi\\";
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

        [TestMethod]
        public void CustomerGetAllTests()
        {
            //Arrange
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), $"https://localhost/api/customers/");

            //Act
            HttpResponseMessage response = _client.SendAsync(request).Result;

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}