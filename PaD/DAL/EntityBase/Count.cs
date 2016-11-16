using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using PaD.DataContexts;

namespace PaD.DAL.EntityBase
{
    public class Count<T> where T: class
    {
        private IDbContext _context;

        //public Count()
        //{
        //}
        public Count(IDbContext context)
        {
            // allow it to be injected
            _context = context;
        }

        public int Execute()
        {
            int count = _context.Set<T>().Count();

            return count;
        }

        public async Task<int> ExecuteAsync()
        {
            int count = await _context.Set<T>().CountAsync();

            return count;
        }

    }
}