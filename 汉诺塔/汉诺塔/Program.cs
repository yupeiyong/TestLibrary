using System;


namespace 汉诺塔
{

    internal class Program
    {

        private static void Main(string[] args)
        {
            int n;
            Console.WriteLine("请输入数字n以解决n阶汉诺塔问题：\n");
            var s = Console.ReadLine();
            n = int.Parse(s);
            Hanoi(n,'A', 'B', 'C');
            Console.ReadKey();
        }


        /*
         程序总结
            1、思路第一步：将n-1个盘从源柱移动到中间柱
            2、将第n个盘从源柱直接移动到目的柱
            3、将n-1个盘从中间柱移动到目的柱（分治）
        
        算法：
            1、当只有一个盘子的时候，只需要从将A塔上的一个盘子移到C塔上。
            2、当A塔上有两个盘子是，先将A塔上的1号盘子（编号从上到下）移动到B塔上，再将A塔上的2号盘子移动的C塔上，最后将B塔上的小盘子移动到C塔上。
            3、当A塔上有3个盘子时，先将A塔上编号1至2的盘子（共2个）移动到B塔上（需借助C塔），然后将A塔上的3号最大的盘子移动到C塔，最后将B塔上的两个盘子借助A塔移动到C塔上。
            4、当A塔上有n个盘子是，先将A塔上编号1至n-1的盘子（共n-1个）移动到B塔上（借助C塔），然后将A塔上最大的n号盘子移动到C塔上，最后将B塔上的n-1个盘子借助A塔移动到C塔上。
              综上所述，除了只有一个盘子时不需要借助其他塔外，其余情况均一样（只是事件的复杂程度不一样）。
        分治法
	        所谓分治法，就是分而治之。将一个问题分解为多个规模较小的子问题，这些子问题互相独立并与原问题解决方法相同。递归解这些子问题，然后将这各子问题的解合并得到原问题的解。
 
	        适用问题的特征：
                该问题的规模缩小到一定的程度就可以容易地解决
                该问题可以分解为若干个规模较小的相同问题，即该问题具有最优子结构性质。所谓最优子结构是指：问题的最优解所包含的子问题的解也是最优的。
                该问题所分解出的各个子问题是相互独立的，即子问题之间不包含公共的子问题
	        一般实现步骤：分解--->递归求解--->合并
            汉诺盘问题正好适用于分治法，当将n-1个盘移动到中间柱时，这n-1个盘与原问题是相同的解决方法，也就是子问题
            只有2个盘时是最优子结构
        */


        public static void Hanoi(int n, char source, char middle, char destination)
        {
            if (n == 1)
            {
                Move(n, source, destination);
            }
            else
            {
                Hanoi(n-1,source,destination,middle);
                Move(n,source,destination);
                Hanoi(n - 1, middle, source, destination);
            }
        }


        public static void Move(int n, char source, char destination)
        {
            Console.WriteLine("第{0}盘，从：{1}柱移动到：{2}",n,source,destination);
        }

    }

}