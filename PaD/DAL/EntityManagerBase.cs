using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq.Expressions;
using System.Web.Mvc;

using Fooz.Logging;
using Fooz.Caching;

using PaD.DAL.EntityBase;
using PaD.DataContexts;
using PaD.Infrastructure;

namespace PaD.DAL
{
    public abstract class EntityManagerBase<T> where T : class
    {
        protected readonly IDbContext DatabaseContext;
        protected readonly ILoggerProvider Logger;
        protected readonly ICacheProvider Cache;

        public EntityManagerBase()
        {
            // Get the class that IDbContext, ILogger resolves to as bound in NinjectDependencyResolver.
            // Allow this way instead of using constructor injection.
            DatabaseContext = (IDbContext)DependencyResolver.Current.GetService(typeof(IDbContext));
            Logger = (ILoggerProvider)DependencyResolver.Current.GetService(typeof(ILoggerProvider));
            Cache = (ICacheProvider)DependencyResolver.Current.GetService(typeof(ICacheProvider));
        }

        public EntityManagerBase(IDbContext databaseContext, ILoggerProvider logger, ICacheProvider cache)
        {
            // allow it to be injected
            DatabaseContext = databaseContext;
            Logger = logger;
            Cache = cache;
        }

        public ICollection<T> GetAll()
        {
            // This base class is generic, so can't instatiate a specific context
            // if it was not instantiated by the calling code.
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            GetAll<T> getAll = new GetAll<T>(DatabaseContext);

            return getAll.Execute();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            GetAll<T> getAll = new GetAll<T>(DatabaseContext);

            return await getAll.ExecuteAsync();
        }

        public T Get(int id)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Get<T> get = new Get<T>(DatabaseContext)
                {
                    Id = id
                };

            return get.Execute();
        }

        public async Task<T> GetAsync(int id)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Get<T> get = new Get<T>(DatabaseContext)
                {
                    Id = id
                };

            return await get.ExecuteAsync();
        }

        public T Find(Expression<Func<T, bool>> match)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Find<T> find = new Find<T>(DatabaseContext)
                {
                    Match = match
                };

            return find.Execute();
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Find<T> find = new Find<T>(DatabaseContext)
                {
                    Match = match
                };

            return await find.ExecuteAsync();
        }

        public ICollection<T> FindAll(Expression<Func<T, bool>> match)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            FindAll<T> findAll = new FindAll<T>(DatabaseContext)
                { 
                    Match = match
                };

            return findAll.Execute();
        }

        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            FindAll<T> findAll = new FindAll<T>(DatabaseContext)
                {
                    Match = match
                };

            return await findAll.ExecuteAsync();
        }

        public T Add(T t)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Add<T> add = new Add<T>(DatabaseContext)
                {
                    Entity = t
                };

            T entity = add.Execute();

            return entity;
        }

        public async Task<T> AddAsync(T t)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Add<T> add = new Add<T>(DatabaseContext)
                {
                    Entity = t
                };

            T entity = await add.ExecuteAsync();

            return entity;
        }

        public T Update(T t, int key)
        {
            if (t == null)
            {
                return null;
            }

            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Update<T> update = new Update<T>(DatabaseContext)
            {
                Entity = t,
                Key = key
            };

            T entity = update.Execute();

            return entity;
        }

        public async Task<T> UpdateAsync(T t, int key)
        {
            if (t == null)
            {
                return null;
            }

            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Update<T> update = new Update<T>(DatabaseContext)
            {
                Entity = t,
                Key = key
            };

            return await update.ExecuteAsync();
        }

        public void Delete(T t)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Delete<T> delete = new Delete<T>(DatabaseContext)
                {
                    Entity = t
                };

            delete.Execute();
        }

        public async Task DeleteAsync(T t)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Delete<T> delete = new Delete<T>(DatabaseContext)
                {
                    Entity = t
                };

            await delete.ExecuteAsync();
        }

        public int Count()
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Count<T> count = new Count<T>(DatabaseContext);
            
            return count.Execute();
        }

        public async Task<int> CountAsync()
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            Count<T> count = new Count<T>(DatabaseContext);

            return await count.ExecuteAsync();
        }

        public void AddRange(ICollection<T> entities)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            AddRange<T> addRange = new AddRange<T>(DatabaseContext)
            {
                Entities = entities
            };

            addRange.Execute();
        }

        public async Task AddRangeAsync(ICollection<T> entities)
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            AddRange<T> addRange = new AddRange<T>(DatabaseContext)
            {
                Entities = entities
            };

            await addRange.ExecuteAsync();
        }

        public int SaveChanges()
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            return DatabaseContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            if (DatabaseContext == null)
                throw new NullReferenceException("DatabaseContext cannot be null");

            return await DatabaseContext.SaveChangesAsync();
        }

    }
}