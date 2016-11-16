using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using PaD.DataContexts;

namespace PaD.DAL.EntityBase
{
    public class Delete<T> where T: class
    {
        private IDbContext _context;

        public T Entity { get; set; }

        public Delete(IDbContext context)
        {
            // allow it to be injected
            _context = context;
        }

        public void Execute()
        {
            _context.Set<T>().Remove(this.Entity);
            _context.SaveChanges();
        }

        public async Task ExecuteAsync()
        {
            _context.Set<T>().Remove(this.Entity);
            await _context.SaveChangesAsync();
        }

    }
}