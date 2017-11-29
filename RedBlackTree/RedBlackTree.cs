using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedBlackTree
{
    public class Tree<K, V> where K: IComparable
    {
        public Node<K, V> Root {get; set;}
        private Node<K, V> nil = null;

        public Tree()
        {
            Root = null;
        }

        public async Task<Node<K, V>> Find(K key)
        {
            return await Task.Run(() => {
                if (Root == null)
                {
                    return null;
                }
                var current = Root;
                while (current != nil)
                {
                    if (key.Equals(current.Key))
                    {
                        return current;
                    }              
                    else if (key.CompareTo(current.Key) < 0)
                    {
                        current = current.Left;
                    }  
                    else
                    {
                        current = current.Right;
                    }
                }
                return null;
            });
        }

        public async Task<bool> Contain(K key)
        {
            return await Task.Run(async () =>
            {
                return (await Find(key)) != null;
            });
        }

        private async Task FixInsert(Node<K, V> add)
        {
            await Task.Run(async () =>
            {
                var current = add;
                while (!current.Equals(Root) && current.Parent.Color)
                {
                    if (current.Parent.Equals(current.Parent.Parent.Left))
                    {
                        var grandDad = current.Parent.Parent.Right;
                        if (grandDad.Color)
                        {
                            current.Parent.Color = false;
                            grandDad.Color = false;
                            current.Parent.Parent.Color = true;
                            current = current.Parent.Parent;
                        }
                        else
                        {
                            if (current.Equals(current.Parent.Right))
                            {
                                current = current.Parent;
                                Root = await current.RotateLeft(Root, nil);
                            }
                            current.Parent.Color = false;
                            current.Parent.Parent.Color = true;
                            Root = await current.Parent.Parent.RotateRight(Root, nil);
                        }
                    }
                    else {
                        var grandDad = current.Parent.Parent.Left;
                        if (grandDad.Color)
                        {
                            current.Parent.Color = false;
                            grandDad.Color = false;
                            current.Parent.Parent.Color = true;
                            current = current.Parent.Parent;
                        }
                        else
                        {
                            if (current.Equals(current.Parent.Left))
                            {
                                current = current.Parent;
                                Root = await current.RotateRight(Root, nil);
                            }
                            current.Parent.Color = false;
                            current.Parent.Parent.Color = true;
                            Root = await current.Parent.Parent.RotateLeft(Root, nil);
                        }
                    }
                }
                Root.Color = false;
            });
        }

        public async Task<bool> Insert(K key, V value)
        {
            return await Task.Run(async () => 
            {
                if (Root == null)
                {
                    nil = new Node<K, V>(key, value, false);
                    Root = new Node<K, V>(key, value, false);
                    Root.Parent = nil;
                    Root.Left = nil;
                    Root.Right = nil;
                    return true;
                }
                if (await Contain(key))
                {
                    return false;
                }
                var newNode = new Node<K, V>(key, value, true);
                Node<K, V> dad = nil;
                var current = Root;
                while (current != nil)
                {
                    dad = current;
                    if (key.CompareTo(current.Key) < 0)
                    {
                        current = current.Left;
                    }
                    else
                    {
                        current = current.Right;
                    }
                }
                newNode.Parent = dad;
                if (dad == nil)
                {
                    Root = newNode;
                }
                else if (key.CompareTo(dad.Key) < 0)
                {
                    dad.Left = newNode;
                }
                else
                {
                    dad.Right = newNode;
                }
                newNode.Left = nil;
                newNode.Right = nil;
                await FixInsert(newNode);
                return true;
            });
        }

        private async Task Transplate(Node<K, V> oldNode, Node<K, V> newNode)
        {   
            await Task.Run(() =>
            {
                if (oldNode.Parent.Equals(nil))
                {
                    newNode.Parent = nil;
                    Root = newNode;
                }
                else
                {
                    if (oldNode.Equals(oldNode.Parent.Left))
                    {
                        oldNode.Parent.Left = newNode;
                    }
                    else
                    {
                        oldNode.Parent.Right = newNode;
                    }
                    newNode.Parent = oldNode.Parent;
                }
            });
        }

        private async Task FixRemove(Node<K, V> replace)
        {
            await Task.Run(async () =>
            {
                var current = replace;
                Node<K, V> brother;
                while (!current.Equals(Root) && !current.Color)
                {
                    if (current.Equals(current.Parent.Left))
                    {
                        brother = current.Parent.Right;
                        if (brother.Color)
                        {
                            brother.Color = false;
                            current.Parent.Color = true;
                            Root = await current.Parent.RotateLeft(Root, nil);
                            brother = current.Parent.Right;
                        }
                        if (!brother.Left.Color && !brother.Right.Color)
                        {
                            brother.Color = true;
                            current = current.Parent;
                        }
                        else
                        {
                            if (!brother.Right.Color)
                            {
                                brother.Left.Color = false;
                                brother.Color = true;
                                Root = await brother.RotateRight(Root, nil);
                                brother = current.Parent.Right;
                            }
                            brother.Color = current.Parent.Color;
                            current.Parent.Color = false;
                            brother.Right.Color = false;
                            Root = await current.Parent.RotateLeft(Root, nil);
                            current = Root;
                        }
                    }
                    else
                    {
                        brother = current.Parent.Left;
                        if (brother.Color)
                        {
                            brother.Color = false;
                            current.Parent.Color = true;
                            Root = await current.Parent.RotateRight(Root, nil);
                            brother = current.Parent.Left;
                        }
                        if (!brother.Left.Color && !brother.Right.Color)
                        {
                            brother.Color = true;
                            current = current.Parent;
                        }
                        else 
                        {
                            if (!brother.Left.Color)
                            {
                                brother.Right.Color = false;
                                brother.Color = true;
                                Root = await brother.RotateLeft(Root, nil);
                                brother = current.Parent.Left;
                            }
                            brother.Color = current.Parent.Color;
                            current.Parent.Color = false;
                            brother.Left.Color = false;
                            Root = await current.Parent.RotateRight(Root, nil);
                            current = Root;
                        }
                    }
                }
                current.Color = false;
            });
        }

        public async Task<bool> Remove(K key)
        {
            return await Task.Run(async () =>
            {
                if (Root == null)
                {
                    return false;
                }
                var removing = await Find(key);
                if (removing == null)
                {
                    return false;
                }
                var removingMove = removing;
                var originalColor = removing.Color;
                Node<K, V> replace;
                if (removing.Left == nil)
                {
                    replace = removing.Right;
                    await Transplate(removing, removing.Right);
                }
                else if (removing.Right == nil)
                {    
                    replace = removing.Left;
                    await Transplate(removing, removing.Left);
                }
                else
                {   
                    removingMove = await removing.Right.GetMinimum(nil);  
                    originalColor = removingMove.Color;
                    replace = removingMove.Right;
                    if (removingMove.Parent.Equals(removing))
                    {
                        replace.Parent = removingMove;
                    }
                    else
                    {
                        await Transplate(removingMove, removingMove.Right);
                        removingMove.Right = removing.Right;
                        removingMove.Right.Parent = removingMove;
                    }
                    await Transplate(removing, removingMove);
                    removingMove.Left = removing.Left;
                    removingMove.Left.Parent = removingMove;
                    removingMove.Color = removing.Color;
                }
                if (!originalColor)
                {
                    await FixRemove(replace);
                }
                return true;
            });
        }

    }
}