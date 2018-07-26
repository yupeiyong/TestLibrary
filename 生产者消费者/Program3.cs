using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using HD.DBHelper;


namespace 生产者消费者
{

    internal class Program3
    {

        public static void Main(string[] args)
        {
            Test_MultiThreadUpdateUsers();
            Console.ReadKey();
        }


        private static void Test_MultiThreadUpdateUsers()
        {
            //先插入指定条数数据
            //AddUsers(50000);
            var watch = new Stopwatch();
            watch.Start();

            var factory = new DbFactory(50);

            //读取线程
            var producer = new ReadUser(factory);
            var producerThread = new Thread(producer.Start);
            producerThread.Start();

            //更新线程
            var consumerThreads = new List<Thread>();
            for (var i = 0; i < 100; i++)
            {
                var consumer = new UpdateUser(factory);
                consumerThreads.Add(new Thread(consumer.Start));
            }
            foreach (var t in consumerThreads)
            {
                t.Start();
            }

            producerThread.Join();

            foreach (var t in consumerThreads)
            {
                t.Join();
            }

            watch.Stop();
            var userSeconds = (double) (watch.ElapsedMilliseconds/1000);
            Console.WriteLine($"共用时：{userSeconds} 秒");
        }


        private static void AddUsers(int count)
        {
            var sqlStr = @"INSERT INTO [dbo].[Users]([UserName])VALUES(@userName)";
            for (var i = 0; i < count; i++)
            {
                var userName = $"user{i}";
                DbHelperSQL.ExecuteSql(sqlStr, new SqlParameter("@userName", userName));
            }
        }

    }

}