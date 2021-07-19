public static class EventManager
{
    public delegate void Action(EventData data);

    public static event Action OnAction;

    public static void InvokeOnAction(EventData data)
    {
        OnAction?.Invoke(data);
    }
}