using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayQueue.Structure
{
    interface IDelayQueue<T> where T : IDelayItem
    {
        T Dequeue();

        void Enqueue(T item);
    }
}
