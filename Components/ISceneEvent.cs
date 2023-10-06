namespace Cobbs_Engine.Components
{
    public interface ISceneEvent
    {
        public string EventType { get; }
        public object Data { get; }
    }
}
