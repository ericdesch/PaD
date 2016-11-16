using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using PaD.DataContexts;

namespace PaD.DAL.EntityBase
{
    public class GetAll<T> where T : class
    {
        private IDbContext _context;

        public GetAll(IDbContext context)
        {
            // allow it to be injected
            _context = context;
        }

        public ICollection<T> Execute()
        {
            return _context.Set<T>().ToList();
        }

        public async Task<ICollection<T>> ExecuteAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

    }
}