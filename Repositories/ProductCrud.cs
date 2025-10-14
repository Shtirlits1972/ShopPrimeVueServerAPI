using Dapper;
using ShopPrimeVueServerAPI.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ShopPrimeVueServerAPI.Crud
{
    public class ProductCrud
    {
        private static readonly string strConn = Ut.GetConnetString();
        public static List<Product> GetAll()
        {
            List<Product> list = new List<Product>();

            using (IDbConnection db = new SqlConnection(strConn))
            {
                list = db.Query<Product>("SELECT Id, [ProductName], [Price], [CategoryId], [CategoryName], [Foto] FROM ProductView").ToList();
            }

            return list;
        }

        public static Product GetOne(int Id)
        {
            Product model = null;

            using (IDbConnection db = new SqlConnection(strConn))
            {
                model = db.Query<Product>("SELECT Id, [ProductName], [Price], [CategoryId], [CategoryName], [Foto] FROM ProductView WHERE Id = @Id;", new { Id }).FirstOrDefault();
            }

            return model;
        }

        public static void Del(int Id)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                db.Execute("DELETE FROM Product WHERE Id = @Id;", new { Id });
            }
        }

        public static void Update(Product model)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                var Query = "UPDATE Product SET ProductName = @ProductName, Price = @Price, CategoryId = @CategoryId,  Foto = @Foto WHERE Id = @Id;";
                db.Execute(Query, model);
            }
        }

        public static Product Insert(Product model)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                var Query = "INSERT INTO Product ([ProductName], [Price], [CategoryId], [Foto]) VALUES(@ProductName, @Price, @CategoryId, @Foto); SELECT CAST(SCOPE_IDENTITY() as int)";
                int Id = db.Query<int>(Query, model).FirstOrDefault();
                model.id = Id;
            }

            return model;
        }
    }
}
