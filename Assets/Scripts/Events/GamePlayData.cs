public class GamePlayData : EventData
{
    public GamePlayData(EventAction eventAction, int playerIndex, int killerIndex) : base(EventCategory.GamePlay, eventAction)
    {
        PlayerIndex = playerIndex;
        KillerIndex = killerIndex;
    }

    public int KillerIndex { get; }
    public int PlayerIndex { get; }
}