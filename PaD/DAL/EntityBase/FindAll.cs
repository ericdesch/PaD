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
    public class FindAll<T> where T: class
    {
        private IDbContext _context;

        public Expression<Func<T, bool>> Match;

        public FindAll()
        {
        }
        public FindAll(IDbContext context)
        {
            // allow it to be injected
            _context = context;
        }

        public ICollection<T> Execute()
        {
            return _context.Set<T>().Where(Match).ToList();
        }

        public async Task<List<T>> ExecuteAsync()
        {
            return await _context.Set<T>().Where(Match).ToListAsync();
        }

    }
}