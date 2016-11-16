using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using Fooz.Logging;
using Fooz.Caching;

using PaD.Models;
using PaD.DAL.Photos;
using PaD.DAL.EntityBase;
using PaD.DataContexts;
using PaD.ViewModels;
using PaD.Infrastructure;

namespace PaD.DAL
{
    public class PhotoImageManager : EntityManagerBase<PhotoImage>
    {
        #region Constructors
        public PhotoImageManager() : base() { }
        
        // Constructor that takes an IDbContext and an ILogger. NInject will create instances for us.
        public PhotoImageManager(IDbContext context, ILoggerProvider logger, ICacheProvider cache) : base(context, logger, cache) { }
        #endregion
    }
}