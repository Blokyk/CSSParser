using System.Collections.Generic;
using System;
using System.Linq;

namespace TreeLib
{
    public class Node<T> : INode<T>
    {

        public List<Node<T>> _childrens;
        Node<T> _parent;

        T _value;

        int _depth;

        int _height;

        public Node<T> Parent {
            get { return _parent; } 
            private set { _parent = value; }
        }
        public List<Node<T>> Childrens {
            get { return _childrens; }
            private set { _childrens = value; }
        }

        public T Value {
            get { return _value; }
            set {_value = value; }
        }

        public int Depth {
            get { return _depth; }
            private set { _depth = value; }
        }

        public int Height {
            get { return _height; }
            private set { _height = value; }
        }
        

        public Node(Node<T> node) : this(node, node._parent) { }

        public Node(Node<T> node, Node<T> parent)
        {
            _value = node._value;
            _childrens = node._childrens;
            _depth = node._depth;
            _height = node._height;
            this._parent = parent;
        }

        public Node(T value)
        {
            this._value = value;
            _childrens = new List<Node<T>>();
            _depth = 1;
            _height = 0;
            this._parent = this;
        }

        public Node(T value, Node<T> parent)
        {
            this._value = value;
            _childrens = new List<Node<T>>();
            _depth = parent._depth + 1;
            _height = 0;
            this._parent = parent;
        }

        public Node(List<Node<T>> nodes) : this(new Node<T>(nodes[0]) { _depth = 0, _height = 1 })
        {

            nodes = new List<Node<T>>(nodes);
            nodes.RemoveAt(0);

            foreach (var node in nodes)
            {
                this.AddChild(node);
            }
        }

        public Node(List<T> values) : this(new Node<T>(values[0]) { _depth = 0, _height = 1 }){
            values = new List<T>(values);
            values.RemoveAt(0);

            foreach (var value in values)
            {
                this.AddChild(value);
            }
        }

        public Node<T> GetParent()
        {
            return _parent;
        }

        public void AddChild(T item)
        {
            if (_childrens.Count == 0) _height = 1;

            _childrens.Add(new Node<T>(item, this) { _depth = this._depth + 1, _height = 0 });
        }

        public void AddChild(Node<T> node)
        {
            if (_childrens.Count == 0) _height = 1 + node._height;

            node._parent = this;
            node._depth = this._depth + 1;
            _childrens.Add(node);
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public bool IsLeaf()
        {
            return (_childrens == null || _childrens.Count == 0);
        }

        public bool IsRoot() {
            return (_parent == this);
        }

        public bool Equals(Node<T> other) {
            return (other.GetHashCode() == GetHashCode());
        }

        public bool Equals(T other) {
            return other.Equals(_value);
        }
    }
}