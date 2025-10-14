using Dapper;
using ShopPrimeVueServerAPI.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Linq;

namespace ShopPrimeVueServerAPI.Crud
{
    public class CategoryCrud
    {
        private static readonly string strConn = Ut.GetConnetString();
        public static List<Category> GetAll()
        {
            List<Category> list = new List<Category>();

            using (IDbConnection db = new SqlConnection(strConn))
            {
                list = db.Query<Category>("SELECT Id, CategoryName FROM Category").ToList();
            }

            return list;
        }

        public static Category GetOne(int Id)
        {
            Category model = null;

            using (IDbConnection db = new SqlConnection(strConn))
            {
                model = db.Query<Category>("SELECT Id, CategoryName FROM Category WHERE Id = @Id;", new { Id }).FirstOrDefault();
            }

            return model;
        }

        public static void Del(int Id)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                db.Execute("DELETE FROM Category WHERE Id = @Id;", new { Id });
            }
        }

        public static void Update(Category model)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                var Query = "UPDATE Category SET CategoryName = @CategoryName WHERE Id = @Id;";
                db.Execute(Query, model);
            }
        }

        public static Category Insert(Category model)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                var Query = "INSERT INTO Category (CategoryName) VALUES(@CategoryName); SELECT CAST(SCOPE_IDENTITY() as int)";
                int Id = db.Query<int>(Query, model).FirstOrDefault();
                model.id = Id;
            }

            return model;
        }
    }
}
