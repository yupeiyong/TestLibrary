using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;


namespace autofac
{

    internal class Program
    {

        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ListMovieFinder>().As<IMovieFinder>();
            builder.RegisterType<MpgMovieLister>();
            var container = builder.Build();
            var lister = container.Resolve<MpgMovieLister>();
            var mpgs = lister.GetMpg();
            foreach (var mpg in mpgs)
            {
                Console.WriteLine(mpg.Name);
            }

            //三种注入方式
            //默认的构造方法注入
            builder = new ContainerBuilder();
            builder.RegisterType<A>();
            builder.RegisterType<B>();
            container = builder.Build();
            var a = container.Resolve<A>();
            Console.WriteLine(a.GetContent());
            //属性注入
            builder = new ContainerBuilder();
            builder.RegisterType<C>().PropertiesAutowired();
            builder.RegisterType<B>();
            container = builder.Build();
            var c = container.Resolve<C>();
            Console.WriteLine(c.GetContent());

            //方法注入（不推荐）

            //类型关联
            builder = new ContainerBuilder();
            builder.RegisterType<Class1>().As<IInterface>();
            container = builder.Build();
            var c1 = container.Resolve<IInterface>();
            Console.WriteLine(c1.Id);

            /*下面这段代码会报错，因为 Class1没有注册类型
            c1 = container.Resolve<Class1>();
            Console.WriteLine(c1.Id);
            */
            builder = new ContainerBuilder();
            //添加AsSelf()后，可以生成自身类型了
            builder.RegisterType<Class1>().As<IInterface>().AsSelf();
            container = builder.Build();
            c1 = container.Resolve<Class1>();
            Console.WriteLine(c1.Id);

            Console.ReadKey();
        }

    }
    #region 注入
    class C
    {

        public B B { get; set; }


        public C()
        {

        }
        public string GetContent()
        {
            return B.Content;
        }

    }
    class A
    {
        private B _b;
        public A(B b)
        {
            this._b = b;
        }


        public string GetContent()
        {
            return _b.Content;
        }
    }

    class B
    {
        public string Content { get; set; } = "This is class B!";
    }

    #endregion

    #region 注册
    public class Movie
    {

        public string Name { get; set; }

    }

    public class MpgMovieLister
    {

        private readonly IMovieFinder _movieFinder;

        //增加了构造函数，参数是IMovieFinder对象
        public MpgMovieLister(IMovieFinder movieFinder)
        {
            _movieFinder = movieFinder;
        }


        public Movie[] GetMpg()
        {
            var allMovies = _movieFinder.FindAll();
            return allMovies.Where(m => m.Name.EndsWith(".MPG")).ToArray();
        }

    }

    public class ListMovieFinder : IMovieFinder
    {

        public List<Movie> FindAll()
        {
            return new List<Movie>
            {
                new Movie {Name = "Die Hard.wmv"},
                new Movie {Name = "My Name is John.MPG"}
            };
        }

    }

    public interface IMovieFinder
    {

        List<Movie> FindAll();

    }
    #endregion

    #region 类型关联
    interface IInterface
    {
        Guid Id { get; }
    }

    class Class1 : IInterface
    {
        private Guid _id;

        public Guid Id
        {
            get { return _id; }
        }
    }
    #endregion
}