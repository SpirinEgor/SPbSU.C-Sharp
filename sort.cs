using System;
using System.IO;
using System.Threading;

class Qsort_object
{
    public int l;
    public int r;

    public Qsort_object(int c_l, int c_r)
    {
        l = c_l;
        r = c_r;
    }
}

class Program
{

    const int BORDER_QSORT = 10;
    const int COUNT_THREADS = 8;

    static string read() {
        try 
        {
            using (StreamReader sr = new StreamReader("sort.in")) 
            {
                String line = sr.ReadToEnd();
                return line;
            }
        } 
        catch (Exception e)
        {
            Console.WriteLine("The file could not be read: ");
            Console.WriteLine(e.Message);
        }
        return "";
    }

    public static void swap<T> (ref T lhs, ref T rhs) {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    static int[] arr;
    static int cnt_thread = 1;

    public static void qsort(object input)  // [l..r)
    {
        Console.WriteLine("Thread #: " + Thread.CurrentThread.Name);
        Qsort_object cur = (Qsort_object)input;
        int size = cur.r - cur.l;
        if (size < 10)
        {
            for (int i = cur.l; i < cur.r; ++i) 
            {
                for (int j = i + 1; j < cur.r; ++j)
                {
                    if (arr[i] > arr[j])
                    {
                        swap(ref arr[i], ref arr[j]);
                    }
                }
            }
            return;
        }
        int mid = size / 2, l_swap = cur.l, r_swap = cur.r - 1;
        while (l_swap <= r_swap) 
        {
            while (arr[l_swap] < arr[mid])
            {
                ++l_swap;
            }
            while (arr[r_swap] > arr[mid])
            {
                --r_swap;
            }
            if (l_swap <= r_swap)
            {
                swap(ref arr[l_swap], ref arr[r_swap]);
                ++l_swap;
                --r_swap;
            }
        }
        if (cnt_thread < COUNT_THREADS)
        {
            Thread left_thread = new Thread(new ParameterizedThreadStart(qsort));
            Thread right_thread = new Thread(new ParameterizedThreadStart(qsort));
            left_thread.Name = (cnt_thread++).ToString();
            right_thread.Name = (cnt_thread++).ToString();
            left_thread.Start(new Qsort_object(cur.l, mid));
            right_thread.Start(new Qsort_object(mid, cur.r));
            left_thread.Join();
            right_thread.Join();
        }
        else
        {
            qsort(new Qsort_object(cur.l, mid));
            qsort(new Qsort_object(mid, cur.r));
        }
    }

    static void Main(string[] args)
    {
        string s = read();
        using (StreamWriter sw = new StreamWriter("sort.out")) 
        {
            sw.Write("");
        }
        string[] s_split = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int n = s_split.Length;
        arr = new int[n];
        for (int i = 0 ; i < n; ++i)
        {
            arr[i] = Convert.ToInt32(s_split[i]);
        }
        qsort(new Qsort_object(0, n));
        for (int i = 0; i < n; ++i)
        {
            using (StreamWriter sw = new StreamWriter("sort.out", true)) 
            {
                sw.Write(arr[i]);
                sw.Write(' ');
            }       
        }
    }
}
