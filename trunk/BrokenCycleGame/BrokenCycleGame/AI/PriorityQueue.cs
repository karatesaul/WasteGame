﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BrokenCycleGame.AI
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T> : IEnumerable<T>
    {
        private class HighestPriorityFirst : IComparer<int>
        {
            public int  Compare(int x, int y)
            {
                return y.CompareTo(x);
            }
        }

        private readonly Func<T, int> defaultPrioritySelector;
        private readonly SortedList<int, Queue<T>> queues;

        /// <summary>
        /// 
        /// </summary>
        public PriorityQueue()
            :this (new HighestPriorityFirst())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public PriorityQueue(IComparer<int> comparer)
        {
            queues = new SortedList<int, Queue<T>>(comparer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prioritySelector"></param>
        public PriorityQueue(Func<T, int> prioritySelector)
            :this()
        {
            defaultPrioritySelector = prioritySelector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prioritySelector"></param>
        /// <param name="comparer"></param>
        public PriorityQueue(Func<T, int> prioritySelector, IComparer<int> comparer)
            : this(comparer)
        {
            defaultPrioritySelector = prioritySelector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            if (defaultPrioritySelector == null)
            {
               throw new InvalidOperationException("Cannot add item without a priority when no priority selector is specified.");
            }
            Enqueue(item, defaultPrioritySelector(item));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="prioritySelector"></param>
        public void Enqueue(T item, Func<T, int> prioritySelector)
        {
            if (prioritySelector == null)
            {
                throw new ArgumentNullException("prioritySelector");
            }
            Enqueue(item, prioritySelector(item));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        public void Enqueue(T item, int priority)
        {
            Queue<T> queue;
            if (!queues.TryGetValue(priority, out queue))
            {
                queue = new Queue<T>();
                queues.Add(priority, queue);
            }
            queue.Enqueue(item);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
          
            var first = queues.First();
            Queue<T> queue = first.Value;
            var result = queue.Dequeue();
            if (queue.Count == 0)
            {
                queues.Remove(first.Key);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public T Dequeue(int priority)
        {
            var result = queues[priority].Dequeue();
            if (queues[priority].Count == 0)
            {
                queues.Remove(priority);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return queues.Sum(kvp => kvp.Value.Count);
            }
        }

        #region IEnumerable<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return queues.SelectMany(kvp => kvp.Value).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
