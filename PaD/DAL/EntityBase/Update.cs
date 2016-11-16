using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

using PaD.DataContexts;

namespace PaD.DAL.EntityBase
{
    public class Update<T> where T: class
    {
        private IDbContext _context;

        public T Entity { get; set; }
        public int Key { get; set; }

        public Update(IDbContext context)
        {
            // allow it to be injected
            _context = context;
        }

        public T Execute()
        {
            T existing = _context.Set<T>().Find(Key);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(Entity);
                _context.SaveChanges();
            }

            return existing;
        }

        public async Task<T> ExecuteAsync()
        {
            T existing = await _context.Set<T>().FindAsync(Key);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(Entity);
                await _context.SaveChangesAsync();
            }

            return existing;
        }

    }
}