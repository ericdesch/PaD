using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;

using PaD.Models;
using PaD.DataContexts;

namespace PaD.DAL.Ratings
{
    public class AddOrUpdateRating
    {
        private IPaDDb _context;

        public int PhotoId { get; set; }
        public string UserName { get; set; }
        public double Value { get; set; }

        public AddOrUpdateRating(IDbContext databaseContext)
        {
            // allow it to be injected
            _context = databaseContext as IPaDDb;
        }

        public async Task<double> ExecuteAsync()
        {
            // Call the spAddOrUpdateRating stored proceedure to add/update this value.
            // Stored proceedures are created with migrations.
            var idParam = new SqlParameter
            {
                ParameterName = "photoId",
                Value = PhotoId
            };
            var usernameParam = new SqlParameter
            {
                ParameterName = "userName",
                Value = UserName
            };
            var valueParam = new SqlParameter
            {
                ParameterName = "value",
                Value = Value
            };

            var results = _context.Database.SqlQuery<double>(
                "spAddOrUpdateRating @photoId, @userName, @value",
                idParam,
                usernameParam,
                valueParam);

            double newAverage = -1;
            bool saveFailed = false;

            try
            {
                newAverage = await results.SingleAsync();
            }
            catch (DbUpdateException ex)
            {
                // Save failed. This should only happen if another thread was called 
                // in between the check for null and theSaveChanges for this page.
                // This will cause a unique constraint exception.
                var sqlException = ex.GetBaseException() as SqlException;
                if (sqlException.Class == 14 && sqlException.Number == 2601) // Unique constraint exception
                {
                    // Set a flag so we know save failed. Can't call async function within a catch block.
                    saveFailed = true;
                }
            }

            if (saveFailed)
            {
                // Unique constraint exception, re-call the method to add/update the value.
                return await ExecuteAsync();
            }

            // add/update succeded. Return the new average.
            return newAverage;
        }
    }
}