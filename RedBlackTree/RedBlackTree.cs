using System;
using System.Collections.Generic;

namespace RedBlackTree
{
    public class Tree<K, V> where K: IComparable
    {
        private Node<K, V> root = null;
        private Node<K, V> nil = null;

        public Node<K, V> Root
        {
            get
            {
                return root;
            }
        }

        public Node<K, V> Find(K key)
        {
            if (root == null)
            {
                return null;
            }
            var current = root;
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
        }

        public bool Contain(K key)
        {
            return Find(key) != null;
        }

        private void FixInsert(Node<K, V> add)
        {
            var current = add;
            while (!current.Equals(root) && current.Parent.Color)
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
                            root = current.RotateLeft(root, nil);
                        }
                        current.Parent.Color = false;
                        current.Parent.Parent.Color = true;
                        root = current.Parent.Parent.RotateRight(root, nil);
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
                            root = current.RotateRight(root, nil);
                        }
                        current.Parent.Color = false;
                        current.Parent.Parent.Color = true;
                        root = current.Parent.Parent.RotateLeft(root, nil);
                    }
                }
            }
            root.Color = false;
        }

        public bool Insert(K key, V value)
        {
            if (root == null)
            {
                nil = new Node<K, V>(key, value, false);
                root = new Node<K, V>(key, value, false);
                root.Parent = nil;
                root.Left = nil;
                root.Right = nil;
                return true;
            }
            if (Contain(key))
            {
                return false;
            }
            var newNode = new Node<K, V>(key, value, true);
            Node<K, V> dad = nil;
            var current = root;
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
                root = newNode;
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
            FixInsert(newNode);
            return true;
        }

        private void Transplate(Node<K, V> oldNode, Node<K, V> newNode)
        {   
            if (oldNode.Parent.Equals(nil))
            {
                newNode.Parent = nil;
                root = newNode;
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
        }

        private void FixRemove(Node<K, V> replace)
        {
            var current = replace;
            Node<K, V> brother;
            while (!current.Equals(root) && !current.Color)
            {
                if (current.Equals(current.Parent.Left))
                {
                    brother = current.Parent.Right;
                    if (brother.Color)
                    {
                        brother.Color = false;
                        current.Parent.Color = true;
                        root = current.Parent.RotateLeft(root, nil);
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
                            root = brother.RotateRight(root, nil);
                            brother = current.Parent.Right;
                        }
                        brother.Color = current.Parent.Color;
                        current.Parent.Color = false;
                        brother.Right.Color = false;
                        root = current.Parent.RotateLeft(root, nil);
                        current = root;
                    }
                }
                else
                {
                    brother = current.Parent.Left;
                    if (brother.Color)
                    {
                        brother.Color = false;
                        current.Parent.Color = true;
                        root = current.Parent.RotateRight(root, nil);
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
                            root = brother.RotateLeft(root, nil);
                            brother = current.Parent.Left;
                        }
                        brother.Color = current.Parent.Color;
                        current.Parent.Color = false;
                        brother.Left.Color = false;
                        root = current.Parent.RotateRight(root, nil);
                        current = root;
                    }
                }
            }
            current.Color = false;
        }

        public bool Remove(K key)
        {
            if (root == null)
            {
                return false;
            }
            var removing = Find(key);
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
                Transplate(removing, removing.Right);
            }
            else if (removing.Right == nil)
            {    
                replace = removing.Left;
                Transplate(removing, removing.Left);
            }
            else
            {   
                removingMove = removing.Right.GetMinimum(nil);  
                originalColor = removingMove.Color;
                replace = removingMove.Right;
                if (removingMove.Parent.Equals(removing))
                {
                    replace.Parent = removingMove;
                }
                else
                {
                    Transplate(removingMove, removingMove.Right);
                    removingMove.Right = removing.Right;
                    removingMove.Right.Parent = removingMove;
                }
                Transplate(removing, removingMove);
                removingMove.Left = removing.Left;
                removingMove.Left.Parent = removingMove;
                removingMove.Color = removing.Color;
            }
            if (!originalColor)
            {
                FixRemove(replace);
            }
            return true;
        }

        public void Draw()
        {
            if (root == null || root == nil)
            {
                System.Console.WriteLine("Дерево еще не создано");
                return;
            }
            var list = new List<Node<K, V>>();
            list.Add(root);
            var isPrint = true;
            var indent = 60;
            while (isPrint)
            {
                isPrint = false;
                indent = indent / 2;   //регулировка кривости 2.0
                var newList = new List<Node<K, V>>();
                for (int i = 0; i < list.Count; ++i)
                {
                    for (int j = 0; j < indent; ++j)
                    {
                        System.Console.Write(" ");
                    }
                    if (list[i] == null){
                        System.Console.Write(" null ");
                        newList.Add(null);
                        newList.Add(null);
                    }
                    else if (list[i] == nil)
                    {
                        System.Console.Write(" nil ");
                        newList.Add(null);
                        newList.Add(null);
                    }
                    else
                    {
                        isPrint = true;
                        list[i].Print();
                        newList.Add(list[i].Left);
                        newList.Add(list[i].Right);
                    }
                }
                System.Console.WriteLine();
                list = new List<Node<K, V>>(newList);
            }
        }
    }
}