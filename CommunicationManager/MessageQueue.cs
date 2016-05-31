/////////////////////////////////////////////////////////////////////////////
//  MessageQueue.cs - demonstrate threads communicating via Queue         //
//  ver 4.0                                                                //
//  Language:     C#, VS 2003                                              //
//  Platform:     Dell Dimension 8100, Windows 2000 Pro, SP2               //
//  Application:  Demonstration for CSE681 - Software Modeling & Analysis  //
//  Author:       Jim Fawcett, CST 2-187, Syracuse University              //
//                (315) 443-3948, jfawcett@twcny.rr.com                    //
/////////////////////////////////////////////////////////////////////////////
/*
 *   Module Operations
 *   -----------------
 *   This module demonstrates communication between two threads using a 
 *   blocking queue.  If the queue is empty when the reader attempts to dequeue
 *   an item then the reader will block until the writing thread enqueues an item.
 *   Thus waiting is efficient.
 * 
 *   NOTE:
 *   This blocking queue is implemented using a Monitor and lock, which is
 *   equivalent to using a condition variable with a lock.

 *   Build Process
 *   -------------
 *   - Required files:   BlockingQueue.cs, Program.cs
 *   - Compiler command: csc BlockingQueue.cs Program.cs
 * 
 *   Maintenance History
 *   -------------------
 *
  * ver 2.0 : 18 Nov 2015
 * Added IsEMPTYQ() method

 *   ver 1.0 : 22 October 2013
 *     - first release
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace RemoteNoSQL
{
    public class MessageQueue<T>
    {
        private Queue BQueue;
        object Blocker = new object();

        //----< constructor >--------------------------------------------

        public MessageQueue()
        {
            BQueue = new Queue();
        }
        //----< enqueue a string >---------------------------------------
        public void enqueue(T message)
        {
            lock (Blocker)
            {
                BQueue.Enqueue(message);
                Monitor.Pulse(Blocker);
            }
        }

        //----< dequeue a T >---------------------------------------
        //
        // Note that the entire deQ operation occurs inside lock.
        // Need a Monitor (or condition variable) to do this.

        public T dequeue()
        {
            T message = default(T);
            lock (Blocker)
            {
                while (this.Length() == 0)
                    Monitor.Wait(Blocker);
                message = (T)BQueue.Dequeue();
                return message;
            }
        }

        public int Length()  // get length of Q
        {
            int length;
            lock (Blocker)
                length = BQueue.Count;
            return length;
        }
         
        // Delete all elements from Q
        public void DeleteAll()
        {
            lock (Blocker)
                BQueue.Clear();
        }

        // Checks whether a Q is empty or not
        public bool IsEMPTYQ()
        {
            if (BQueue.Count == 0)
                return true;
            else
                return false;
        }
    }
}