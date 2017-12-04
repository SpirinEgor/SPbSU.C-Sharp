using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RedBlackTree
{
    
    public class Request
    {
        public int command {get; set;} // 0 - insert, 1 - remove, 2 - find, 3 - exit
        public int x {get; set;}
        public int y {get; set;}
        public int id {get; set;}

        public static readonly string[] Commands = {"insert", "remove", "find", "exit"};

        public Request(ref string commandValue, int xValue, int yValue = 0)
        {
            x = xValue;
            y = yValue;
            for (int i = 0; i < Commands.Length; ++i)
            {
                if (commandValue.Equals(Commands[i]))
                {
                    command = i;
                    break;
                }
            }
        }
    }
    
    class RedBlackTree
    {

        static Queue<Request> requestQueue = new Queue<Request>();

        static void Main(string[] args)
        {
            // Console.WriteLine("insert x y\nremove x\nfind x\nexit");
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            int processCount = 2;
            ThreadPool.QueueUserWorkItem(async _ =>
            {
                await ControlInput();
                // Console.WriteLine("Work with input has ended");
                lock ("Decrease procces counter")
                {
                    --processCount;
                    if (processCount == 0)
                    {
                        manualResetEvent.Set();
                    }
                }
            });
            ThreadPool.QueueUserWorkItem(async _ =>
            {
                await QueryProcessor();
                // Console.WriteLine("All requests processed");
                lock ("Decrease procces counter")
                {
                    --processCount;
                    if (processCount == 0)
                    {
                        manualResetEvent.Set();
                    }
                }
            });
            manualResetEvent.WaitOne();
        }

        static Task ControlInput()
        {
            return Task.Run(() =>
            {
                int idFind = 0;
                while(true)
                {
                    string[] input = Console.ReadLine().Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
                    if (input.Length == 0)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    bool checkInput = false;
                    for (int i = 1; i < input.Length; ++i)
                    {
                        try
                        {
                            Convert.ToInt32(input[i]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Can't parse args with exception " + ex.ToString());
                            checkInput = true;
                        }
                    }
                    if (checkInput)
                    {
                        continue;
                    }
                    if (input[0].Equals(Request.Commands[0]) && input.Length == 2)
                    {
                        Console.WriteLine("There aren't all arguments for insert");
                        continue;
                    }
                    if (input[0].Equals(Request.Commands[3]))
                    {
                        var request = new Request(ref input[0], 0, 0);
                        lock ("Enqueue lock")
                        {
                            requestQueue.Enqueue(request);
                        }
                        break;
                    }
                    else if (Array.Exists(Request.Commands, x => x.Equals(input[0])))
                    {
                        var request = new Request(ref input[0], Convert.ToInt32(input[1]),
                                            input.Length == 3 ? Convert.ToInt32(input[2]): 0);
                        if (input[0].Equals(Request.Commands[2]))
                        {
                            request.id = idFind;
                            ++idFind;
                        }
                        lock ("Enqueue lock")
                        {
                            requestQueue.Enqueue(request);
                            // System.Console.WriteLine(String.Join(" ", input) + " successfully enqueued");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unknown command");
                    }
                }
            });
        }

        static Task QueryProcessor()
        {
            return Task.Run(async () =>
            {
                int workingRequest = 0; // >= 0 - start find, == 0 - start updates, exit
                var tree = new Tree<int, int>();
                while (true)
                {
                    if (requestQueue.Count == 0)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    Request nextRequest;
                    lock ("Peek lock")
                    {
                        nextRequest = requestQueue.Peek();
                    }
                    if (nextRequest.command == 2)
                    {
                        bool flag = false;
                        lock ("Try to increase counter")
                        {
                            if (workingRequest >= 0)
                            {
                                ++workingRequest;
                                requestQueue.Dequeue();
                                flag = true;
                            }        
                        }
                        if (flag)
                        {
                            ThreadPool.QueueUserWorkItem(async _ =>
                            {
                                var requestResult = await tree.Find(nextRequest.x);
                                // Console.Write("result for request: find " + nextRequest.x);
                                // Console.WriteLine(" is " + (requestResult == null ?
                                                // "no such key" : requestResult.Value.ToString()));
                                Console.WriteLine("#" + nextRequest.id + ": " +
                                    (requestResult == null ? "null" : requestResult.Value.ToString()));
                                lock ("Decrease counter")
                                {
                                    --workingRequest;
                                }
                            });
                        }
                    }
                    else if (nextRequest.command == 3)
                    {
                        bool flag = false;
                        lock ("Try to exit")
                        {
                            if (workingRequest == 0)
                            {
                                requestQueue.Dequeue();
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    else if (nextRequest.command <= 1)
                    {
                        bool flag = false;
                        lock ("Try to update")
                        {
                            if (workingRequest == 0)
                            {
                                --workingRequest;
                                requestQueue.Dequeue();
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            if (nextRequest.command == 0)
                            {
                                var requestResult = await tree.Insert(nextRequest.x, nextRequest.y);
                                // Console.Write("result for request: insert " + nextRequest.x + " " + nextRequest.y);
                                // Console.WriteLine(" is " + (requestResult ? "success": "fail"));
                            }
                            else
                            {
                                var requestResult = await tree.Remove(nextRequest.x);
                                // Console.Write("result for request: remove " + nextRequest.x);
                                // Console.WriteLine(" is " + (requestResult ? "success": "fail"));
                            }
                            lock ("Increase counter")
                            {
                                ++workingRequest;
                            }
                        }
                    }
                }
            });
        }

    }

}