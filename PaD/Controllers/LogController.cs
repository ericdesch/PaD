using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Fooz.Logging;

using PaD.Infrastructure;
using PaD.DataContexts;
using Fooz.Caching;

namespace PaD.Controllers
{
    public class LogController : ControllerBase
    {
        #region Constructor
        public LogController(IDbContext dbContext, ILoggerProvider loggerProvider, ICacheProvider cacheProvider) 
            : base(dbContext, loggerProvider, cacheProvider)
        { }
        #endregion

        // GET: Log
        public void Error(string message)
        {
            Logger.Log(LogLevel.Error, message);
        }
    }
}