public class MessageMinRawelected : Messages
{
    public int player;
    public MessageMinRawelected(CardCowMover card, int rawIndex, int _player)
    {
        Card = card;
        base.rawIndex = rawIndex-1;
        this.player = _player;
    }
    
    public MessageMinRawelected(int rawIndex, int _player)
    {
        base.rawIndex = rawIndex-1;
        this.player = _player;
    }
}