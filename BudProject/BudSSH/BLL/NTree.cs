using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudSSH.BLL
{
    /// <summary>
    /// 此树形结构用于表达目录的层次
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class NTree<T>
    {
        private T data;

        public T Data { get { return data; } }
        private int depth;

        public int Depth { get { return depth; } }

        private NTree<T> parent = null;
        public NTree<T> Parent { get { return parent; } }
        LinkedList<NTree<T>> children;

        public NTree(T data)
        {
            this.data = data;
            //默认是根节点
            this.depth = 0;
            this.parent = null;
            children = new LinkedList<NTree<T>>();
        }

        /// <summary>
        /// 用于添加子节点
        /// </summary>
        /// <param name="data"></param>
        /// <param name="depth"></param>
        private NTree(T data, NTree<T> parent, int depth)
        {
            this.data = data;
            this.parent = parent;
            this.depth = depth;
            children = new LinkedList<NTree<T>>();
        }

        public void AddChild(T data)
        {
            children.AddFirst(new NTree<T>(data, this, depth + 1));
        }

        public NTree<T> GetChild(int i)
        {
            foreach (NTree<T> n in children)
                if (--i == 0) return n;
            return null;
        }

        /// <summary>
        /// 获取第一个子节点
        /// </summary>
        /// <returns></returns>
        public NTree<T> FirstChild()
        {
            if (children.Count > 0)
            {
                return children.First.Value;
            }

            return null;
        }

        /// <summary>
        /// 判断是否有子节点
        /// </summary>
        /// <returns></returns>
        public bool HasChild()
        {
            if (children.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// clear
        /// </summary>
        public void RemoveAllChildren()
        {
            children.Clear();
        }

        /// <summary>
        /// 弹出一个子节点 并Remove
        /// </summary>
        /// <returns></returns>
        public NTree<T> PushChild()
        {
            if (children.Count > 0)
            {
                NTree<T> n = children.First.Value;
                children.RemoveFirst();

                return n;
            }

            return null;
        }

    }
}
