using Dapper;
using ShopPrimeVueServerAPI.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ShopPrimeVueServerAPI.Crud
{
    public class OrderDetailCrud
    {
        private static readonly string strConn = Ut.GetConnetString();
        public static List<OrderDetail> GetAll(int orderId)
        {
            List<OrderDetail> list = new List<OrderDetail>();

            using (IDbConnection db = new SqlConnection(strConn))
            {
                list = db.Query<OrderDetail>("SELECT Id, [orderId], [ProductId], [Qty], [ProductName], [Price], [RowSum] FROM OrderDetailView  WHERE [orderId] = @orderId;", new { orderId  }).ToList();
            }

            return list;
        }

        public static OrderDetail GetOne(int Id)
        {
            OrderDetail model = null;

            using (IDbConnection db = new SqlConnection(strConn))
            {
                model = db.Query<OrderDetail>("SELECT Id, [orderId], [ProductId], [Qty], [ProductName], [Price], [RowSum] FROM OrderDetailView WHERE Id = @Id;", new { Id }).FirstOrDefault();
            }

            return model;
        }

        public static void Del(int Id)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                db.Execute("DELETE FROM OrderDetail WHERE Id = @Id;", new { Id });
            }
        }

        public static void DelByOrderHeadId(int orderId)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                db.Execute("DELETE FROM OrderDetail WHERE orderId = @orderId;", new { orderId });
            }
        }

        public static void Update(OrderDetail model)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                var Query = "UPDATE OrderDetail SET orderId = @orderId, ProductId = @ProductId, Qty = @Qty WHERE Id = @Id;";
                db.Execute(Query, model);
            }
        }

        public static OrderDetail Insert(OrderDetail model)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                var Query = "INSERT INTO OrderDetail ([orderId], [ProductId], [Qty]) VALUES(@orderId, @ProductId, @Qty); SELECT CAST(SCOPE_IDENTITY() as int)";
                int Id = db.Query<int>(Query, model).FirstOrDefault();
                model.id = Id;
            }

            return model;
        }
    }
}
