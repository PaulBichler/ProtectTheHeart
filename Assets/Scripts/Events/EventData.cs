public class EventData
{
    protected EventData(EventCategory eventCategory, EventAction eventAction)
    {
        EventAction = eventAction;
        EventCategory = eventCategory;
    }

    public EventAction EventAction { get; }

    public EventCategory EventCategory { get; }
}