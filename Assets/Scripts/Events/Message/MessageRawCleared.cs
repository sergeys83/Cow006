public class MessageRawCleared : Messages
{
    public int player;
    public TargetRowFinder RowFinder;
    public MessageRawCleared(TargetRowFinder RowFinder, int _player, CardCowMover card)
    {
        this.RowFinder =RowFinder;
        this.player = _player;
        Card = card;
    }
}
