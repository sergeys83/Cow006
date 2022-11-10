using UnityEngine;

public class SoundEventListener : MonoBehaviour
{
    public AudioClip clip;
    private MessageCardMoved messageCardMoved;
    private MessageClicked messageClicked;
    private MessageGameEnded messageGameEnded;

    // Start is called before the first frame update
    void Awake()
    {
        EventManager.Instance.Sub(EventId.CardMoved, () => onCardMoved(EventManager.Instance.msg as MessageCardMoved));
        EventManager.Instance.Sub(EventId.EndGame, () => onEndGame(EventManager.Instance.msg as MessageGameEnded));
    }

    public void onCardMoved(MessageCardMoved msg)
    {
        messageCardMoved = msg;
        clip = messageCardMoved.Clip;
        SoundManager.Instance.PlaySolo(clip);
    }
    public void onCardClicked(MessageClicked msg)
    {
        messageClicked = msg;
        clip = messageClicked.Clip;
        SoundManager.Instance.PlaySolo(clip);
    }

    public void onEndGame(MessageGameEnded msg)
    {
        messageGameEnded = msg;
        clip = messageGameEnded.Clip;
        SoundManager.Instance.PlaySolo(clip);
    }

    public void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.UnSub(EventId.CardMoved, () => onCardMoved(messageCardMoved));
            EventManager.Instance.UnSub(EventId.EndGame, () => onEndGame(messageGameEnded));
        }
    }
}

public class MessageCardRemoved : Messages
{
    public MessageCardRemoved(AudioClip clip)
    {
        Clip = clip;
    }
}

public class MessageGameEnded : Messages
{ 
    public MessageGameEnded(AudioClip clip)
    {
        Clip = clip;
    }
}
public class MessageCardMoved : Messages
{
    public MessageCardMoved(AudioClip clip)
    {
        Clip = clip;
    }
}
public class MessageClicked : Messages
{
    public MessageClicked(AudioClip clip)
    {
        Clip = clip;
    }
}
