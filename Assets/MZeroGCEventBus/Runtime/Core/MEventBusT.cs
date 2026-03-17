using System;
using MZeroGCEventBus.Core.Interface;

namespace MZeroGCEventBus.Core
{
    public static class MEventBusT<T>
    {
        //相对于座位编号，乘客上车时，给予凭证，凭证号用来标记乘客的座位号
        //用来标记订阅位，保证Unsubscribe时不会取消订阅错事件
        public struct ListenerSlot
        {
            public IMListener<T> listener;
            public int version;
        }

        private static int count = 0;

        private static ListenerSlot[] listeners = new ListenerSlot[128];

        //乘客订阅时，给予凭证
        public static MEventHandle Subscribe(IMListener<T> listener)
        {
            if (count >= listeners.Length)
            {
                Array.Resize(ref listeners, listeners.Length * 2);
            }

            int index = count++;
            
            listeners[index].listener = listener;
            listeners[index].version++;

            return new MEventHandle()
            {
                index = index,
                version = listeners[index].version
            };
        }

        //凭借凭证下车
        public static void Unsubscribe(MEventHandle handle)
        {
            if (handle.index >= count)
                return;
            
            ref var slot = ref listeners[handle.index];
            
            //格子版本和句柄的版本不同
            if (slot.version != handle.version)
                return;
            
            int last = count - 1;

            listeners[handle.index] = listeners[last];
            listeners[last].listener = null;

            count--;
        }

        //通知车上所有乘客
        public static void Publish(in T e)
        {
            for (int i = 0; i < count; i++)
            {
                listeners[i].listener.OnEvent(e);
            }
        }

        //到终点站清除所有乘客
        public static void Clear()
        {
            for (int i = 0; i < count; i++)
            {
                listeners[i].listener = null;
            }
            
            count = 0;
        }
    }
}