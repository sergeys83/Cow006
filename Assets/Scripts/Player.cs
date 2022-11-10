using System;
using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

public enum PlayerType
{
    human,
    ai
}

[Serializable]
public class Player
{
    public enum Moved
    {
        finished,
        waiting,
        idl,
    }

    public Moved moveState = Moved.idl;
    public PlayerType type = PlayerType.ai;
    public int playerNum;
    public SlotDef handSlotDef;
    public List<CardCowMover> hand;
    public Hardnest _difficulty;
    public Sprite Avatar;
    public int points
    {
        get {
            if (plStack.Count>0)
                return plStack.Sum(t=>t.cv.Points);
            else
                return 0;
        }
        set
        {
        if (plStack.Count>0)
            plStack.Sum(t=>t.cv.Points);
        else
            value = 0;
    } 
    }
    public string name;
    public int energy;
    private static CardCowMover _card;
    public bool canPlay = false;

    public BotStrategy _botStrategy;
    public List<CardCowMover> ExtraCards = new List<CardCowMover>();
    public List<CardCowMover> plStack = new List<CardCowMover>();
    public List<CardCowMover> temp;
    public bool canPlayExtra => ExtraCards.Count > 0;
    [ContextMenu("Players")]
    //Добавляет карту в руку игрока
    public CardCowMover AddCard(CardCowMover eCard)
    {
        if (hand == null)
            hand = new List<CardCowMover>();
        hand.Add(eCard);
        eCard.playerNum = handSlotDef.player;
        FanHand();
        return eCard;

    }
    public List<CardCowMover> AddStack(List<CardCowMover> row)
    {
        if (plStack == null)
            plStack = new List<CardCowMover>();
        plStack.AddRange(row);
        return plStack;
    }
    

    public void CBCallback(CardCowMover card)
    {
        Utils.tr("Player.CBCallback()", card.name, "Player ", playerNum);
        card.playerNum = playerNum;
    }
    //Расчет поворота и позиции карты в руке
    public virtual void FanHand()
    {
       
        if (hand.Count > 1)
            Sorting();
        Vector3 pos;

        for (int i = 0; i < hand.Count; i++)
        {
            if (type == PlayerType.human)
            {
                pos = Vector3.one;
                pos += handSlotDef.pos;
                pos.z = 0f;
                pos.x += CardView.CARD_WIDTH * i;
            }
            else
            {
                pos = Vector3.zero;
            }

            hand[i].state = CardState.toHand;
            hand[i].MoveTo(pos);
            hand[i].cv.faceUp = type == PlayerType.human;
        }
    }

    public virtual void Sorting()
    {
        var _cardsT = new List<CardCowMover>(hand.OrderBy(item => item.cv.Attack)).ToArray();
        hand.Clear();
        hand = new List<CardCowMover>(_cardsT);
    }

    // Удаляет карту из руки
    public void RemoveCard(CardCowMover eCd)
    {
        if (hand != null && hand.Contains(eCd))
        {
            hand.Remove(eCd);
        }
    }

    public void TakeTurn(List<Transform> rows = null)
    {
        if (moveState == Moved.finished)
            return;

        _botStrategy = new BotStrategy(_difficulty, hand, rows);
        CardCowMover card = _botStrategy.Card;
        Debug.Log($"Card = {card.name}");
       // card.callbackPlayer = this;
        card.playerNum = playerNum;

       // Utils.tr("Bot.TakeTurn");
        GameManagerScr.S.MoveToTarget(card);
        moveState = Moved.finished;
        RemoveCard(card);
    }
}
