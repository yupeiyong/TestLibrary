using System;


namespace 回溯法
{

    public class NQueens
    {

        /// <summary>
        ///     是否可以放皇后
        /// </summary>
        /// <returns></returns>
        private bool CanPlace(int[] location, int n)
        {
            for (var i = 0; i < n; i++)
            {
                //假设两个皇后被放置在(i,j)和(k,l)位置上，则仅当：
                //i-j=k-l或i+j=k+l是，它们在同一条斜角线上
                //即：j-l=i-k或j-l=k-l
                //亦即：当且仅当|j-l|=|i-k|时，两个皇后在同一斜角线上
                if (location[i] == location[n] || Math.Abs(i - n) == Math.Abs(location[i] - location[n]))
                    return false;
            }
            return true;
        }


        public int[] NQueen(int n)
        {
            //皇后的座标位置
            var locations = new int[n];

            //设置初始位置
            for (var m = 0; m < n; m++)
            {
                locations[m] = 1;
            }

            var i = 0;
            var flag = false;
            while (i >= 0)
            {
                //检查并放置当前行皇后
                while (locations[i] <= n)
                {
                    //放置成功，1）不是最后一行，跳到下一行。2）是最后一行，标志成功
                    if (CanPlace(locations, i))
                    {
                        if (i == n - 1)
                        {
                            flag = true;
                            break;
                        }
                        else
                        {
                            i++;
                            locations[i] = 1;
                        }
                    }
                    else
                    {
                        locations[i]++;
                    }
                }
                if (flag) break;
                locations[i] = 1;
                //回溯上一行，重新放置
                i--;
                locations[i]++;
            }
            if (!flag) return null;
            return locations;
        }

    }

}