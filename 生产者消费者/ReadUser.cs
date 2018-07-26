using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using HD.DBHelper;


namespace 生产者消费者
{

    public class ReadUser
    {

        private DbFactory _factory;


        public ReadUser(DbFactory factory)
        {
            _factory = factory;
        }


        public void Start()
        {
            if (_factory == null)
                return;

            while (!_factory.Completed)
            {
                _factory.Push(GetUsers(100));
                Console.WriteLine("读入100条数据到内存");
            }
        }


        public List<Users> GetUsers(int count)
        {
            var sql = @"UPDATE [dbo].[Users]
                   SET [Flag] = 1
                 WHERE UserID In(Select Top " + count + " [UserID] from [Users] where Flag is null or Flag=0 order by [UserID])";

            DbHelperSQL.ExecuteSql(sql);

            //再查询出来
            sql = @"SELECT [UserID]
                  ,[UserName]
                  ,[Flag]
                  ,[Amount]
              FROM [dbo].[Users]
              Where [Flag] = 1";
            var users = new List<Users>();
            using (var read = DbHelperSQL.ExecuteReader(sql))
            {
                while (read.Read())
                {
                    var user = new Users();
                    user.UserID = read.GetInt32(0);
                    user.UserName = read.GetString(1);
                    user.Flag = read.GetInt32(2);
                    user.Amount = (double) (read.IsDBNull(3) ? 0 : read.GetDecimal(3));
                    users.Add(user);
                }
                read.Close();
            }
            return users;
        }

    }

    public class UpdateUser
    {

        private DbFactory _factory;


        public UpdateUser(DbFactory factory)
        {
            _factory = factory;
        }


        public void Start()
        {
            while (!_factory.Completed)
            {
                var user = _factory.Pop();
                if (user != null)
                    Update(user);
                Console.WriteLine("更新一条数据");
            }
        }


        public void Update(Users user)
        {
            var sqlStr = @"
            UPDATE [dbo].[Users]
               SET [Amount] = @amount,Flag=2
             WHERE [UserID]=@userId";
            var parameters = new[] {new SqlParameter("@amount", 2000), new SqlParameter("@userId", user.UserID)};
            DbHelperSQL.ExecuteSql(sqlStr, parameters);
        }

    }

    public enum State
    {

        None = 0,
        Processing = 1,
        Completed = 2

    }

}