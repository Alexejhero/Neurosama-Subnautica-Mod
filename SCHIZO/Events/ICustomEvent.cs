namespace SCHIZO.Events
{
    public interface ICustomEvent
    {
        string Name { get; }
        bool IsOccurring { get; }
        void StartEvent();
        void EndEvent();
    }
}
