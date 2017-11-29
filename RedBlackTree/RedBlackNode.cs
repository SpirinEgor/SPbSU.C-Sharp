using System;
using System.Threading.Tasks;

namespace RedBlackTree
{

    public class Node<K, V> where K: IComparable
    {
        public K Key {get; set;}
        public V Value {get; set;}
        public bool Color {get; set;}   // false - black, true - red
        public Node<K, V> Left {get; set;}
        public Node<K, V> Right {get; set;}
        public Node<K, V> Parent {get; set;}

        public Node(K keyNew, V valueNew, bool colorNew)
        {
            Key = keyNew;
            Value = valueNew;
            Color = colorNew;
            Left = null;
            Right = null;
            Parent = null;
        }

        public override bool Equals(object obj)
        {
            if (obj is Node<K, V>)
            {
                var other = obj as Node<K, V>;
                if (other == null)
                {
                    return false;
                }
                else
                {
                    return Key.Equals(other.Key) && Value.Equals(other.Value) && Color == other.Color;
                }
            }
            else
            {
                return false;    
            }
        }

        public override int GetHashCode()
        {
            // TODO: create hash for node
            return 0;
        }

        public async Task<Node<K, V>> GetMinimum(Node<K, V> nil)
        {
            return await Task.Run(async () => 
            {
                if (Left == nil)
                {
                    return this;
                }
                else
                {
                    return await Left.GetMinimum(nil);
                }
            });
        }

        public async Task<Node<K, V>> GetMaximum(Node<K, V> nil)
        {
            return await Task.Run(async () =>
            {
                if (Right == null)
                {
                    return this;
                }
                else
                {
                    return await Right.GetMaximum(nil);
                }
            });
        }

        public async Task<Node<K, V>> RotateLeft(Node<K, V> rootOld, Node<K, V> nil)
        {
            return await Task.Run(() => 
            {
                var root = rootOld;
                var rightSon = Right;
                Right = rightSon.Left;
                if (rightSon.Left != nil)
                {
                    rightSon.Left.Parent = this;
                }
                rightSon.Parent = Parent;
                if (Parent == nil)
                {
                    root = rightSon;
                }
                else if (this == Parent.Right)
                {
                    Parent.Right = rightSon;
                }
                else
                {
                    Parent.Left = rightSon;
                }
                rightSon.Left = this;
                Parent = rightSon;
                return root;
            });
        }

        public async Task<Node<K, V>> RotateRight(Node<K, V> rootOld, Node<K, V> nil)
        {
            return await Task.Run(() =>
            {
                var root = rootOld;
                var leftSon = Left;
                Left = leftSon.Right;
                if (leftSon.Right != nil)
                {
                    leftSon.Right.Parent = this;
                }
                leftSon.Parent = Parent;
                if (Parent == nil)
                {
                    root = leftSon;
                }
                else if (this == Parent.Right)
                {
                    Parent.Right = leftSon;
                }
                else
                {
                    Parent.Left = leftSon;
                }
                leftSon.Right = this;
                Parent = leftSon;
                return root;
            });
        }

    }

}