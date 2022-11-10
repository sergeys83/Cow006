using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotsOrder : MonoBehaviour
{
    public static BotsOrder Instance;
   [SerializeField] private List<Bot> _bots = new List<Bot>();
   [SerializeField] private List<Bot> _botsInGame = new List<Bot>();
   [SerializeField] private List<BotInfo> ListPlayerUi = new List<BotInfo>();
   private int botNumber = 100;

   protected  void Awake()
   {
       Instance = this;
       GameManagerScr.S.onSetPlayers +=SetPlayerInfo;
   }
 
   private void SetPlayerInfo(Player obj)
   {
       ListPlayerUi[obj.playerNum-1].SetPlayerInfo(obj);
   }

   public void BotsInGame(Bot bot)
   {
       if (!_botsInGame.Contains(bot))
           _botsInGame.Add(bot);
   }
    public Bot GetBot(int j)
    {
        if (j != 0)
            botNumber = Random.Range(1, _bots.Count - 1);
        else
            botNumber = j;
        Bot bot = _bots[botNumber];
        BotsInGame(bot);
        return bot;
    }
    
    private void OnDisable()
    {
        GameManagerScr.S.onSetPlayers -=SetPlayerInfo;
    }
}
