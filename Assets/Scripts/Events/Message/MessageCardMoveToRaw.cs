using UnityEngine;
public class MessageCardMoveToRaw:Messages
{
    public MessageCardMoveToRaw(CardCowMover card, Transform target)
    {
        Card = card;
        Target = target;
    }
}
