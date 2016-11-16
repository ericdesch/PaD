using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

using PaD.Models;
using PaD.DataContexts;
using System.Data.SqlClient;

namespace PaD.DAL.Trackers
{
    public class IncrementProjectCounter
    {
        private IPaDDb _context;

        public int ProjectId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        public IncrementProjectCounter() { }
        public IncrementProjectCounter(IDbContext context)
        {
            // allow it to be injected
            _context = (IPaDDb)context;
        }

        public async Task<long> ExecuteAsync()
        {
            var results = GetDbRawSqlQuery();

            long counter = -1;
            bool saveFailed = false;

            try
            {
                counter = await results.SingleAsync();
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
                // Unique constraint exception, re-call the method to increment the count.
                return await ExecuteAsync();
            }

            // Increment succeded. Return the counter.
            return counter;
        }

        public long Execute()
        {
            var results = GetDbRawSqlQuery();

            long counter = -1;

            try
            {
                counter = results.Single();
            }
            catch (DbUpdateException ex)
            {
                // Save failed. This should only happen if another thread was called 
                // in between the check for null and theSaveChanges for this page.
                // This will cause a unique constraint exception.
                var sqlException = ex.GetBaseException() as SqlException;
                if (sqlException.Class == 14 && sqlException.Number == 2601) // Unique constraint exception
                {
                    // Re-call the method to increment the count.
                    return Execute();
                }
            }

            // Increment succeded. Return the counter.
            return counter;
        }

        private DbRawSqlQuery<long> GetDbRawSqlQuery()
        {
            // Call the spIncrementProjectCounter stored proceedure to increment this value.
            // Stored proceedures are created with migrations.
            var idParam = new SqlParameter
            {
                ParameterName = "projectId",
                Value = ProjectId
            };
            var yearParam = new SqlParameter
            {
                ParameterName = "year",
                Value = Year
            };
            var monthParam = new SqlParameter
            {
                ParameterName = "month",
                Value = Month
            };

            var results = _context.Database.SqlQuery<long>(
                "spIncrementProjectCounter @projectId, @year, @month",
                idParam, yearParam, monthParam);

            return results;
        }

    }
}