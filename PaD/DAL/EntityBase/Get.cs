using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using PaD.DataContexts;

namespace PaD.DAL.EntityBase
{
    public class Get<T> where T : class
    {
        private IDbContext _context;

        public int Id { get; set; }

        public Get(IDbContext context)
        {
            // allow it to be injected
            _context = context;
        }

        public T Execute()
        {
            return _context.Set<T>().Find(Id);
        }

        public async Task<T> ExecuteAsync()
        {
            return await _context.Set<T>().FindAsync(Id);
        }

    }
}