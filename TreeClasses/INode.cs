using System.Collections.Generic;
using System;
using System.Linq;

namespace TreeLib 
{
    public interface INode<T> : IEquatable<T>, IEquatable<Node<T>>
    {        
        //new bool Equals(T other);
        //new bool Equals(Node<T> other);

        T Value {get;set;}
        int Depth {get;}

        void AddChild(Node<T> node);
        void AddChild(T item);

        Node<T> GetParent();


        bool IsLeaf();
        bool IsRoot();
    }
}