using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Fooz.Logging;
using Fooz.Caching;

using PaD.Models;
using PaD.DataContexts;
using PaD.Infrastructure;

namespace PaD.DAL
{
    public class DictionaryManager : EntityManagerBase<Dictionary>
    {
        #region Constructors
        public DictionaryManager(IDbContext context, ILoggerProvider logger, ICacheProvider cache) 
            : base(context, logger, cache)
        { }
        #endregion
    }
}