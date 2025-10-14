using Dapper;
using ShopPrimeVueServerAPI.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ShopPrimeVueServerAPI.Crud
{
    public class UsersCrud
    {
        private static readonly string strConn = Ut.GetConnetString();
        public static List<Users> GetAll()
        {
            List<Users> list = new List<Users>();

            using (IDbConnection db = new SqlConnection(strConn))
            {
                list = db.Query<Users>("SELECT Id, [Email], [Password], [Role], [UsersName], [isAppruved] FROM Users").ToList();
            }

            return list;
        }

        public static Users Autorize(String Email, String Password)
        {
            Users model = null;

            using (IDbConnection db = new SqlConnection(strConn))
            {
                model = db.Query<Users>("SELECT  TOP 1 Id, [Email], [Password], [Role], [UsersName], [isAppruved] FROM Users WHERE [Email] = @Email AND [Password] = @Password AND [isAppruved] = 1;", new { Email,  Password }).FirstOrDefault();
            }

            return model;
        }

        public static Users GetOne(int Id)
        {
            Users model = null;

            using (IDbConnection db = new SqlConnection(strConn))
            {
                model = db.Query<Users>("SELECT Id, [Email], [Password], [Role], [UsersName], [isAppruved] FROM Users WHERE Id = @Id;", new { Id }).FirstOrDefault();
            }

            return model;
        }

        public static void Del(int Id)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                db.Execute("DELETE FROM Users WHERE Id = @Id;", new { Id });
            }
        }

        public static void Update(Users model)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                var Query = "UPDATE Users SET Email = @Email , Password = @Password, Role = @Role, UsersName = @UsersName, isAppruved = @isAppruved WHERE Id = @Id;";
                db.Execute(Query, model);
            }
        }

        public static Users Insert(Users model)
        {
            using (IDbConnection db = new SqlConnection(strConn))
            {
                var Query = "INSERT INTO Users ([Email], [Password], [Role], [UsersName], [isAppruved]) VALUES(@Email, @Password, @Role, @UsersName, @isAppruved); SELECT CAST(SCOPE_IDENTITY() as int)";
                int Id = db.Query<int>(Query, model).FirstOrDefault();
                model.id = Id;
            }

            return model;
        }
    }
}
