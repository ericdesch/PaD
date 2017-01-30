using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Fooz.Logging;
using Fooz.Caching;

using PaD.Controllers;
using PaD.DataContexts;

namespace PaD.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest : IDisposable
    {
        #region Private Member Variables
        private IDbContext _databaseContext;
        private ILoggerProvider _logger;
        private ICacheProvider _cache;

        private HttpServer _server;
        private string _urlBase = "http://localhost/";
        #endregion

        #region Constructors
        public HomeControllerTest(IDbContext dbContext, ILoggerProvider loggerProvider, ICacheProvider cacheProvider) : this()
        {
            _databaseContext = dbContext;
            _logger = loggerProvider;
            _cache = cacheProvider;
        }

        public HomeControllerTest()
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(name: "Default", routeTemplate: "api/{controller}/{id}", defaults: new { id = RouteParameter.Optional });
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            //config.MessageHandlers.Add(new WebApiKeyHandler());

            _server = new HttpServer(config);
        }
        #endregion

        [TestMethod]
        public async Task InMemoryApiTest()
        {
            // Arrange
            var client = new HttpClient(_server);
            var request = CreateRequest("api/TestApi", "application/json", HttpMethod.Get);
            var expectedJson = "[\"value1\",\"value2\"]";

            // Act
            using (HttpResponseMessage response = await client.SendAsync(request))
            {
                string contentString = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.IsNotNull(response.Content);
                Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
                Assert.AreEqual(expectedJson, contentString);
            }

            request.Dispose();
        }

        [TestMethod]
        public async Task Index()
        {
            // Arrange
            HomeController controller = new HomeController(_databaseContext, _logger, _cache);

            // Act
            ViewResult result = await controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController(_databaseContext, _logger, _cache);

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController(_databaseContext, _logger, _cache);

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        private HttpRequestMessage CreateRequest(string url, string mthv, HttpMethod method)
        {
            var request = new HttpRequestMessage();

            request.RequestUri = new Uri(_urlBase + url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mthv));
            request.Method = method;

            return request;
        }

        private HttpRequestMessage CreateRequest<T>(string url, string mthv, HttpMethod method, T content, MediaTypeFormatter formatter) where T : class
        {
            HttpRequestMessage request = CreateRequest(url, mthv, method);
            request.Content = new ObjectContent<T>(content, formatter);

            return request;
        }

        public void Dispose()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
        }

    }
}
