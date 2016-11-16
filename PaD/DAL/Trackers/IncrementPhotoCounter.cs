using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Entity.Core;
using System.Text.RegularExpressions;
using System.Data.Entity.Core.Objects;

using PaD.Models;
using PaD.DataContexts;

namespace PaD.DAL.Trackers
{
    public class IncrementPhotoCounter
    {
        private IPaDDb _context;

        public int PhotoId { get; set; }

        public IncrementPhotoCounter() { }
        public IncrementPhotoCounter(IDbContext context)
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
            // Call the spIncrementPhotoCounter stored proceedure to increment this value.
            // Stored proceedures are created with migrations.
            var idParam = new SqlParameter
            {
                ParameterName = "photoId",
                Value = PhotoId
            };

            var results = _context.Database.SqlQuery<long>(
                "spIncrementPhotoCounter @photoId",
                idParam);

            return results;
        }

        //****************************************************************
        // Saved for posterity. Pattern to use to avoid concurrency issues
        //****************************************************************

        ///// <summary>
        ///// Creates a new PhotoTracker for these properties with Counter = 1
        ///// </summary>
        ///// <returns>Task</returns>
        //private async Task<long> CreatePhotoTrackerAsync()
        //{
        //    PhotoTracker photoTracker = new PhotoTracker()
        //    {
        //        PhotoId = PhotoId,
        //        Counter = 1
        //    };

        //    _context.PhotoTracker.Add(photoTracker);

        //    bool saveFailed = false;
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        // Save failed. This should only happen if another thread was called 
        //        // in between the check for null and theSaveChanges for this page.
        //        // This will cause a unique constraint exception.
        //        var sqlException = ex.GetBaseException() as SqlException;
        //        if (sqlException.Class == 14 && sqlException.Number == 2601) // Unique constraint exception
        //        {
        //            // Set a flag so we know save failed. Can't call async function within a catch block.
        //            saveFailed = true;
        //        }
        //    }

        //    if (saveFailed)
        //    {
        //        // Unique constraint exception, re-call the method to increment the count.
        //        // Need to clear the context's local collection. It is tracking it from the first find
        //        // in the Excute() method.
        //        _context.PhotoTracker.Local.Clear();
        //        return await Execute();
        //    }

        //    // Save succeded. Return the count which will always be 1 for the first tracking entry.
        //    return 1;
        //}

        ///// <summary>
        ///// Increments the counter for the passed PhotoTracker
        ///// </summary>
        ///// <param name="projectTracker">The ProjectTracker whose Count you wish to increment</param>
        ///// <returns>Task</returns>
        //private async Task<long> IncrementPhotoTrackerAsync(PhotoTracker photoTracker)
        //{
        //    long newCount;

        //    // Use optomistic concurrency where the database wins. If SaveChanges fails due to concurrency exception, 
        //    // reload the tracker, increment the new Counter and try to save again. Repeat until it succeeds.
        //    bool saveFailed;
        //    do
        //    {
        //        saveFailed = false;

        //        newCount = ++photoTracker.Counter;

        //        // Check for concurrancy issue.
        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException e)
        //        {
        //            // Save failed due to concurrency issue. Reload with new value and try again.
        //            saveFailed = true;
        //            e.Entries.Single().Reload();
        //        }

        //    } while (saveFailed);

        //    return newCount;
        //}
    }
}