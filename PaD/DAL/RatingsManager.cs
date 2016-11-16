using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using Fooz.Logging;
using Fooz.Caching;

using PaD.Models;
using PaD.DAL.Ratings;
using PaD.DAL.EntityBase;
using PaD.DataContexts;
using PaD.ViewModels;
using PaD.Infrastructure;

namespace PaD.DAL
{
    public class RatingsManager : EntityManagerBase<PhotoRating>
    {
        #region Constructors
        public RatingsManager() : base() { }

        // Constructor that takes an IDbContext and an ILogger. NInject will create instances for us.
        public RatingsManager(IDbContext context, ILoggerProvider logger, ICacheProvider cache) : base(context, logger, cache) { }
        #endregion

        public async Task<double> AddOrUpdateRatingAsync(int photoId, string userName, double value)
        {
            AddOrUpdateRating addOrUpdateRating = new AddOrUpdateRating(DatabaseContext)
            {
                PhotoId = photoId,
                UserName = userName,
                Value = value
            };

            double newAverage = await addOrUpdateRating.ExecuteAsync();

            return newAverage;
        }

        public async Task<double> GetUserRatingValueAsync(int photoId, string userName)
        {
            GetUserRatingValue GetUserRatingValue = new GetUserRatingValue(DatabaseContext)
            {
                PhotoId = photoId,
                UserName = userName
            };

            double value = await GetUserRatingValue.ExecuteAsync();

            return value;
        }
    }
}