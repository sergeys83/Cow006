using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class TargetRowFinder : MonoBehaviour
{
    public AudioClip CrunchClip;
    public List<CardCowMover> _list = new List<CardCowMover>();
    public List<CardCowMover> ListToDelete = new List<CardCowMover>();
  //  public int Count;
    public bool blocked;
    public CardCowMover card;
    
   /* public void OnChildrenChanged()
    {
         Count = _list.Count;
    }*/
    private void Start()
    {
        _list.Clear();
        ListToDelete.Clear();
            //GameManagerScr.S.onConditionChanged+=OnChildrenChanged;
            //GameManagerScr.S.onLineCleared += LineDestroyed;
    }

    private void LineDestroyed()
    {
        _list.Clear();
    }

    private void OnDestroy()
    {
        //   GameManagerScr.S.onConditionChanged -=OnChildrenChanged;
        _list.Clear();
        ListToDelete.Clear();
        //GameManagerScr.S.onLineCleared -= LineDestroyed;
    }

    public void AddCd(CardCowMover card)
    {
        _list.Add(card);
        _list = _list.Distinct().ToList();
    }    
    
    public CardCowMover lastCard()
    { 
        Debug.Log($"name = {transform.name}, {_list.Count} = _list.Last().name");
        return _list[_list.Count-1];
    }

    public void RemoveCards()
    {
        if (_list.Count>5)
        {
            ListToDelete.AddRange(_list.GetRange(0,_list.Count-1));
            _list.RemoveRange(0,_list.Count-1);
            return;
        }
        ListToDelete.AddRange(_list);
        _list.Clear();
    }
    public List<CardCowMover> RowList()
    {
        return _list;
    }

    public void CheckRow()
    {
        int player =_list.Last().playerNum;
        CardCowMover cardLast = _list.Last();
        foreach (var item in _list)
        {
            if (item.cv.Attack != cardLast.cv.Attack)
            {
                Vector3 pos;
                item.playerNum = player;
                CardCowMover card = MoveToGrave(item, GameManagerScr.S.TargetPlayer(player), out pos );
                card.MoveTo(pos);
                card.transform.SetParent(GameManagerScr.S.TargetPlayer(player)); 
            }
           else
            {
              /*  item.onPlace = false;
                item.state = CardState.toTempRaw;
              item.MoveTo(transform.position);
               GameManagerScr.S.MoveToRaw(item, this.transform,Vector3.zero);*/
              item.transform.localPosition = Vector3.zero;
              Debug.Log($" SIX Card ={item.name}, player =  {item.playerNum}"); 
            }
        }
      //  MessageCardMoveToTempRaw msgr = new MessageCardMoveToTempRaw(this,cardLast);
      //  EventManager.Instance.SendEvent(EventId.CardMoveToTempRow,msgr);
        _list.RemoveRange(0, 5);
          MessageNextFase msgr = new MessageNextFase();
         EventManager.Instance.SendEvent(EventId.BeginPhase_2,msgr);
        
        MessageCardMoved msg = new MessageCardMoved(CrunchClip);
        EventManager.Instance.SendEvent(EventId.CardMoved,msg);
    }
    
    public int RowPoints()
    {
        int Point = 0;
        int i = 1;
        List<CardCowMover> tmpList=new List<CardCowMover>(_list);
        while (tmpList.Count>0)
        {
            Point += tmpList[0].cv.Points;
            tmpList.RemoveAt(0);
            i++;
        }
        return Point;
    }
  
    public virtual void CardRowRemover(int player)
    {
        RemoveCards();

        foreach (var item in ListToDelete)
        {
            Vector3 pos;
            CardCowMover card =   MoveToGrave(item, GameManagerScr.S.TargetPlayer(player), out pos );
            card.playerNum = player;
            Debug.Log($"Card ={card.name}, {card.playerNum}");
            card.MoveTo(pos);
            card.transform.SetParent(GameManagerScr.S.TargetPlayer(player));
        }
        ListToDelete.Clear();
        
        MessageRawCleared msgr = new MessageRawCleared(this,player,card);
        EventManager.Instance.SendEvent(EventId.RawCleared,msgr);
        
        MessageCardMoved msg = new MessageCardMoved(CrunchClip);
        EventManager.Instance.SendEvent(EventId.CardMoved,msg);
    }
    
    public CardCowMover MoveToGrave(CardCowMover tCB, Transform player, out Vector3 position)
    {
        tCB.onPlace = false;
        position =player.position;
        tCB.state = CardState.toGraveyard;
         return tCB;
    }
}
