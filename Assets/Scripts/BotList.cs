using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BotList", menuName = "Bots/List")]
public class BotList : ScriptableObject
{
    public List<Bot> Bots =new List<Bot>();
}
