namespace GPOyun.Core.FSM
{
    public interface IMiniStateMachine
    {
        string MachineName { get; }
        string GetCurrentState();
        void OnEvent(string eventName);
    }
}
