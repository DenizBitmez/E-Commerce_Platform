using ETicaretBusiness.Abstract;
using ETicaretData.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretBusiness.Concreate
{
    public class GenericRepository<T, TContext> : IGenericRepository<T> where T : class, new()
        where TContext : IdentityDbContext<AppUser, AppRole, int>, new()
    {
        public void Add(T entity)
        {
            using (var db = new TContext())
            {
                db.Set<T>().Add(entity);
                db.SaveChanges();
            }
        }

        public void Delete(T entity)
        {
            using (var db = new TContext())
            {
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
                //db.Set<T>().Remove(entity);
                //db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (TContext db = new TContext())
            {
                var nesne = db.Set<T>().Find(id);
                db.Set<T>().Remove(nesne);
                db.SaveChanges();
            }
        }

        public T Get(int id)
        {
            using (var db = new TContext())
            {
                var nesne = db.Set<T>().Find(id);
                return nesne;
            }
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            using (var db = new TContext())
            {
                var nesne = db.Set<T>().Find(filter);
                return nesne;
            }
        }

        public List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            using (var db = new TContext())
            {
                return filter == null ? db.Set<T>().ToList() : db.Set<T>().Where(filter).ToList();
            }
        }

        public void Update(T entity)
        {
            using (TContext db = new TContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }


    }
}
