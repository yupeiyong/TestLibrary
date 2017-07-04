using System;
using System.Collections.Generic;


namespace 活动选择问题
{

    internal class Program
    {

        private static void Main(string[] args)
        {
            var gGreedyAct=new GreedyAct();
            var selectActs = GreedyAct.SelectAct();
            gGreedyAct.Print(selectActs);
            Console.ReadKey();
        }

    }

    /// <summary>
    ///     活动
    /// </summary>
    public class Act : IComparable<Act>
    {

        public DateTime Start { get; set; }

        public DateTime End { get; set; }


        public int CompareTo(Act other)
        {
            if (other == null) return 1;
            if (End > other.End) return 1;
            if (End == other.End) return 0;
            return -1;
        }

    }


    /// <summary>
    ///     贪心法选择活动
    /// </summary>
    public class GreedyAct
    {
        private static List<Act> acts;

        public GreedyAct()
        {
            acts = InitAct(15);
            acts.Sort();
        }


        private static List<Act> InitAct(int n)
        {
            var acts = new List<Act>();
            var rnd=new Random();
            for (var i = 0; i < n; i++)
            {
                var addStartHour = rnd.Next(0, 24);
                var addEndHour = addStartHour + rnd.Next(1, 3);
                acts.Add(new Act
                {
                    Start = DateTime.Now.AddHours(addStartHour),
                    End = DateTime.Now.AddHours(addEndHour)
                });
            }
            return acts;
        }


        public static List<Act> SelectAct()
        {
           
            var i = 0;
            var selecteds = new List<Act>() {acts[i]};
            for (var j = 1; j < acts.Count; j++)
            {
                if (acts[j].Start < acts[i].End) continue;
                selecteds.Add(acts[j]);
                i = j;
            }
            return selecteds;
        }


        public void Print(List<Act> actsParameter)
        {
            if (actsParameter == null || actsParameter.Count == 0) return;
            foreach (var act in actsParameter)
            {
                Console.WriteLine("活动开始时间:{0:yyyy-MM-dd HH:mm:ss}，结束时间:{1:yyyy-MM-dd HH:mm:ss}", act.Start,act.End);
            }
        }
    }

}