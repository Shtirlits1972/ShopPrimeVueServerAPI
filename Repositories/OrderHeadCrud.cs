using Dapper;
using ShopPrimeVueServerAPI.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ShopPrimeVueServerAPI.Crud
{
    public class OrderHeadCrud
    {
        private static readonly string strConn = Ut.GetConnetString();


        public static string GetNewOrderNumber()
        {
            string orderNumber = "######";
            using (IDbConnection db = new SqlConnection(strConn))
            {
                orderNumber = db.Query<string>("SELECT [dbo].[getNewOrderNumber] ()").FirstOrDefault() ?? "";
            }
            return orderNumber;
        }

        public static List<OrderHead> GetAll()
        {
            List<OrderHead> list = new List<OrderHead>();

            using (IDbConnection db = new SqlConnection(strConn))
            {
                list = db.Query<OrderHead>("SELECT Id, [orderNumber], [userId], [UsersName], [orderData], [TotalPrice] FROM OrderHeadView").ToList();
            }

            return list;
        }

        // Добавьте этот метод в ваш OrderHeadCrud класс
        public static List<OrderHead> GetAllByUserId(int userId)
        {
            List<OrderHead> list = new List<OrderHead>();

            using (IDbConnection db = new SqlConnection(strConn))
            {
                list = db.Query<OrderHead>(
                    "SELECT Id, [orderNumber], [userId], [UsersName], [orderData], [TotalPrice] FROM OrderHeadView WHERE userId = @UserId",
                    new { UserId = userId }
                ).ToList();
            }

            return list;
        }

        public static OrderHead GetOne(int Id)
        {
            OrderHead model = null;

            using (IDbConnection db = new SqlConnection(strConn))
            {
                model = db.Query<OrderHead>("SELECT Id, [orderNumber], [userId], [UsersName], [orderData], [TotalPrice] FROM OrderHeadView WHERE Id = @Id;", new { Id }).FirstOrDefault();
            }

            return model;
        }

        public static void Del(int Id)
        {
            OrderDetailCrud.DelByOrderHeadId(Id);

            using (IDbConnection db = new SqlConnection(strConn))
            {
                db.Execute("DELETE FROM OrderHead WHERE Id = @Id;", new { Id });
            }
        }

        public static void Update(OrderHead model)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                var Query = "UPDATE OrderHead SET orderNumber = @orderNumber, userId = @userId, orderData = @orderData WHERE Id = @Id;";
                db.Execute(Query, model);
            }
        }

        public static OrderHead Insert(OrderHead model)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                var Query = "INSERT INTO OrderHead ([orderNumber], [userId], [orderData]) VALUES(@orderNumber, @userId, @orderData); SELECT CAST(SCOPE_IDENTITY() as int)";
                int Id = db.Query<int>(Query, model).FirstOrDefault();
                model.id = Id;
            }

            return model;
        }
    }
}
