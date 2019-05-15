using System.Collections.Generic;
using System;
using System.Linq;

namespace CSSParser.Tree
{
    public interface ITree<T> : ICollection<Node<T>>, ICollection<T>
    {
        // new IEnumerator<T> GetEnumerator();
        // //IEnumerator<T> IEnumerable.GetEnumerator();
        // new int Count {get;}

        // new bool IsReadOnly {get;}

        // new void Add(T item);
        void Add(T item, Node<T> parent);
        //new void Add(Node<T> node);
        void Add(Node<T> node, Node<T> parent);


        Node<T> BFS(Node<T> root);

        Node<T> BFS(T root);


        //new void Clear();


        Node<T> Find(Predicate<T> match);

        Node<T> Find(Predicate<Node<T>> match);


        Node<T>[] FindAll(Predicate<T> match);

        Node<T>[] FindAll(Predicate<Node<T>> match);


        //new bool Contains(Node<T> node);
        //new bool Contains(Predicate<Node<T>> match);

        //bool Contains(INode<T> node, SearchMode mode);
        //new bool Contains(T item);
        bool Contains(Predicate<T> match);

        //bool Contains(T item, SearchMode mode);

        // new void CopyTo(T[] array, int copyIndex);
        // new void CopyTo(Node<T>[] array, int copyIndex);

        // new bool Remove(T item);
        // new bool Remove(Node<T> node);

        bool RemoveAll(Predicate<T> match);
        bool RemoveAll(Predicate<Node<T>> match);
    }
}