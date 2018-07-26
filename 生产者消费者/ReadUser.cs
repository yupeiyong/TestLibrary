using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
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
                var users = GetUsers(100);
                var count = users.Count;
                _factory.Push(users);
                if (count > 0)
                {
                    Console.WriteLine($"读入{count}条数据到内存");
                }
                else
                {
                    break;
                }
            }
            //Console.WriteLine($"读取线程：{Thread.CurrentThread.ManagedThreadId},结束。");
        }


        public List<Users> GetUsers(int count)
        {
            var sql = @"UPDATE [dbo].[Users]
                   SET [Flag] = 1
                 WHERE UserID In(Select Top " + count + " [UserID] from [Users] where Flag is null or Flag=0 order by [UserID])";

            var users = new List<Users>();
            var changeRows=DbHelperSQL.ExecuteSql(sql);

            //如果Flag＝0的行数为0，直接返回
            if (changeRows == 0)
                return users;

            //再查询出来
            sql = @"SELECT [UserID]
                  ,[UserName]
                  ,[Flag]
                  ,[Amount]
              FROM [dbo].[Users]
              Where [Flag] = 1";
            
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
            Users user=null;
            do
            {
                user = _factory.Pop();
                if (user == null) continue;
                Update(user);
                Console.WriteLine($"更新了Id={user.UserID}的用户数据");
            } while (user != null);
            //Console.WriteLine($"更新线程：{Thread.CurrentThread.ManagedThreadId},结束。");
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