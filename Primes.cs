using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace Primes
{
    class Primes
    {
        static void Main(string[] args)
        {
            int l = 1, r = (int)1e7;
            Stopwatch timer = Stopwatch.StartNew();

            Console.WriteLine("Using threads on range from {0} to {1}", l, r);
            timer.Restart();
            List<int> result_thread = new ThreadSolver().get_primes(l, r);
            timer.Stop();
            Console.WriteLine("There are {0} primes in this range", result_thread.Count);
            Console.WriteLine("Time: {0}", timer.Elapsed);
            Console.WriteLine("===========================================");
            Console.WriteLine("Using tasks on range from {0} to {1}", l, r);
            timer.Restart();
            List<int> result_task = new TaskSolver().get_primes(l, r);
            timer.Stop();
            Console.WriteLine("There are {0} primes in this range", result_task.Count);
            Console.WriteLine("Time: {0}", timer.Elapsed);
            Console.WriteLine("===========================================");
            Console.WriteLine("Using thread pool on range from {0} to {1}", l, r);
            timer.Restart();
            List<int> result_threadpool = new ThreadPoolSolver().get_primes(l, r);
            timer.Stop();
            Console.WriteLine("There are {0} primes in this range", result_threadpool.Count);
            Console.WriteLine("Time: {0}", timer.Elapsed);
        }
    }

    class Solver
    {
        public List<int> get_primes(int l, int r)
        {
            List<int> result = new List<int>();
            find_primes(l, r, ref result);
            return result;
        }

        protected void find_primes(int l, int r, ref List<int> primes)
        {
            if (r < l)
            {
                return;
            }
            for (int i = l; i <= r; ++i)
            {
                if (is_prime(i) == true)
                {
                    lock ("Thread lock")
                    {
                        primes.Add(i);
                    }
                }
            }
        }        

        protected bool is_prime(int n)
        {
            if (n == 1)
            {
                return false;
            }
            for (int i = 2; 1LL * i * i <= n; ++i)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    class ThreadSolver : Solver
    {
        private const int COUNT_THREADS = 8;

        new public List<int> get_primes(int l, int r)
        {
            List<int> result = new List<int>();
            int range_per_thread = (r - l + 1) / COUNT_THREADS;
            Thread[] threads = new Thread[COUNT_THREADS];
            for (int i = 0; i < COUNT_THREADS - 1; ++i)
            {
                int cur_l = l + i * range_per_thread,
                    cur_r = l + (i + 1) * range_per_thread - 1;
                threads[i] = new Thread(() => 
                {
                    find_primes(cur_l, cur_r, ref result);
                });
                threads[i].Start();
            }
            threads[COUNT_THREADS - 1] = new Thread(() => 
            {
                find_primes(l + (COUNT_THREADS - 1) * range_per_thread, r, ref result);
            });
            threads[COUNT_THREADS - 1].Start();
            for (int i = 0; i < COUNT_THREADS; ++i)
            {
                threads[i].Join();
            }
            return result;
        }
    }

    class TaskSolver : Solver
    {
        private const int LOWER_BOUND = 500;

        new public List<int> get_primes(int l, int r)
        {
            if (r - l + 1 < LOWER_BOUND)
            {
                List<int> result = new List<int>();
                find_primes(l, r, ref result);
                return result;
            }
            else
            {
                int mid = l + ((r - l) >> 1);
                Task<List<int>> left = Task.Run(() => get_primes(l, mid));
                Task<List<int>> right = Task.Run(() => get_primes(mid + 1, r));
                Task.WaitAll(left, right);
                List<int> result = left.Result;
                result.AddRange(right.Result);
                return result;
            }
        }
    }

    class ThreadPoolSolver : Solver
    {
        new public List<int> get_primes(int l, int r)
        {
            List<int> result = new List<int>();
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(_ => {
                find_primes(l, r, ref result);
                manualResetEvent.Set();
            });
            manualResetEvent.WaitOne();
            return result;
        }
    }

}