using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

namespace CSSParser.Tree
{
    public class Tree<T>: ITree<T>
    {
        public Node<T> root;


        public int Count {
            get {
                return ToList(TraversalMode.Breadth).Count;
            }
        }

        public bool IsReadOnly { get { return false; } }

        
        public Tree(T rootValue) {
            root = new Node<T>(rootValue);
        }

        public Tree(List<T> values) {
            root = new Node<T>(values);
        }

        public Tree(Node<T> root) {
            this.root = root;
        }

        public Tree(List<Node<T>> nodes) {
            root = new Node<T>(nodes);
        }

        public void Add(T item) {
            if (root == null) {
                root = new Node<T>(item);
                return;
            }

            root.AddChild(item);
        }

        public void Add(T item, Node<T> parent) {
            if (!this.Contains(parent)) {
                root.AddChild(parent);
            }

            parent.AddChild(item);
            
        }

        public void Add(Node<T> node) {
            if (root == null) {
                root = node;
                return;
            }

            root.AddChild(node);
        }

        public void Add(Node<T> node, Node<T> parent) {
            if (!this.Contains(parent)) {
                root.AddChild(parent);
            }

            parent.AddChild(node);
        }

        public void Clear() {
            //throw new NotImplementedException();

            ClearNode(ref root);
        }

        public bool Contains(Node<T> node)
        {
            foreach (var treeNode in GetEnumerable())
            {
                if (node == treeNode) {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(Predicate<Node<T>> match) {
            foreach (var treeNode in GetEnumerable())
            {
                if (match(treeNode)) {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(T item)
        {
            foreach (var treeNode in GetEnumerable())
            {
                if (treeNode.Value.Equals(item)) {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(Predicate<T> match)
        {
            foreach (var treeNode in GetEnumerable())
            {
                if (match(treeNode.Value)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Fast equivalent of Clear(). Sets the root to null. CAUTION ! This can cause a garbage collection,
        /// and therefore it's not recommended to call this method repeatedly. Prefer Clear() in this case.
        /// </summary>
        public void DereferenceRoot() {
            root = null;
        }

        void ClearNode(ref Node<T> node) {
            if (node.Childrens == null)
            {
                node = null;
                return;
            }

            node.Childrens.ToList().ForEach(n => ClearNode(ref n));

            node = null;
        }

        public Node<T> Find(Predicate<T> match) {
            var tempQueue = new Queue<Node<T>>();
            tempQueue.Enqueue(root);

            while (tempQueue.Count > 0)
            {
                var currentNode = tempQueue.Dequeue();
                
                if (match(currentNode.Value)) return currentNode;

                foreach (var child in currentNode._childrens)
                {
                    tempQueue.Enqueue(child);
                }
            }

            return null;
        }

        public Node<T> Find(Predicate<Node<T>> match) {
            var tempQueue = new Queue<Node<T>>();
            tempQueue.Enqueue(root);

            while (tempQueue.Count > 0)
            {
                var currentNode = tempQueue.Dequeue();
                
                if (match(currentNode)) return currentNode;

                foreach (var child in currentNode._childrens)
                {
                    tempQueue.Enqueue(child);
                }
            }

            return null;
        }

        public Node<T>[] FindAll(Predicate<T> match) {
            var output = new List<Node<T>>();

            var tempQueue = new Queue<Node<T>>();
            tempQueue.Enqueue(root);

            while (tempQueue.Count > 0)
            {
                var currentNode = tempQueue.Dequeue();
                
                if (match(currentNode.Value)) output.Add(currentNode);

                foreach (var child in currentNode._childrens)
                {
                    tempQueue.Enqueue(child);
                }
            }

            return output.ToArray();
        }

        public Node<T>[] FindAll(Predicate<Node<T>> match) {
            var output = new List<Node<T>>();

            var tempQueue = new Queue<Node<T>>();
            tempQueue.Enqueue(root);

            while (tempQueue.Count > 0)
            {
                var currentNode = tempQueue.Dequeue();
                
                if (match(currentNode)) output.Add(currentNode);

                foreach (var child in currentNode._childrens)
                {
                    tempQueue.Enqueue(child);
                }
            }

            return output.ToArray();
        }

        public bool Remove(T item) {
            var node = Find(new Predicate<T>((value) => value.Equals(item)));

            if (node != null) {
                ClearNode(ref node);
                return true;
            }

            return false;
        }

        public bool Remove(Node<T> node) {
            var foundNode = Find((otherNode) => otherNode == node);

            if (foundNode != null) {
                ClearNode(ref foundNode);
                return true;
            }

            return false;
        }

        // TODO: Implement the method (RemoveAll() in Tree.cs(244))
        public bool RemoveAll(Predicate<T> match) {
            /*foreach (var node in GetEnumerable())
            {
                if (match(node.Value)) {
                    node = null;
                }
            }*/

            return true;
        }

        // TODO: Implement the method (RemoveAll() in Tree.cs(255))
        public bool RemoveAll(Predicate<Node<T>> match) {
            /*foreach (var node in GetEnumerable())
            {
                if (match(node)) {
                    node = null;
                }
            }*/

            return true;
        }

        public IEnumerable<Node<T>> GetEnumerable() {
            var tempQueue = new Queue<Node<T>>();
            tempQueue.Enqueue(root);

            while (tempQueue.Count != 0) {
                var currentNode = tempQueue.Dequeue();

                foreach (var child in currentNode._childrens)
                {
                    tempQueue.Enqueue(child);
                }

                yield return currentNode;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return ToValueList(TraversalMode.Breadth).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return (IEnumerator) ToValueList(TraversalMode.Breadth).GetEnumerator();
        }

        IEnumerator<Node<T>> IEnumerable<Node<T>>.GetEnumerator() {
            return ToList(TraversalMode.Breadth).GetEnumerator();
        }

        public Node<T> BFS(T item)
        {
            var tempQueue = new Queue<Node<T>>();
            tempQueue.Enqueue(root);

            while (tempQueue.Count > 0)
            {
                var currentNode = tempQueue.Dequeue();
                
                if (currentNode.Value.Equals(item)) return currentNode;

                foreach (var child in currentNode._childrens)
                {
                    tempQueue.Enqueue(child);
                }
            }

            return null;
        }

        public Node<T> BFS(Node<T> node) {
            var tempQueue = new Queue<Node<T>>();
            tempQueue.Enqueue(root);

            while (tempQueue.Count > 0)
            {
                var currentNode = tempQueue.Dequeue();
                
                if (currentNode == node) return currentNode;

                foreach (var child in currentNode._childrens)
                {
                    tempQueue.Enqueue(child);
                }
            }

            return null;
        }

        public List<Node<T>> ToList(TraversalMode mode) {
            switch (mode)
            {
                case TraversalMode.Breadth:
                    return Breadth(root);
                case TraversalMode.PreOrder:
                    return PreOrder(root);
                case TraversalMode.InOrder:
                    return InOrder(root);
                case TraversalMode.PostOrder:
                    return PostOrder(root);
                default:
                    throw new NotImplementedException();
            }
        }

        public List<T> ToValueList(TraversalMode mode) {
            switch (mode)
            {
                case TraversalMode.Breadth:
                    return BreadthValue(root);
                case TraversalMode.PreOrder:
                    return PreOrderValue(root);
                case TraversalMode.InOrder:
                    return InOrderValue(root);
                case TraversalMode.PostOrder:
                    return PostOrderValue(root);
                default:
                    throw new NotImplementedException();
            }
        }

        static List<T> BreadthValue(Node<T> node) {
            var output = new List<T>();

            var tempQueue = new Queue<Node<T>>();
            tempQueue.Enqueue(node);

            while (tempQueue.Count != 0) {
                var currentNode = tempQueue.Dequeue();

                foreach (var child in currentNode._childrens)
                {
                    tempQueue.Enqueue(child);
                }

                output.Add(currentNode.Value);
            }

            return output;
        }

        static List<T> PreOrderValue(Node<T> node)
        {
            var output = new List<T>();

            output.Add(node.Value);

            foreach (var child in node._childrens)
            {
                output.Concat(PreOrderValue(child));
            }

            return output;
        }

        static List<T> InOrderValue(Node<T> node)
        {
            var output = new List<T>();

            int i = 0;

            for (; i < node._childrens.Count / 2; i++)
            {
                output = output.Concat(InOrderValue(node._childrens[i])).ToList();
            }

            output.Add(node.Value);

            for (; i < node._childrens.Count; i++)
            {
                output = output.Concat(InOrderValue(node._childrens[i])).ToList();
            }

            return output;
        }

        static List<T> PostOrderValue(Node<T> node)
        {
            var output = new List<T>();

            foreach (var child in node._childrens)
            {
                output.Concat(PostOrderValue(child));
            }

            output.Add(node.Value);

            return output;
        }
    

        static List<Node<T>> Breadth(Node<T> node) {
            var output = new List<Node<T>>();

            var tempQueue = new Queue<Node<T>>();
            tempQueue.Enqueue(node);

            while (tempQueue.Count != 0) {
                var currentNode = tempQueue.Dequeue();

                foreach (var child in currentNode._childrens)
                {
                    tempQueue.Enqueue(child);
                }

                output.Add(currentNode);
            }

            return output;
        }

        static List<Node<T>> PreOrder(Node<T> node)
        {
            var output = new List<Node<T>>();

            output.Add(node);

            foreach (var child in node._childrens)
            {
                output.Concat(PreOrder(child));
            }

            return output;
        }

        static List<Node<T>> InOrder(Node<T> node)
        {
            var output = new List<Node<T>>();

            int i = 0;

            for (; i < node._childrens.Count / 2; i++)
            {
                output = output.Concat(InOrder(node._childrens[i])).ToList();
            }

            output.Add(node);

            for (; i < node._childrens.Count; i++)
            {
                output = output.Concat(InOrder(node._childrens[i])).ToList();
            }

            return output;
        }

        static List<Node<T>> PostOrder(Node<T> node)
        {
            var output = new List<Node<T>>();

            foreach (var child in node._childrens)
            {
                output.Concat(PostOrder(child));
            }

            output.Add(node);

            return output;
        }
    
        public void CopyTo(Node<T>[] array, int copyIndex) {
            if (copyIndex == 0) {
                array = ToList(TraversalMode.Breadth).ToArray();
                return;
            }

            var treeArray = ToList(TraversalMode.Breadth).ToArray();

            for (int i = copyIndex; i < array.Length; i++)
            {
                array[i] = treeArray[i - copyIndex];
            }
        }

        public void CopyTo(T[] array, int copyIndex) {
            var treeArray = ToList(TraversalMode.Breadth).ToArray();

            for (int i = copyIndex; i < array.Length; i++)
            {
                array[i] = treeArray[i - copyIndex].Value;
            }
        }
    
        public override string ToString() {
            var strBuilder = new System.Text.StringBuilder("");

            foreach (var node in GetEnumerable())
            {
                strBuilder.AppendLine(node.ToString());
            }

            return strBuilder.ToString();
        } 
    }

    public enum TraversalMode {Breadth, PreOrder, InOrder, PostOrder}
}