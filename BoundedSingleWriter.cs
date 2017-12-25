using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BoundedSingleWriter
{
    class BoundedSingleWriter
    {
        static private readonly int RegisterCount = 2;

        static void Main(string[] args)
        {
            var bsw = new BSW(RegisterCount);
            var random = new Random();
            var task = new Task[3];
            for (var i = 0; i < 10; i++)
            {
                var id = i % 2;
                var value = random.Next(1000);
                task[id] = Task.Run(() =>
                {
                    Console.WriteLine("write value {0} in register №{1}", value, id);
                    bsw.Update(id, value);
                });

                if (i % 3 == 0)
                {
                    var count = i;
                    if (i > 0)
                    {
                        Task.WaitAll(task[2]);
                    }
                    task[2] = Task.Run(() =>
                    {
                        Console.WriteLine("read from registers on interation {1} by register №{0}: ({2})",
                                id, count, string.Join(", ", bsw.Scan(id)));
                    });
                }

                if (i % 2 == 1)
                {
                    Task.WaitAll(task[0], task[1]);
                }
            }
            Task.WaitAll(task[2]);
            var log = bsw.GetLog();
            Console.WriteLine("\n{0}\n{1}", log.Item1, log.Item2);
        }
    }

    class Register
    {
        public int Value {get; set;}
        public bool[] P {get; set;}
        public bool Toggle {get; set;}
        public int[] Snapshot {get; set;}
    }

    class BSW
    {
        private bool[,] q;
        private Register[] register;
        private int RegisterCount;

        private Stopwatch timer = new Stopwatch();
        private Dictionary<TimeSpan, int>[] logWrite;
        private Dictionary<TimeSpan, int[]> logRead = new Dictionary<TimeSpan, int[]>();

        public BSW(int RegCount)
        {
            RegisterCount = RegCount;
            register = new Register[RegisterCount];
            q = new bool[RegisterCount, RegisterCount];
            logWrite = new Dictionary<TimeSpan, int>[RegisterCount];
            for (int i = 0; i < RegisterCount; ++i)
            {
                register[i] = new Register();
                register[i].p = new bool[RegisterCount];
                register[i].snapshot = new int[RegisterCount];
                logWrite[i] = new Dictionary<TimeSpan, int>();
            }
            timer.Start();
        }

        public int[] Scan(int id, bool flag = true)
        {
            var moved = new int[RegisterCount];
            while (true)
            {
                for (var i = 0; i < RegisterCount; ++i)
                {
                    q[id, i] = register[i].p[id];
                }
                var a = new Register[RegisterCount];
                Array.Copy(register, a, RegisterCount);
                var b = new Register[RegisterCount];
                Array.Copy(register, b, RegisterCount);
                bool noChangeFlag = true;
                for (int i = 0; i < RegisterCount; ++i)
                {
                    if (a[i].p[id] == b[i].p[id] &&
                        b[i].p[id] == q[id, i] &&
                        a[i].toggle == b[i].toggle)
                    {
                        continue;
                    }
                    else
                    {
                        noChangeFlag = false;
                    }
                }
                if (noChangeFlag)
                {
                    int[] data = new int[RegisterCount];
                    for (int i = 0; i < RegisterCount; ++i)
                    {
                        data[i] = b[i].value;
                    }
                    if (flag)
                    {
                        logRead.Add(timer.Elapsed, data);
                    }
                    return data;
                }
                for (var i = 0; i < RegisterCount; ++i)
                {
                    if (a[i].p[id] != q[id, i] ||
                        b[i].p[id] != q[id, i] ||
                        a[i].toggle != b[i].toggle)
                    {
                        if (moved[i] == 1)
                        {
                            if (flag) 
                            {
                                logRead.Add(timer.Elapsed, b[i].snapshot);
                            }
                            return b[i].snapshot;
                        }
                        else
                        {
                            ++moved[i];
                        }
                    }
                }
            }
        }

        public void Update(int id, int value)
        {
            var f = new bool[RegisterCount];
            for (var i = 0; i < RegisterCount; ++i)
            {
                f[i] = !q[i, id];
            }
            register[id].value = value;
            Array.Copy(f, register[id].p, RegisterCount);
            register[id].toggle = !register[id].toggle;
            register[id].snapshot = Scan(id, false);
            logWrite[id].Add(timer.Elapsed, value);
        }

        public Tuple<string, string> GetLog()
        {
            string write = "", read = "";
            for (var i = 0; i < RegisterCount; ++i)
            {
                write += string.Format("register №{0} and his log:\n", i);
                write += string.Format("({0}, [{1}], {2}, [{3}])\n",
                        register[i].value, string.Join(",", register[i].p),
                        register[i].toggle, string.Join(",", register[i].snapshot));
                foreach (var change in logWrite[i])
                {
                    write += change + "\n";
                }
            }
            foreach (var scan in logRead)
            {
                read += string.Format("< values = (" + string.Join(", ", scan.Value));
                read += string.Format("), time = {0} >\n", scan.Key);
            }
            return new Tuple<string, string>(write, read);
        }
    }

}