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
using PaD.ViewModels;
using PaD.DataContexts;

namespace PaD.DAL.Projects
{
    public class GetCurrentStreakDays
    {
        private IPaDDb _context;

        public int ProjectId { get; set; }

        public GetCurrentStreakDays() { }
        public GetCurrentStreakDays(IDbContext context)
        {
            // allow it to be injected
            _context = (IPaDDb)context;
        }

        public async Task<int> ExecuteAsync()
        {
            // Call the spGetCurrentStreak stored proceedure to calculate this value.
            // Stored proceedures are created with migrations.
            var idParam = new SqlParameter {
                ParameterName = "projectId",
                Value = ProjectId
            };

            var results = _context.Database.SqlQuery<int>(
                "spGetCurrentStreak @projectId",
                idParam);
            var streakDays = await results.SingleOrDefaultAsync();

            return streakDays;
        }
    }
}