﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Fooz.Logging;
using Fooz.Caching;

using PaD;
using PaD.Controllers;
using PaD.DataContexts;
using PaD.Infrastructure;

namespace PaD.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        readonly IDbContext databaseContext = new PaDDb();
         ILoggerProvider logger = (ILoggerProvider)DependencyResolver.Current.GetService(typeof(ILoggerProvider));
         ICacheProvider cache = (ICacheProvider)DependencyResolver.Current.GetService(typeof(ICacheProvider));

        [TestMethod]
        public async Task Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = await controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}