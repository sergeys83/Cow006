using UnityEngine;
public class MessageCardMoveToTempRaw : Messages
{
    public TargetRowFinder _targetRowFinder;
    public MessageCardMoveToTempRaw(TargetRowFinder target, CardCowMover card)
    {
        Card = card;
        this._targetRowFinder = target;
    }
}