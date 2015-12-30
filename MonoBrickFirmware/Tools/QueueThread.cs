using System;
using System.Collections.Generic;
using System.Threading;
using MonoBrickFirmware.Display;

namespace MonoBrickFirmware.Tools
{
    public class QueueThread : IDisposable
    {
        private QueueThread()
        {
            Thread t = new Thread(ThreadFunc);
            t.IsBackground = true;
            t.Start();
        }

        public static QueueThread Instance
        {
            get { return Nested.instance; }
        }

        private class Nested
        {
            static Nested() { }
            internal static readonly QueueThread instance = new QueueThread();
        }

        private Queue<EventNode> queue = new Queue<EventNode>();
        private EventWaitHandle stop = new EventWaitHandle(false, EventResetMode.ManualReset);
        private void ThreadFunc()
        {
            while (!stop.WaitOne(0))
            {
                EventNode nodeToRaise = null;
                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        nodeToRaise = queue.Dequeue();
                    }
                    else
                    {
                        Monitor.Wait(queue);
                    }
                }
                if ((nodeToRaise != null) && (nodeToRaise.eventToRaise != null))
                {
                    Delegate eventToRaise = nodeToRaise.eventToRaise;
                    object[] parameters = nodeToRaise.parameters;

                    try
                    {
                        eventToRaise.DynamicInvoke(parameters);
                    }
                    catch (Exception ex)
                    {
                        LcdConsole.WriteLine(ex.ToString());
                    }
                }
            }
        }

        public void Enqueue(Delegate eventToEnqueue, params Object[] paramaters)
        {
            lock (queue)
            {
                EventNode node = new EventNode(eventToEnqueue, paramaters);
                queue.Enqueue(node);
                Monitor.Pulse(queue);
            }
        }

        public void Dispose()
        {
            lock (queue)
            {
                stop.Set();
                Monitor.Pulse(queue);
            }
        }
    }
}

