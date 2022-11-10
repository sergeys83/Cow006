using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardPlanner : MonoBehaviour
{
    public List<TargetRowFinder> rawList = new List<TargetRowFinder>();
    public List<CardCowMover> cardsInGame = new List<CardCowMover>();
    public CardCowMover card;
    public CardCowMover currentcard;
    public TargetRowFinder targetRaw;
    public Transform target;
    private bool senToRaw = false;
    public List<int> Temp = new List<int>();
    public bool started = false;
    public Utils Utils;
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Subscription on Card Moves");
        EventManager.Instance.Sub(EventId.CardMoveToRow, () => OnCardMovedToRaw(EventManager.Instance.msg));
        EventManager.Instance.Sub(EventId.CardMoveToHand, () => OnCardMovedToHand(EventManager.Instance.msg));
        EventManager.Instance.Sub(EventId.CardMoveToTarget, () => OnCardMovedToTarget(EventManager.Instance.msg));
        EventManager.Instance.Sub(EventId.CardMoveToTempRow, () => OnCardMovedToTempRaw(EventManager.Instance.msg));
        EventManager.Instance.Sub(EventId.minRawSelected, () => ClearMinRow(EventManager.Instance.msg));
        EventManager.Instance.Sub(EventId.BeginPhase_2, () => StageTwo(EventManager.Instance.msg));
        EventManager.Instance.Sub(EventId.RawCleared, () => onRawCLeared(EventManager.Instance.msg));
    }

    private void Start()
    {
        Utils = FindObjectOfType<Utils>();
        rawList.Clear();
        foreach (var item     in GameManagerScr.S.cardsTr)
        {
            rawList.Add(item.GetComponent<TargetRowFinder>());
        }
    }

    public TargetRowFinder RawsDiffAttack()
    {
        TargetRowFinder row = null;
        Temp.Clear();
        foreach (var t in rawList)
        {
            Temp.Add(currentcard.cv.Attack - t.lastCard().cv.Attack);
        }

        int min = Utils.S.MinPositive(Temp.ToArray());
       // Debug.Log($"MIN = {min}");
        if (min > 0)
        {
            row = rawList.Find(x => (currentcard.cv.Attack - x.lastCard().cv.Attack) == min);
            targetRaw = row;
            return row;
        }
        int[] rows =
        {
            rawList[0].RowPoints(), rawList[1].RowPoints(), rawList[2].RowPoints(), rawList[3].RowPoints()
        };
        BotStrategy bs = new BotStrategy(GameManagerScr.S.players[card.playerNum - 1]._difficulty, rows);
        targetRaw = rawList[bs.Row];
        return null;
    }

    private void StageTwo(Messages instanceMsg)
    {
        if (cardsInGame.Count == 0)
        {
            senToRaw = false;
        }
        StageTwo();
    }

    private void StageTwo()
    {
        if (targetRaw && targetRaw._list.Count > 5)
        {
            senToRaw = false;
            targetRaw.CheckRow();
            MessageCardMoveToTempRaw msgr = new MessageCardMoveToTempRaw(targetRaw,targetRaw._list.Last());
            EventManager.Instance.SendEvent(EventId.CardMoveToTempRow,msgr);
            return;
        }

        if (cardsInGame.Count == 0)
        {
            GameManagerScr.S.phase = TurnPhase.endRound;
            senToRaw = false;
            started = false;
            StartCoroutine(GameManagerScr.S.GameCycle());
            return;
        }

        senToRaw = true;
        Debug.Log($"Cards in game {cardsInGame.Last()} {cardsInGame.Last().name}");
        currentcard = cardsInGame.Last();
       // currentcard = cardsInGame.Find(t => t.cv.Attack == cardsInGame.Min(item => item.cv.Attack));
        targetRaw = RawsDiffAttack();
        if (targetRaw != null)
        {
            int cardsInRaw = targetRaw.transform.childCount;
            Vector3 position = targetRaw.transform.position;
            position.x += CardView.CARD_WIDTH * cardsInRaw;
            GameManagerScr.S.MoveToRaw(currentcard, targetRaw.transform, position);
        }
        else
        {
            int BtnLine = 1;// Random.Range(1, 4);
            Debug.Log($"BtnLine random From CardPlanner = {BtnLine}");

            MessageMinRawelected msg = new MessageMinRawelected(currentcard, BtnLine, currentcard.playerNum);
            EventManager.Instance.SendEvent(EventId.minRawSelected, msg);
        }
    }

    private void onRawCLeared(Messages instanceMsg)
    {
        MessageRawCleared msg = (MessageRawCleared)instanceMsg;
        currentcard = cardsInGame.Find(c => c.playerNum == msg.player);
        TargetRowFinder target = rawList.Find(r => r.Equals(msg.RowFinder));
        GameManagerScr.S.MoveToRaw(currentcard, target.transform, target.transform.position);
        currentcard.transform.SetParent(target.transform);
        cardsInGame.Remove(currentcard);
        senToRaw = true;
    }

    private void ClearMinRow(Messages instanceMsg)
    {
        MessageMinRawelected msg = (MessageMinRawelected)instanceMsg;
        Debug.Log("Target raw = " + msg.rawIndex + " Target player " + msg.player);
        targetRaw = rawList[msg.rawIndex];
        if (GameManagerScr.S.players[msg.player-1].canPlayExtra)
        {
            ShowExtraCardsUi();
        }
        else
            rawList[msg.rawIndex].CardRowRemover(msg.player);
    }
    private void ShowExtraCardsUi()
    {
        Debug.Log("Extra cards shown menu");
    }

    private void OnCardMovedToRaw(Messages instanceMsg)
    {
         Messages msg = (MessageCardMoveToRaw)instanceMsg;
        currentcard = msg.Card;
        msg.Target.GetComponent<TargetRowFinder>().AddCd(currentcard);
       // Debug.Log($"Card " + currentcard.name);
        cardsInGame.Remove(currentcard);
        currentcard = null;
        if (senToRaw)
        {
            StageTwo();
        }
    }

    private void OnCardMovedToTempRaw(Messages instanceMsg)
    {
        var msg = (MessageCardMoveToTempRaw)instanceMsg;
        currentcard = msg.Card;
        target = msg._targetRowFinder.transform;
       GameManagerScr.S.MoveToTempRaw(currentcard, target, target.position);
       // currentcard.transform.DOMove(target.position, 0.3f, false);
        Debug.Log($"CardPlanner Six Card on 1 place" + currentcard.name);
        currentcard = null;

        if (cardsInGame.Count > 1)
          StageTwo();
        else
        {
            GameManagerScr.S.phase = TurnPhase.endRound;
            senToRaw = false;
        }
    }

    private void OnCardMovedToHand(Messages instanceMsg)
    {
        Messages msg = (MessageCardMoveToHand)instanceMsg;
        card = msg.Card;
    }

    private void OnCardMovedToTarget(Messages instanceMsg)
    {
        Messages msg = (MessageCardMoveToTarget)instanceMsg;
        card = msg.Card;
        cardsInGame.Add(card);
    }

    private void Sorting(ref List<CardCowMover> tlist)
    {
        var _cardsT = new List<CardCowMover>(tlist.OrderByDescending(item => item.cv.Attack)).ToArray();
        var list = new List<CardCowMover>(_cardsT);
        tlist = list;
    }

    public IEnumerator TargetCardsUpdate()
    {
        if ((cardsInGame.Count == GameManagerScr.S.numberPlayers && GameManagerScr.S.phase == TurnPhase.stage_2))
        {
            Debug.Log("Started");
            started = false;
            Sorting(ref cardsInGame);
            TurnCards();
            yield return new WaitForSeconds(0.5f);
            started = true;
        }

    }

    public void TurnCards()
    {
        for (int i = 0; i < cardsInGame.Count; i++)
            cardsInGame[i].OpenAndMove(i, GameManagerScr.S.numberPlayers);
    }
    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.UnSub(EventId.CardMoveToRow, () => OnCardMovedToRaw(EventManager.Instance.msg));
            EventManager.Instance.UnSub(EventId.CardMoveToHand, () => OnCardMovedToHand(EventManager.Instance.msg));
            EventManager.Instance.UnSub(EventId.CardMoveToTarget, () => OnCardMovedToTarget(EventManager.Instance.msg));
            EventManager.Instance.UnSub(EventId.CardMoveToTempRow, () => OnCardMovedToTempRaw(EventManager.Instance.msg));
            EventManager.Instance.UnSub(EventId.minRawSelected, () => ClearMinRow(EventManager.Instance.msg));
            EventManager.Instance.UnSub(EventId.BeginPhase_2, () => StageTwo(EventManager.Instance.msg));
            EventManager.Instance.UnSub(EventId.RawCleared, () => onRawCLeared(EventManager.Instance.msg));
        }

    }
}
