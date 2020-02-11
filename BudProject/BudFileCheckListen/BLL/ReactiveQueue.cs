using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BudFileCheckListen.BLL
{
         /// <summary>
      /// Delegate DequeueEventHandler.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="item"></param>
      public delegate void DequeueEventHandler<T>(List<T> item);
   
      /// <summary>
      /// A thread safe queue.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      public class ReactiveQueue<T> : IDisposable
      {
          private Queue<T> _queue;
          private bool _closed;
   
          /// <summary>
          /// Gets or sets a value indicating whether this <see 
          /// cref="ReactiveQueue&lt;T&gt;"/> is closed.
          /// </summary>
          /// <value><c>true</c> if closed; otherwise, <c>false</c>.</value>
          public bool Closed
          {
              get
              {
                  lock (_closeLock)
                  {
                      return _closed;
                  }
              }
              set
              {
                  lock (_closeLock)
                  {
                      _closed = value;
                  }
              }
          }
   
          /// <summary>
          /// Gets or sets the queue.
          /// </summary>
          /// <value>The queue.</value>
          internal Queue<T> Queue
          {
              get { return _queue; }
              set { _queue = value; }
          }
   
          private  object _syncLock = new object();
          private object _closeLock = new object();
          /// <summary>
          /// Gets or sets the sync lock.
          /// </summary>
          /// <value>The sync lock.</value>
          internal object SyncLock
          {
              get { return _syncLock; }
              set { _syncLock = value; }
          }
          
          /// <summary>
          /// Initializes a new instance of the ReactiveQueue class.
          /// </summary>
          public ReactiveQueue(int maxfactor = 5)
          {
              _queue = new Queue<T>();
              _maxFactor = maxfactor;
              ThreadPool.QueueUserWorkItem(OnEnqueue);           
          }
   
          /// <summary>
          /// Initializes a new instance of the ReactiveQueue class with the given 
          /// capacity.
          /// </summary>
          /// <param name="capacity">The capacity of the queue.</param>
          /// <param name="maxfactor">上限因子</param>
          public ReactiveQueue(int capacity, int maxfactor = 5)
              : this(maxfactor)
          {
              _queue = new Queue<T>(capacity);
          }

          /// <summary>
          /// 内存过大问题 20140706 xiecongwen 
          /// 用于设置上限因子 
          /// 因为Queue 默认初始容量（32）并使用默认增长因子（2.0）。
          /// 所以如果没有设置初始容量 则上限为32*5
          /// </summary>
          public int MaxFactor { get { return _maxFactor; } set { _maxFactor = value; } }
          private int _maxFactor = 5;
          /// <summary>
          /// Enqueues the specified item.
          /// 如果返回true 则表示插入成功
          /// 如果返回false 则表示插入失败
          /// </summary>
          /// <param name="item">The item.</param>
          public bool Enqueue(T item)
          {
              if (!Closed)
              {
                  lock (_syncLock)
                  {
                      if (_queue.Count >= 32 * _maxFactor)
                      {
                          return false;
                      }
                      _queue.Enqueue(item);
                      Monitor.PulseAll(_syncLock);
                  }
                  return true;
              }

              return false;
              
          }
   
          /// <summary>
          /// Occurs when [dequeue handler].
          /// </summary>
          public event DequeueEventHandler<T> DequeueHandler;
   
          /// <summary>
          /// Occurs when Enqueue is called.
          /// </summary>
          protected void OnEnqueue(object obj)
          {
              Thread.CurrentThread.Name = "ReactiveQueue";
              //消息队列中的消息，可能会丢失一部分
              while (!Closed)
              {
                  List<T> items = null;
                  lock (_syncLock)
                  {
                      if (_queue.Count == 0)
                      {
                          Monitor.Wait(_syncLock);
                      }
                      
                      if (DequeueHandler != null && _queue.Count > 0)
                      {
                          items = _queue.ToList();
                          _queue.Clear();
                      }                    
                  }
   
                  if (DequeueHandler != null && items != null && items.Count > 0)
                  {
                      DequeueHandler(items);       
                  }                
              }
          }
   
          /// <summary>
          /// Closes this instance.
          /// </summary>
          public void Close()
          {
              Dispose();
          }
          
          #region IDisposable Members
   
          /// <summary>
          /// Releases unmanaged and - optionally - managed resources
          /// </summary>
          /// <param name="disposing"><c>true</c> to release both managed and 
          /// unmanaged resources; <c>false</c> to release only unmanaged 
          /// resources.</param>
          private void Dispose(bool disposing)
          {
              if (!_disposed)
              {
                  if (disposing)
                  {
                      Closed = true;
                      lock (_syncLock)
                      {
                          Monitor.PulseAll(_syncLock);
                      }
                  }
              }
              _disposed = true;
          }
   
          private bool _disposed = false;
          /// <summary>
          /// Performs application-defined tasks associated with freeing, 
          /// releasing, or resetting unmanaged resources.
          /// </summary>
          public void Dispose()
          {
              Dispose(true);
              GC.SuppressFinalize(this);
          }
          
          /// <summary>
          /// Releases unmanaged resources and performs other cleanup operations 
          /// before the
          /// <see cref="SharedMemoryReceiverChannel"/> is reclaimed by garbage 
          /// collection.
          /// </summary>
          ~ReactiveQueue()
          {
              Dispose(false);
          }
   
          #endregion
   
   
      }

}
