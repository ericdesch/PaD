using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

using PaD.DataContexts;

namespace PaD.DAL.EntityBase
{
    public class AddRange<T> where T: class
    {
        private IDbContext _context;

        public ICollection<T> Entities { get; set; }

        public AddRange(IDbContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (T entity in Entities)
                {
                    _context.Set<T>().Add(entity);
                }
                _context.SaveChanges();

                scope.Complete();
            }
        }

        public async Task ExecuteAsync()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (T entity in Entities)
                {
                    _context.Set<T>().Add(entity);
                }
                await _context.SaveChangesAsync();

                scope.Complete();
            }
        }

    }
}