using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

using PaD.Models;
using PaD.DataContexts;

namespace PaD.DAL.Ratings
{
    public class GetUserRatingValue
    {
        private IPaDDb _context;

        public int PhotoId { get; set; }
        public string UserName { get; set; }

        public GetUserRatingValue(IDbContext databaseContext)
        {
            // allow it to be injected
            _context = databaseContext as IPaDDb;
        }

        public async Task<double> ExecuteAsync()
        {
            var value = await _context.PhotoRating
                .Where(r => r.PhotoId == PhotoId && r.IdentityUserName == UserName)
                .Select(r => r.Value)
                .FirstOrDefaultAsync();

            return value;
        }
    }
}