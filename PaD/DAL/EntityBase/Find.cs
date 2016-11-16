using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

using PaD.DataContexts;

namespace PaD.DAL.EntityBase
{
    public class Find<T> where T: class
    {
        private IDbContext _context;

        public Expression<Func<T, bool>> Match;

        public Find(IDbContext context)
        {
            // allow it to be injected
            _context = context;
        }

        public T Execute()
        {
            return _context.Set<T>().SingleOrDefault(Match);
        }

        public async Task<T> ExecuteAsync()
        {
            return await _context.Set<T>().SingleOrDefaultAsync(Match);
        }

    }
}