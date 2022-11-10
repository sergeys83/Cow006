
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Score : MonoBehaviour
{
   public static Score Sc;
   private int curScore;
   [SerializeField] 
   private List<BotInfo> playerInfos;
    private void Awake()
    {       
        Sc = this;
    }

    public void SetScore(int player, int score)
    {
        curScore = GameManagerScr.S.players[player - 1].points;
        playerInfos[player-1].points.text = curScore.ToString();
        
        Debug.Log($"Score updated");
    }
   
}
