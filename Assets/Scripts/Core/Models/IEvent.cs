namespace Core.Interfaces {
    public interface IEventOn<T> {
        void On(T triggerEvent);
    }
}
