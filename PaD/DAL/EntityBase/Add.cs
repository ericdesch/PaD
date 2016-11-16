using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using PaD.DataContexts;

namespace PaD.DAL.EntityBase
{
    public class Add<T> where T: class
    {
        private IDbContext _context;

        public T Entity { get; set; }

        public Add(IDbContext context)
        {
            // allow it to be injected
            _context = context;
        }

        public T Execute()
        {
            T entity = _context.Set<T>().Add(this.Entity);
            _context.SaveChanges();

            return entity;
        }

        public async Task<T> ExecuteAsync()
        {
            T entity = _context.Set<T>().Add(this.Entity);
            await _context.SaveChangesAsync();

            return entity;
        }

    }
}