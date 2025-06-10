using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using CbcRoastersErp.Models;
using CbcRoastersErp.Helpers;

namespace CbcRoastersErp.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IDbConnection _db;

        public Repository()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public IEnumerable<T> GetAll()
        {
            return _db.Query<T>($"SELECT * FROM {typeof(T).Name}");
        }

        public T GetById(int id)
        {
            return _db.Query<T>($"SELECT * FROM {typeof(T).Name} WHERE Id = @Id", new { Id = id }).FirstOrDefault();
        }

        public void Add(T entity)
        {
            _db.Execute($"INSERT INTO {typeof(T).Name} VALUES (@entity)", new { entity });
        }

        public void Update(T entity)
        {
            _db.Execute($"UPDATE {typeof(T).Name} SET @entity WHERE Id = @Id", new { entity });
        }

        public void Delete(int id)
        {
            _db.Execute($"DELETE FROM {typeof(T).Name} WHERE Id = @Id", new { Id = id });
        }
    }
}
