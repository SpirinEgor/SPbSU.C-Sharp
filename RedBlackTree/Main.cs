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
            Console.WriteLine("insert x y\nremove x\nfind x\nexit");
            Task input = ControlInput();
            Task queryProcessor = QueryProcessor();
            input.Wait();
            Console.WriteLine("Work with input has ended");
            queryProcessor.Wait();
            Console.WriteLine("All requests processed");
        }

        static Task ControlInput()
        {
            return Task.Run(() => 
            {
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
                        lock ("Enqueue lock")
                        {
                            requestQueue.Enqueue(request);
                            System.Console.WriteLine(String.Join(" ", input) + " successfully enqueued");
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
            return Task.Run(() => 
            {
                int workingRequest = 0; // >= 0 - start find, < 0 - currently working with updates
                var tree = new Tree<int, int>();
                while (true)
                {
                    if (requestQueue.Count == 0)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    lock ("Peek lock")
                    {
                        Request nextRequest = requestQueue.Peek();
                        if (nextRequest.command == 2 && workingRequest >= 0)
                        {
                            ++workingRequest;
                            requestQueue.Dequeue();
                            ThreadPool.QueueUserWorkItem(async _ =>
                            {
                                var requestResult = await tree.Find(nextRequest.x);
                                Console.Write("result for request: find " + nextRequest.x);
                                Console.WriteLine(" is " + (requestResult == null ? "no such key" : requestResult.Value.ToString()));
                                --workingRequest;
                            });
                        }
                        else if (nextRequest.command == 3 && workingRequest == 0)
                        {
                            requestQueue.Dequeue();
                            break;
                        }
                        else if (nextRequest.command <= 1 && workingRequest == 0)
                        {
                            --workingRequest;
                            requestQueue.Dequeue();
                            if (nextRequest.command == 0)
                            {
                                ThreadPool.QueueUserWorkItem(async _ =>
                                {
                                    var requestResult = await tree.Insert(nextRequest.x, nextRequest.y);
                                    Console.Write("result for request: insert " + nextRequest.x + " " + nextRequest.y);
                                    Console.WriteLine(" is " + (requestResult ? "success": "fail"));
                                    ++workingRequest;
                                });
                            }
                            else
                            {
                                ThreadPool.QueueUserWorkItem(async _ =>
                                {
                                    var requestResult = await tree.Remove(nextRequest.x);
                                    Console.Write("result for request: remove " + nextRequest.x);
                                    Console.WriteLine(" is " + (requestResult ? "success": "fail"));
                                    ++workingRequest;
                                });
                            }
                        }
                    }
                }
            });
        }

    }

}