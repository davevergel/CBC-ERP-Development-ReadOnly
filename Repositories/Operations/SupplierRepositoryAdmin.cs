using System.Data;
using CbcRoastersErp.Helpers;
using CbcRoastersErp.Models;
using Dapper;

namespace CbcRoastersErp.Repositories
{
    public class SupplierRepositoryAdmin
    {
        private readonly IDbConnection _db;

        public SupplierRepositoryAdmin()
        {
            _db = DatabaseHelper.GetConnection();
        }

        public IEnumerable<Suppliers> GetAll()
        {
            try
            {
                return _db.Query<Suppliers>("SELECT * FROM Suppliers").ToList();
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetAll), nameof(SupplierRepositoryAdmin), Environment.UserName);
                return new List<Suppliers>();
            }
        }
        public Suppliers GetById(int id)
        {
            try
            {
                return _db.QueryFirstOrDefault<Suppliers>("SELECT * FROM Suppliers WHERE Supplier_id = @id", new { id });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(GetById), nameof(SupplierRepositoryAdmin), Environment.UserName);
                return null;
            }
        }

        public void Add(Suppliers supplier)
        {
            try
            {
                const string sql = @"
                INSERT INTO Suppliers (Supplier_Name, Contact_email, Contact_phone, Address, Created_at)
                VALUES (@Supplier_Name, @Contact_email, @Contact_phone, @Address, NOW())";
                _db.Execute(sql, supplier);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(Add), nameof(SupplierRepositoryAdmin), Environment.UserName);
            }
        }

        public void Update(Suppliers supplier)
        {
            try
            {
                const string sql = @"
                UPDATE Suppliers SET
                    Supplier_Name = @Supplier_Name,
                    Contact_email = @Contact_email,
                    Contact_phone = @Contact_phone,
                    Address = @Address
                WHERE Supplier_id = @Supplier_id";
                _db.Execute(sql, supplier);
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(Update), nameof(SupplierRepositoryAdmin), Environment.UserName);
            }
        }

        public void Delete(int id)
        {
            try
            {
                _db.Execute("DELETE FROM Suppliers WHERE Supplier_id = @id", new { id });
            }
            catch (Exception ex)
            {
                ApplicationLogger.Log(ex, nameof(Delete), nameof(SupplierRepositoryAdmin), Environment.UserName);
            }
        }
    }
}

