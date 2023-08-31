namespace SCHIZO.Events
{
    public interface ICustomEvent
    {
        string Name { get; }
        bool IsOccurring { get; set; }
        void StartEvent();
        void EndEvent();
    }
}
