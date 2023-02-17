using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranVinh.Helper
{
    internal class MainThreadDispatcher
    {
        internal static void dispatcher(Action action)
        {
            Queue.Enqueue(action);
        }

        internal static void update()
        {
            while (Queue.Count > 0)
            {
                Queue.Dequeue()();
            }
        }

        static readonly Queue<Action> Queue = new Queue<Action>();
    }
}
