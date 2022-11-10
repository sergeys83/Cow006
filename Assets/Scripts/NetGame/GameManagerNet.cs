using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManagerNet : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks //,IOnEventCallback
{
    public enum TurnPhase
    {
        idle,
        pre,
        waiting,
        post,
        gameOver,
        endRound
    }

    public static GameManagerNet S;
    public GameObject root;
    private RawViewer _row;
    public List<CardNet> _tempList = new List<CardNet>();
    private int _tempMin, minIdx;
    public Text _Time, Round;
    private bool allmoved;
    public Transform Board;
    public int BtnLine;
    public List<Transform> cardsTr = new List<Transform>();
    public PlayerExt CURRENT_PLAYER;
    public DeckNet deck;
    public List<CardNet> drawPile;
    public GameObject Finish;
    public GameObject GetLine;
    public float handsFanDegrees;
    public TextAsset layotXML;
    public CowLayot layout;
    private Transform layoutAnchor;
    public int numberPlayers;
    public int numCards = 10;

    //  public Action onSetPlayers ;
    public Action onGameEnded;
    public Action onConditionChanged;
    public Action onLineCleared;

    public TurnPhase phase = TurnPhase.idle;
    [SerializeField] private Transform pl3, pl4, pl1, pl2;
   public Transform PlayerHand,
        AI3,
        AI2,
        AI4,
        Row_1,
        Row_2,
        Row_3,
        Row_4;

    public List<PlayerExt> players;
    public Transform Temp;
    private int TmpIndex;
    private Transform rawTransform;
    //private int Minx;
    private int Turn, TurnTime, TurnLogic;

    private bool _isOpendSeetting = false;
    public bool isOpendSetting
    {
        get { return _isOpendSeetting; }
        set { _isOpendSeetting = value; }
    }
    private Settings _settings;
    private TempNet tempPoint;
    public PlayerExt pl;
    private int playerMoves;
    private Button btnStart;
    public GameType type = GameType.classic;
    public List<CardView> temp = new List<CardView>();
    public PunTurnManager turnManager;

    private void Awake()
    {
        S = this;
        turnManager = GetComponent<PunTurnManager>();
        turnManager.TurnManagerListener = this;

        Turn = TurnLogic = 1;

        BtnLine = -1;
        numberPlayers = 4;
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ReconnectAndRejoin();
            numberPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        }
        else
        {
            numberPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        }
        Debug.Log(numberPlayers + "= PLAYERS");
        playerMoves = numberPlayers;

        tempPoint = Temp.transform.GetComponent<TempNet>();
        onGameEnded += GameOver;

        Round.text = "Round: " + Turn;
    }

    private void SetGameType()
    {
        type = GameType.original;
        Debug.Log($"{type} = Current game type");
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            if (turnManager.IsOver)
            {
                return;
            }

            if (Round != null)
            {
                Round.text = "Round: " + turnManager.Turn;
            }

            if (turnManager.Turn > 0 && _Time != null && phase == TurnPhase.pre)
            {
                _Time.text = turnManager.RemainingSecondsInTurn.ToString("F1") + " SECONDS";

                /*  TimerFillImage.anchorMax =
                       new Vector2(1f - this.turnManager.RemainingSecondsInTurn / this.turnManager.TurnDuration, 1f);*/
            }
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    private void Start()
    {
        layout = GetComponent<CowLayot>();
        layout.ReadLayot(layotXML.text);
        layout.drawPile.pos = Temp.position;

        GetLine.SetActive(false);
        Finish.SetActive(false);

        foreach (var item in PhotonNetwork.PlayerList)
        {
            Transform hand;
            PlayerManager.Instance.CreatePlayer(item, out hand);
            if (PhotonNetwork.IsMasterClient)
                HintBox.Instance.Show("I'm Master", "");
            else
                HintBox.Instance.Show("I'm Slave", "");
        }
        SetPlayers(numberPlayers);

        SetGameType();

        FillBoard();
        turnManager.TurnDuration = 25f;
        StartCoroutine(Delay(1f));
        StartTurn();
    }
    public void StartTurn()
    {
        StopAllCoroutines();
        if (CheckEndGame())
        {
            phase = TurnPhase.gameOver;
            onGameEnded?.Invoke();
            return;
        }
        if (PhotonNetwork.IsMasterClient /*&& turnManager.Turn<=1 */ && phase == TurnPhase.waiting)
        {
            Turn = turnManager.Turn;
            Round.text = "Round: " + Turn;
            turnManager.BeginTurn();
            phase = TurnPhase.pre;
        }
    }

    [PunRPC]
    public void CardClicked(CardNet Card)
    {
        if (isOpendSetting) return;
        if (phase != TurnPhase.pre) return;
        if (turnManager.IsFinishedByMe) return;
        switch (Card.state)
        {
            case CardState.hand:
                CURRENT_PLAYER = players.Find(p => p.playerNum == Card.playerNum);
                PhotonView PV = Card.gameObject.GetPhotonView();
                CURRENT_PLAYER.RemoveCard(Card);
                CURRENT_PLAYER = null;
                PV.RPC("MoveToTarget", RpcTarget.All);
                int btn = Utils.MinIdNet(Card, cardsTr, _row);
                break;
        }
    }
    private void SetPlayers(int pls)
    {
        players = new List<PlayerExt>(pls);
        Debug.Log(PhotonNetwork.NickName + "Photon SERVER from players " + PhotonNetwork.CurrentRoom.Players.Count);

        foreach (var item in layout.slotDefs)
        {
            if (item.player <= numberPlayers && item.type == "hand")
            {
                pl = new PlayerExt();
                pl.handSlotDef = item;
                players.Add(pl);
                Debug.Log(players.Count);
                pl.playerNum = item.player;
                pl.type = PlayerType.human;
                pl.moveState = PlayerExt.Moved.finished;
            }
        }

        if (numberPlayers == 2)
        {
            pl3.gameObject.SetActive(false);
            pl4.gameObject.SetActive(false);
        }

        if (numberPlayers == 3)
        {
            pl4.gameObject.SetActive(false);
        }
    }
    private void FillBoard()
    {
        deck = GetComponent<DeckNet>();

        if (PhotonNetwork.IsMasterClient)
        {
            deck.InitDeck(type, numberPlayers);
            deck.shuffle(ref deck.cards);
        }

        /////CARD TO NetCArd
        drawPile = Utils.UpgradeCardList(deck.cards);
        for (int i = 0; i < deck.cards.Count; i++)
        {
            PhotonView PV = deck.cards[i].gameObject.GetPhotonView();
            PV.RPC("MakeRemoteDeck", RpcTarget.Others, drawPile[i].Attack, drawPile[i].Points, drawPile[i].name);
        }
        ArrangeDrawPile();

        var x = 0;
        Invoke(nameof(DrawCardsToRow), 0.5f);
        while (x++ < numCards) Invoke(nameof(DrawCardsToHand), 1f);

        foreach (var item in players)
        {
            item.moveState = PlayerExt.Moved.waiting;
        }
        phase = TurnPhase.waiting;
    }
    private void DrawCardsToHand()
    {
        CardNet tCB;
        var j = -1;

        while (j++ < numberPlayers - 1)
        {
            if (drawPile.Count == 0)
            {
                phase = TurnPhase.waiting;
                break;
            }

            tCB = Draw();

            players[j].AddCard(tCB);
            if (j == 0) tCB.transform.SetParent(PlayerHand);
            if (j == 1) tCB.transform.SetParent(AI2);
            if (j == 2) tCB.transform.SetParent(AI3);
            if (j == 3) tCB.transform.SetParent(AI4);
        }
    }

    private void SwapHAnds(Transform t1, Transform t2)
    {
       
    }
    public void DrawCardsToRow()
    {
        var Rows = new List<Transform>
        {
            Row_1,
            Row_2,
            Row_3,
            Row_4
        };

        foreach (var item in Rows)
        {
            CardNet card = Draw();
            if (PhotonNetwork.IsMasterClient)
            {
                card.MoveToRaw(item.name);
                PhotonView PV = card.gameObject.GetPhotonView();
                PV.RPC("MoveToRaw", RpcTarget.Others, item.name);
            }

            item.GetComponent<RawViewer>().AddCd(card);
            card.faceUp = true;
        }
        onConditionChanged?.Invoke();
    }
    private void StartRound()
    {
        CardNet tCB;
        var j = -1;
        if (drawPile.Count != 0 && Temp.transform.childCount == 0)
        {
            while (j++ < numberPlayers - 1)
            {
                if (drawPile.Count == 0)
                {
                    break;
                }
                tCB = Draw();
                players[j].AddCard(tCB);
                if (j == 0) tCB.transform.SetParent(PlayerHand);
                if (j == 1) tCB.transform.SetParent(AI2);
                if (j == 2) tCB.transform.SetParent(AI3);
                if (j == 3) tCB.transform.SetParent(AI4);
            }
        }
    }
    public IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
    }

    private bool CheckEndGame()
    {
        if (drawPile.Count == 0 && PlayerHand.childCount == 0 && Temp.transform.childCount == 0)
        {
            phase = TurnPhase.gameOver;
            onGameEnded?.Invoke();
            return true;
        }
        return false;
    }

    public void Sorting()
    {
        var _cardsT = new List<CardNet>(_tempList).ToArray();
        _cardsT = _tempList.OrderBy(item => item.Attack).ToArray();
        _tempList = new List<CardNet>(_cardsT);
    }

    public void StartEngine()
    {
        if (_tempList.Count > 0)
            _tempList.Clear();
        else
            _tempList = new List<CardNet>();

        phase = TurnPhase.post;

        while (tempPoint._list.Count > 0)
        {
            _tempList.Add(tempPoint._list[0]);
            tempPoint._list.RemoveAt(0);
        }

        Sorting();
        allmoved = true;
    }

    public IEnumerator ChoseRow(CardNet card)
    {
        PhotonView PV = card.gameObject.GetPhotonView();
        RawViewer rawDps = null;
        var position = new Vector3();
        int Atak = card.Attack;

        var g = new int[4];
        for (var i = 0; i < cardsTr.Count; i++)
        {
            _row = cardsTr[i].GetComponent<RawViewer>();
            g[i] = Atak - _row.lastCard().Attack;
        }
        int min;

        min = Utils.S.MinPositive(g);
        for (var i = 0; i < g.Length; i++)
            if (g[i].Equals(min))
            {
                rawTransform = cardsTr[i];
                rawDps = rawTransform.GetComponent<RawViewer>();
                break;
            }

        //Первые минимальные карты выше рангом, чем на столе
        if (min > 0)
        {
            int cardsInRaw = rawTransform.childCount;
            position = rawTransform.position;
            position.z = 0;
            position.x += cardsInRaw * CardNet.CARD_WIDTH;
            //в ряду еще меньше 5 карт
            if (cardsInRaw < 5)
            {
                rawDps.AddCd(card);
                if (card != null)
                    PV.RPC("MoveToRawX", RpcTarget.All, rawTransform.name, position);
            }

            if (cardsInRaw >= 5)
            {
              //  players[card.playerNum - 1].points += rawDps.RowPoints();
                rawDps.CardRowRemover(card.playerNum);
                position = rawTransform.position;
                if (card != null)
                    PV.RPC("MoveToRawX", RpcTarget.All, rawTransform.name, position);
                //    Score.Sc.Up(card.playerNum,players[card.playerNum - 1].points);
                rawDps.AddCd(card);
            }
            _tempList.RemoveAt(0);
            yield return new WaitForSeconds(0.51f);
        }

        if (min < 0)
        {
            /////////////////////////////////////////////////////
            BtnLine = 4;
            rawTransform = cardsTr[BtnLine - 1];
            ///////////////////////////////////////////////////
            IEnumerator ClearRow()
            {
                rawDps = rawTransform.GetComponent<RawViewer>();
             //   players[card.playerNum - 1].points += rawDps.RowPoints();
                rawDps.CardRowRemover(card.playerNum);
                yield return null;
            }

            yield return ClearRow();
            //  Score.Sc.Up(card.playerNum,players[card.playerNum - 1].points);
            rawDps.AddCd(card);
            if (card != null)
                PV.RPC("MoveToRawX", RpcTarget.All, rawTransform.name, rawTransform.position);
            _tempList.RemoveAt(0);
            yield return new WaitForSeconds(0.51f);
        }
    }

    // выдача 1-ой карты  из колоды
    [PunRPC]
    public CardNet Draw()
    {
        var cd = drawPile[0];
        drawPile.RemoveAt(0);
        return cd;
    }
    public Transform TargetPlayer(int player)
    {
        Transform t = pl1;
        switch (player)
        {
            case 1:
                t = pl1;
                break;
            case 2:
                t = pl2;
                break;
            case 3:
                t = pl3;
                break;
            case 4:
                t = pl4;
                break;
        }

        return t;
    }
    public void ArrangeDrawPile()
    {
        for (var i = 0; i < drawPile.Count; i++)
        {
            drawPile[i].transform.localPosition = new Vector3(1, 1, 1);
            drawPile[i].faceUp = false;
            drawPile[i].state = CardState.drawpile;
        }
    }
    public void GameOver()
    {
        Finish.SetActive(true);
        StopAllCoroutines();
        onGameEnded -= GameOver;
    }

    #region TurnManagerCallBacks
    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins() turn: " + turn);
        foreach (var p in players)
        {
            p.moveState = Player.Moved.idl;
        }
        phase = TurnPhase.pre;
    }

    public void OnTurnCompleted(int obj)
    {
        Debug.Log("OnTurnCompleted: " + obj);
        StartCoroutine(Delay(1f));
        tempPoint.OnGetChildren();
        HintBox.Instance.Show("Child = " + tempPoint._list.Count, "");
        /* if (tempPoint._list.Count<numberPlayers)
         {
             return;
         }*/
        tempPoint.TurnCards();
        _Time.text = "moving... ";
        phase = TurnPhase.post;
        StartEngine();
        StartCoroutine(Delay(1f));
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(Row());
        }
    }

    public IEnumerator Row()
    {
        while (_tempList.Count > 0)
        {
            yield return StartCoroutine(ChoseRow(_tempList[0]));
            yield return null;
        }
        phase = TurnPhase.waiting;
        StartCoroutine(Delay(1f));
        StartTurn();
    }

    public void OnPlayerMove(Photon.Realtime.Player player, int turn, object move)
    {
        //  throw new NotImplementedException();
    }

    public void OnPlayerFinished(Photon.Realtime.Player player, int turn, object move)
    {
        Debug.Log("OnTurnFinished: " + player + " turn: " + turn + " action: " + move);
    }

    public void OnTurnTimeEnds(int turn)
    {
        OnTurnCompleted(-1);
    }
    #endregion

}
