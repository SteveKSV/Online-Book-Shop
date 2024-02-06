using Discount.Managers.Interfaces;
using Dapper;
using Discount.Entities;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Discount.Managers
{
    public class DiscountManager : IDiscountManager
    {
        protected SqlConnection _sqlConnection;
        protected IDbTransaction _dbTransaction;
        public DiscountManager(SqlConnection sqlConnection, IDbTransaction dbTransaction)
        {
            _sqlConnection = sqlConnection;
            _dbTransaction = dbTransaction;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            try
            {
                var affected = await _sqlConnection.ExecuteAsync(
                    "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                    new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount },
                    _dbTransaction);

                // Commit the transaction
                _dbTransaction.Commit();

                if (affected == 0)
                    return false;

                return true;
            }
            catch
            {
                // Roll back the transaction on exception
                _dbTransaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            try
            {
                var affected = await _sqlConnection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
                new { ProductName = productName }, _dbTransaction);

                // Commit the transaction
                _dbTransaction.Commit();

                if (affected == 0)
                    return false;

                return true;
            }
            catch
            {
                // Roll back the transaction on exception
                _dbTransaction.Rollback();
                throw;
            }
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            var coupon = await _sqlConnection.QueryFirstOrDefaultAsync<Coupon>
                ("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName }, _dbTransaction);

            if (coupon == null)
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };
            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            try
            {
                var affected = await _sqlConnection.ExecuteAsync
                    ("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id }, _dbTransaction);

                // Commit the transaction
                _dbTransaction.Commit();

                if (affected == 0)
                    return false;

                return true;
            }
            catch
            {
                // Roll back the transaction on exception
                _dbTransaction.Rollback();
                throw;
            }
        }
    }
}

