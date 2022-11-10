using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.DataStorage;
using Scripts.Profile;

public class BotInfo : MonoBehaviour
{
    public Bot bot = null;
    public Dictionary<int, BotInfo> plBot;
    public new Text name;
    public Text points;
    public Text energy;
    public Image avatarka;
    private string trName;
    public int plNum;

    // Start is called before the first frame update
    /*private void Awake()
    {
        GameManagerScr.S.onSetPlayers +=SetPlayerInfo;
    }*/
    public void SetUI(Player _player)
    {
        if (plNum == _player.playerNum)
        {
            name.text =  _player.name;
            avatarka.sprite = _player.Avatar;
            points.text = _player.points.ToString();
            energy.text = bot.energy.ToString();
            energy.text = null;
        }
    }

    public void SetPlayerInfo(Player _player)
    {
        bot = BotsOrder.Instance.GetBot(plNum-1);
       
        if (_player.playerNum == 1 && _player.playerNum == plNum)
        {
            _player.name = DataSaver.Instance.playerData.playerName;
            _player.Avatar = AvatarManager.Instance.LoadAvatar(DataSaver.Instance.playerData.playerAvatar);
            _player.energy = bot.energy;
            _player.points = bot.points;
        }
        else if (_player.playerNum != 1 && _player.playerNum == plNum)
        {
            _player.name = bot.name;
            _player.Avatar = bot.avatar;
            _player._difficulty = bot.diff;
            _player.energy = bot.energy;
            _player.points = bot.points;  
        }
        
        SetUI(_player);
    }

    /*private void OnDisable()
    {
        GameManagerScr.S.onSetPlayers -=SetPlayerInfo;
    }*/
}
