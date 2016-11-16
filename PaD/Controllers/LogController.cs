using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Fooz.Logging;

using PaD.Infrastructure;

namespace PaD.Controllers
{
    public class LogController : ControllerBase
    {
        // GET: Log
        public void Error(string message)
        {
            Logger.Log(LogLevel.Error, message);
        }
    }
}