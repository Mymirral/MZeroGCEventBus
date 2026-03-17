namespace MZeroGCEventBus.Core.Interface
{
    public interface IMListener<T>
    {
        public void OnEvent(in T e);
    }
}
