using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cards;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum TurnPhase
{
    idle,
    stage_1,
    waiting,
    post,
    gameOver,
    endRound,
    stage_2
}

public class GameManagerScr : MonoBehaviour
{
    public static GameManagerScr S;
    private TargetRowFinder _row;
    public List<CardCowMover> _tempList = new List<CardCowMover>();
    private int _tempMin, minIdx;
    public Text _Time, Round;
    private bool allmoved;
    public int BtnLine;
    public List<Transform> cardsTr = new List<Transform>();
    public Player CURRENT_PLAYER;
    public Deck deck;
    public List<CardCowMover> drawPile;
    public GameObject Finish;
    public GameObject GetLine;
    public TextAsset layotXML;
    public CowLayot layout;
    public int numberPlayers;
    public int numCards = 10;

    public event Action<Player> onSetPlayers;
    public event Action onGameEnded;
    public event Action OnStartGame;
   // public event Action onConditionChanged;
    public event Action onLineCleared;

    public Utils Utils;
    public TurnPhase phase = TurnPhase.idle;

    [SerializeField] private Transform pl3, pl4, pl1, pl2;

    [SerializeField] private Transform PlayerHand,
        AI3,
        AI2,
        AI4,
        Row_1,
        Row_2,
        Row_3,
        Row_4;

    public List<Player> players;

    public Transform Temp;
    private int TmpIndex;
    private Transform rawTransform;
    private int Turn, TurnTime, TurnLogic;
    private bool _isOpendSeetting = false;
    public bool isOpendSetting
    {
        get { return _isOpendSeetting; }
        set { _isOpendSeetting = value; }
    }
    private Settings _settings;
    private TempPoint tempPoint;
    public Player pl;
    private int playerMoves;
    private Button btnStart;
    private GameType type = GameType.classic;
    public GameTableSettings gameTable;
    public CardPlanner cardPlanner;
    bool sent = false;
    private void Awake()
    {
        S = this;
        Utils = FindObjectOfType<Utils>();
        gameTable = new GameTableSettings(PlayerPrefs.GetInt("players", 0));

        SceneChanged msg = new SceneChanged("Level_3");
        EventManager.Instance.SendEvent(EventId.Scenechanged, msg);

        Turn = TurnLogic = 1;

        cardsTr.Add(S.Row_1);
        cardsTr.Add(S.Row_2);
        cardsTr.Add(S.Row_3);
        cardsTr.Add(S.Row_4);

        BtnLine = -1;

        numberPlayers = gameTable.NumberPlayers;
        playerMoves = gameTable.NumberPlayers;
        type = gameTable.Type;

        var go = GameObject.Find("BtnStart");
        btnStart = go.GetComponent<Button>();
        btnStart.gameObject.SetActive(false);

        tempPoint = Temp.transform.GetComponent<TempPoint>();
        onGameEnded += GameOver;
        OnStartGame += () =>
        {
            btnStart.gameObject.SetActive(true);
            btnStart.onClick.AddListener(StartNewGame);
        };
    }
    private void Start()
    {
        GetLine.SetActive(false);
        Finish.SetActive(false);

        deck = GetComponent<Deck>();
        deck.InitDeck(type, numberPlayers);
        deck.shuffle(ref deck.cards);

        layout = GetComponent<CowLayot>();
        layout.ReadLayot(layotXML.text);
        layout.drawPile.pos = Temp.position;

        FillBoard();
    }

    private List<CardCowMover> UpgradeCardList(List<CardView> lCard)
    {
        var lCC = new List<CardCowMover>();
        foreach (var item in lCard) lCC.Add(item.gameObject.AddComponent<CardCowMover>());
        return lCC;
    }
    public void CardClicked(CardCowMover Card)
    {
        if (!players[0].canPlay) 
            return;
        if (isOpendSetting || Card.playerNum != 1 || phase != TurnPhase.stage_1) 
            return;
        if (phase == TurnPhase.stage_1) 
            players[0].canPlay = false; 
            players[0].moveState = Player.Moved.finished;
            Card.playerNum = players[0].playerNum;
            
        switch (Card.state)
        {
            case CardState.hand:

                MoveToTarget(Card);
                gameTable.ShowRowUi(Utils.S.MinId(Card, cardsTr, _row) < 0);
                players[0].RemoveCard(Card);
                playerMoves--;
                break;
        }
    }
    private void MakeAutMove(List<CardCowMover> Cards, int index)
    {
        if (Cards.Count == 0)
        {
            phase = TurnPhase.gameOver;
            onGameEnded?.Invoke();
            return;
        }
        players[0].moveState = Player.Moved.finished;
        index = Random.Range(0, Cards.Count - 1);
        CardCowMover Card = Cards[index];
        gameTable.ShowRowUi(Utils.S.MinId(Card, cardsTr, _row) < 0);

        switch (Card.state)
        {
            case CardState.hand:

                MoveToTarget(Card);
                players[0].RemoveCard(Card);
                playerMoves--;
                break;
        }
    }
    private void SetPlayers()
    {
        players = new List<Player>(numberPlayers);

        foreach (var item in layout.slotDefs)
        {
            if (item.player <= numberPlayers && item.type == "hand")
            {
                pl = new Player();
                pl.handSlotDef = item;
                players.Add(pl);
                pl.playerNum = item.player;
                pl.moveState = Player.Moved.finished;
                pl.canPlay = false;
                onSetPlayers?.Invoke(pl);
            }
        }

        if (numberPlayers == 2)
        {
            pl3.gameObject.SetActive(false);
            pl4.gameObject.SetActive(false);
        }

        if (numberPlayers == 3)
            pl4.gameObject.SetActive(false);

        players[0].type = PlayerType.human;
       // onSetPlayers?.Invoke();
    }
    private void FillBoard()
    {
        drawPile = UpgradeCardList(deck.cards);
        ArrangeDrawPile();
        SetPlayers();

        DrawRawCards();
        DrawPlayerCards();

        foreach (var item in players)
        {
            item.moveState = Player.Moved.waiting;
            item.canPlay = true;
        }

        phase = TurnPhase.post;
    }

    public void StartNewGame()
    {
        if (type == GameType.classic)
        {
            foreach (var item in drawPile)
            {
                Destroy(item.gameObject);
            }
            drawPile.Clear();
        }
        foreach (var item in players)
        {
            if (item.moveState == Player.Moved.idl)
                return;
        }

        StartCoroutine(Delay(1f));
        StartCoroutine(GameCycle());

        btnStart.gameObject.SetActive(false);
    }
    private async void DrawPlayerCards()
    {
        CardCowMover tCB;
        var cards = 0;
        while (cards++ < numCards)
        {
            if (drawPile.Count == 0)
            {
                phase = TurnPhase.stage_1;
                break;
            }
            for (int j = 0; j < players.Count; j++)
            {
                tCB = Draw();
                players[j].AddCard(tCB);
                if (j == 0) tCB.transform.SetParent(PlayerHand);
                if (j == 1) tCB.transform.SetParent(AI2);
                if (j == 2) tCB.transform.SetParent(AI3);
                if (j == 3) tCB.transform.SetParent(AI4);
                await WaitOneSecondAsync(0.4f);
            }
        }

        OnStartGame?.Invoke();
    }
    private async Task WaitOneSecondAsync(float time)
    {
        await Task.Delay(TimeSpan.FromSeconds(time));
    }
    public async void DrawRawCards()
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
             MoveToRaw(Draw(), item);
            await WaitOneSecondAsync(0.5f);
        }

      //  onConditionChanged?.Invoke();
    }
    public IEnumerator GameCycle()
    {
        Round.text = "Round: " + Turn;
        Debug.Log($"GAME CYCLE STARTED");
        
        while (phase != TurnPhase.gameOver)
        {
            while (phase == TurnPhase.post)
            {
                _Time.text = "STARTING ";
                yield return new WaitForSeconds(1.5f);
                phase = TurnPhase.stage_1;
            }

            while (phase == TurnPhase.stage_1)
            {
                while (gameTable.TimeOnRound-- > 0)
                {
                    _Time.text = gameTable.TimeOnRound.ToString();
                    BotTurn();
                    if (playerMoves == 0 && !GetLine.activeSelf)
                    {
                        gameTable.timeOffset = 0.2f;
                    }
                    yield return new WaitForSeconds(gameTable.timeOffset);
                }
                phase = TurnPhase.stage_2;
            }

            while (phase == TurnPhase.stage_2)
            {
                Debug.Log($" tage_2 Player state {players[0].moveState}");
                if (players[0].moveState == Player.Moved.waiting)
                {
                    players[0].moveState = Player.Moved.finished;
                    int tempCard = Random.Range(0, players[0].hand.Count);
                    MakeAutMove(players[0].hand, tempCard);

                    Debug.Log($"waiting StartGame BTNLINE = {BtnLine}");
                    
                    yield return new WaitForSeconds(0.75f);
                }
               
                _Time.text = "STAGE 2 ";
                gameTable.ResetTime();
                GetLine.SetActive(GameTableSettings.MinRowChecked);
               
                if (GetLine.activeInHierarchy)
                {
                    while (gameTable.TimeOnRound-- > 0)
                    {
                        _Time.text = gameTable.TimeOnRound.ToString();
                        if (BtnLine > 0)
                        {
                            gameTable.timeOffset = 0.1f;
                            MessageMinRawelected msgRawelected = new MessageMinRawelected(BtnLine, players[0].playerNum);
                            EventManager.Instance.SendEvent(EventId.minRawSelected, msgRawelected);
                            sent = true;
                            _Time.text = "0";
                            break;
                        }
                        yield return new WaitForSeconds(gameTable.timeOffset);
                    }
                    GetLine.SetActive(false);
                    gameTable.ShowRowUi(false);

                    if (BtnLine < 0)
                    {
                        BtnLine = Random.Range(1, 4);
                        Debug.Log($"BtnLine = {BtnLine} Player from Random GameManagerScr");
                        
                        MessageMinRawelected msg = new MessageMinRawelected( BtnLine, players[0].playerNum);
                        EventManager.Instance.SendEvent(EventId.minRawSelected, msg);
                        sent = true;
                    }
                }
                else if (!sent)
                {
                    Coroutine coroutine = StartCoroutine(cardPlanner.TargetCardsUpdate());
                       
                    while (!cardPlanner.started)
                    {
                        yield return coroutine;
                    }
                   // StartCoroutine();
                   sent = true;
                    Debug.Log($"BtnLine = {BtnLine}");
                    MessageNextFase msg = new MessageNextFase();
                    EventManager.Instance.SendEvent(EventId.BeginPhase_2, msg);
                }
                phase = TurnPhase.waiting;
                Debug.Log($"GAME CYCLE BRAKED");
                yield break;
            }

            while (phase == TurnPhase.waiting)
            {
                Debug.Log($"GAME CYCLE waiting");
                yield return null;
            }

            if (phase == TurnPhase.gameOver)
            {
                Debug.Log($"GAME OVER 1");
                onGameEnded.Invoke();
                yield break;
            }

            if (phase == TurnPhase.endRound)
            {
                if (CheckEndGame())
                {
                    Debug.Log($"GAME OVER 2");
                    phase = TurnPhase.gameOver;
                    onGameEnded.Invoke();
                    yield break;
                }
                Debug.Log($"GAME CYCLE ENDED");
                sent = false;
                playerMoves = numberPlayers;
                gameTable.ResetTime();
                BtnLine = -1;
                allmoved = false;
                yield return null;
                NewRound();
                foreach (Player player in players)
                {
                    player.moveState = Player.Moved.waiting;
                    player.canPlay = true;
                }
            }
        }
    }
    private void StartRound()
    {
        CardCowMover tCB;
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
            phase = TurnPhase.post;
        }
    }

    public IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
    }

    private bool CheckEndGame()
    {
        switch (type)
        {
            case GameType.logic:
                if (drawPile.Count == 0 && PlayerHand.childCount == 0 && Temp.transform.childCount == 0)
                {
                   // onLineCleared?.Invoke();
                    TurnLogic++;
                    if (TurnLogic < 4)
                    {
                        CardCowMover[] arrCards = FindObjectsOfType<CardCowMover>();
                        foreach (CardCowMover item in arrCards)
                        {
                            Destroy(item.gameObject);
                        }

                        deck.ClearDeck();
                        deck.InitDeck(GameType.logic, numberPlayers);
                        deck.shuffle(ref deck.cards);
                        drawPile = UpgradeCardList(deck.cards);

                        ArrangeDrawPile();
                        StartCoroutine(Delay(1f));
                        var x = 0;
                        Invoke(nameof(DrawRawCards), 0.5f);
                        StartCoroutine(Delay(1f));
                        while (x++ < numCards - 1) Invoke(nameof(DrawPlayerCards), 1f);
                    }
                    else
                    {
                        phase = TurnPhase.gameOver;
                        onGameEnded?.Invoke();
                        return true;
                    }
                }
                break;

            case GameType.original:
                if (drawPile.Count == 0 && PlayerHand.childCount == 0 && Temp.transform.childCount == 0)
                {
                    phase = TurnPhase.gameOver;
                    onGameEnded?.Invoke();
                    return true;
                }
                break;

            case GameType.classic:
                int finishPoints = 6;
                foreach (Player player in players)
                {
                    if (player.points >= finishPoints)
                    {
                        Debug.Log($"player ={player.playerNum}, points = {player.points}");
                        phase = TurnPhase.gameOver;
                        onGameEnded?.Invoke();
                        return true;
                    }
                }
                if (PlayerHand.childCount == 0 && Temp.transform.childCount == 0)
                {
                   // onLineCleared?.Invoke();

                    CardCowMover[] arrCards = FindObjectsOfType<CardCowMover>();
                    foreach (CardCowMover item in arrCards)
                    {
                        Destroy(item.gameObject);
                    }

                    deck.ClearDeck();
                    deck.InitDeck(GameType.original, numberPlayers);
                    deck.shuffle(ref deck.cards);
                    drawPile = UpgradeCardList(deck.cards);

                    ArrangeDrawPile();
                    StartCoroutine(Delay(1f));
                    var x = 0;
                    Invoke(nameof(DrawRawCards), 0.5f);
                    StartCoroutine(Delay(1f));
                    while (x++ < numCards - 1) Invoke(nameof(DrawPlayerCards), 1f);
                    StartCoroutine(Delay(3f));

                    int index = numberPlayers * 10 + 4;
                    int count = drawPile.Count - index;

                    for (int i = index; i < drawPile.Count; i++)
                    {
                        Destroy(drawPile[i].gameObject);
                    }
                    drawPile.RemoveRange(index, count);
                }
                break;
        }
        return false;
    }

    public void NewRound()
    {
        Turn += 1;
        Round.text = "Round: " + Turn;
        if (drawPile.Count > 0)
        {
            StartRound();
        }
        else
            phase = TurnPhase.post;
    }
    private void Update()
    {
        if (Input.touchCount == 2 && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Escape))) Application.Quit();
    }

    public void BotTurn(int num = -1)
    {
        foreach (var player in players)
        {
            if (player.moveState == Player.Moved.waiting)
            {
                if (player.type != PlayerType.human)
                {
                    player.TakeTurn(cardsTr);
                    playerMoves--;
                    break;
                }
            }
        }
    }

    public void MoveToTarget(CardCowMover tCB)
    {
        tCB.state = CardState.toTarget;
        tCB.MoveTo(layout.drawPile.pos);
        tCB.transform.SetParent(Temp);
    }

    public void MoveToRaw(CardCowMover tCB, Transform r, Vector3? pos = null)
    {
        Debug.Log($"Card  =  {tCB.cv.Attack}, {r} = transform name");
        Vector3? position = pos == null ? r.position : pos;
        tCB.state = CardState.toRaw;
        tCB.MoveTo((Vector3)position);
        tCB.transform.SetParent(r);
        tCB.cv.faceUp = true;
       // return tCB;
    }
    
    public void MoveToTempRaw(CardCowMover tCB, Transform r, Vector3  pos )
    {
        Vector3 position = pos;
        tCB.state = CardState.toTempRaw;
        tCB.MoveTo( position);
       // return tCB;
    }
    
    // выдача 1-ой карты  из колоды
    public CardCowMover Draw()
    {
        var cd = drawPile[0];
        drawPile.RemoveAt(0);
        return cd;
    }

    public Transform TargetPlayer(int player)
    {
        Transform t=null;
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
        CardCowMover tCB;
        for (var i = 0; i < drawPile.Count; i++)
        {
            tCB = drawPile[i];
            tCB.transform.localPosition = Vector3.zero;
            tCB.cv.faceUp = false;
            tCB.state = CardState.drawpile;
        }
    }

    public void GameOver()
    {
        Finish.SetActive(true);
        StopAllCoroutines();
        onGameEnded -= GameOver;
        OnStartGame -= () =>
        {
            btnStart.gameObject.SetActive(true);
            btnStart.onClick.AddListener(StartNewGame);
        };
    }
}
