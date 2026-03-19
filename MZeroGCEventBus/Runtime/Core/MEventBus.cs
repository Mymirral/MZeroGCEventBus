

using MZeroGCEventBus.Runtime.Core.Interface;

namespace MZeroGCEventBus.Runtime.Core
{
    public class MEventBus
    {
        public static MEventHandle Subscribe<T>(IMListener<T> listener)
        {
            return MEventBusT<T>.Subscribe(listener);
        }
        
        public static void Unsubscribe<T>(MEventHandle handle)
        {
            MEventBusT<T>.Unsubscribe(handle);
        }

        public static void Publish<T>(in T e)
        {
            MEventBusT<T>.Publish(e);
        }

        public static void Clear<T>()
        {
            MEventBusT<T>.Clear();
        }
    }
}