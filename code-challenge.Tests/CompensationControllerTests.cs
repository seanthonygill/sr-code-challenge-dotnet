using challenge.Controllers;
using challenge.Data;
using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        /*
         * CreateCompensation_Returns_Created
         * 
         * Tests Compensation object creation and persistence
         * 
         */
        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var getRequestTask = _httpClient.GetAsync($"api/employee/16a596ae-edd3-4847-99fe-c4518e82c86f");
            var resp = getRequestTask.Result;
            var employee = resp.DeserializeContent<Employee>();

            var compensation = new Compensation()
            {
                Employee = employee,
                Salary = 80000,
                EffectiveDate = new DateTime(2017, 02, 15)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.Employee.EmployeeId);
            Assert.AreEqual(compensation.Employee.FirstName, newCompensation.Employee.FirstName);
            Assert.AreEqual(compensation.Employee.LastName, newCompensation.Employee.LastName);
            Assert.AreEqual(compensation.Employee.Department, newCompensation.Employee.Department);
            Assert.AreEqual(compensation.Employee.Position, newCompensation.Employee.Position);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);

        }


        /*
         * GetCompensationByEmployeeId_Returns_Ok
         * 
         * Tests Compensation object retrieval from persistant layer
         * 
         * NOTE: Since we are not using an initial load file for the In Memory Database
         * for Compensation objects, we will simply make a test that creates a
         * Compensation object through the API, then GETs that object through the
         * API as well
         * 
         */
        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_Ok()
        {
            // First, get the Employee object to use for Compensation
            var getRequestTask = _httpClient.GetAsync($"api/employee/62c1084e-6e34-4630-93fd-9153afb65309");
            var resp = getRequestTask.Result;
            var employee = resp.DeserializeContent<Employee>();

            // Create the Compensation object
            var compensation = new Compensation()
            {
                Employee = employee,
                Salary = 80000,
                EffectiveDate = new DateTime(2017, 02, 15)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Create the Compensation object in the DB by using the API call
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            // Retrieve the DB Compensation object using the employeeid in the API call
            var getReq = _httpClient.GetAsync($"api/compensation/{compensation.Employee.EmployeeId}");
            var res = getReq.Result;
            var comp = res.DeserializeContent<Compensation>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
            Assert.IsNotNull(comp.Employee.EmployeeId);
            Assert.AreEqual(compensation.Employee.FirstName, comp.Employee.FirstName);
            Assert.AreEqual(compensation.Employee.LastName, comp.Employee.LastName);
            Assert.AreEqual(compensation.Employee.Department, comp.Employee.Department);
            Assert.AreEqual(compensation.Employee.Position, comp.Employee.Position);
            Assert.AreEqual(compensation.EffectiveDate, comp.EffectiveDate);
            Assert.AreEqual(compensation.Salary, comp.Salary);

        }

    }
}
