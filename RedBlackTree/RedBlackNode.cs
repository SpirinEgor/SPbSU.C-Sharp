using System;

namespace RedBlackTree
{

    public class Node<K, V> where K: IComparable
    {
        private K key;
        private V value;
        private bool color = false; // false - black, true - red
        private Node<K, V> left = null;
        private Node<K, V> right = null;
        private Node<K, V> parent = null;
    
        public K Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        public V Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public bool Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public Node<K, V> Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
            }
        }

        public Node<K, V> Right
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
            }
        }

        public Node<K, V> Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }        

        public Node(K keyNew, V valueNew, bool colorNew)
        {
            key = keyNew;
            value = valueNew;
            color = colorNew;
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
                    return key.Equals(other.Key) && value.Equals(other.Value) && color == other.Color;
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

        public Node<K, V> GetMinimum(Node<K, V> nil)
        {
            if (left == nil)
            {
                return this;
            }
            else
            {
                return left.GetMinimum(nil);
            }
        }

        public Node<K, V> GetMaximum(Node<K, V> nil)
        {
            if (right == null)
            {
                return this;
            }
            else
            {
                return right.GetMaximum(nil);
            }
        }

        public Node<K, V> RotateLeft(Node<K, V> rootOld, Node<K, V> nil)
        {
            var root = rootOld;
            var rightSon = right;
            right = rightSon.Left;
            if (rightSon.Left != nil)
            {
                rightSon.Left.Parent = this;
            }
            rightSon.Parent = parent;
            if (parent == nil)
            {
                root = rightSon;
            }
            else if (this == parent.Right)
            {
                parent.Right = rightSon;
            }
            else
            {
                parent.Left = rightSon;
            }
            rightSon.Left = this;
            parent = rightSon;
            return root;
        }

        public Node<K, V> RotateRight(Node<K, V> rootOld, Node<K, V> nil)
        {
            var root = rootOld;
            var leftSon = left;
            left = leftSon.Right;
            if (leftSon.Right != nil)
            {
                leftSon.Right.Parent = this;
            }
            leftSon.Parent = parent;
            if (parent == nil)
            {
                root = leftSon;
            }
            else if (this == parent.Right)
            {
                parent.Right = leftSon;
            }
            else
            {
                parent.Left = leftSon;
            }
            leftSon.Right = this;
            parent = leftSon;
            return root;
        }

        public void Print()
        {
            string res = "(" + key + "/" + value + "/";
            if (color)
            {
                res += "RED";
            }
            else
            {
                res += "BLACK";
            }
            res += ")";
            System.Console.Write(res);
        }
    }

}