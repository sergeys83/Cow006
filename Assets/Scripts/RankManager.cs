using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Scripts.Gameplay.UI;


public class RankManager : MonoBehaviour
{
    private const int WINNER_MONEY = 27;
    private const int LOOSER_MONEY = 15;
    private const int LOSE_MONEY = 0;
    private const int WINNER_POINTS = 20;
    private const int LOOSER_POINTS = -5;
    private const int LOOS_POINTS = -10;

    [SerializeField] private SummaryMenu _summaryMenu;
    private List<Player> _tempPl = new List<Player>();
    private int rank = 0;

    // Start is called before the first frame update
    private void Start()
    {
        GameManagerScr.S.onGameEnded += ShowFinish;
    }
    public void Sorting()
    {
        Player[] _pls = new List<Player>(GameManagerScr.S.players).ToArray();
        _pls = GameManagerScr.S.players.OrderBy(item => item.points).ToArray();
        _tempPl = new List<Player>(_pls);
    }
    private void OnDisable()
    {
        GameManagerScr.S.onGameEnded -= ShowFinish;
    }
    private void ShowFinish()
    {
        if (GameManagerScr.S.phase == TurnPhase.gameOver)
        {
            int i = 1;
            Sorting();
            foreach (var pl in _tempPl)
            {
                if (pl.playerNum == 1)
                {
                    rank = i;
                }
                i++;
            }
            _summaryMenu.Show();
            switch (rank)
            {
                case 1:
                    _summaryMenu.SetResult(true, WINNER_MONEY, WINNER_POINTS);
                    break;
                case 2:
                case 3:
                    _summaryMenu.SetResult(false, LOOSER_MONEY, LOOSER_POINTS);
                    break;
                case 4:
                    _summaryMenu.SetResult(false, LOSE_MONEY, LOOS_POINTS);
                    break;
            }
        }
    }
}
