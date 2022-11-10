
using System;
using UnityEngine;

public enum GameType
{
    none=0,
    classic=1,
    logic=2,
    original=3,
    tactic=4
}
public class GameTableSettings
{
    public GameType Type { get; private set; }
    public int NumberPlayers { get;  }
    public static bool MinRowChecked => _minRowChecked;
    private static bool _minRowChecked;
    
    public int TimeOnRound
    {
        get => _timeOnRound;
        set => _timeOnRound = value;
    }
    public float TimeOnStage_2
    {
        get =>_timeOnStage_2;
        set => _timeOnStage_2 = value;
    }

    public float timeOffset = 1f;
    private int _timeOnRound = 19;
    private float _timeOnStage_2 = 10f;

    public GameTableSettings(int getInt)
    {
        if (PlayerPrefs.HasKey("players")&& getInt!=0)
        {
            NumberPlayers = getInt;
        }
        else
        {
            PlayerPrefs.SetInt("players",4);
            NumberPlayers = 4;
        }

        _minRowChecked = false;
        SetGameType();
    }
    private void SetGameType()
    {
        Type = (GameType)PlayerPrefs.GetInt("GameType", 1);
        Debug.Log($"{Type} = Current game type");
    }
    public void ShowRowUi(bool showRow)
    {
        _minRowChecked = showRow;
    }
    
    public void ResetTime()
    {
        _timeOnRound = 10;
        timeOffset = 1f;
    }
}
