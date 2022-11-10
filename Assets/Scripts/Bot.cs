
using UnityEngine;
   
   [CreateAssetMenu(fileName = "New Bot",menuName = "Bots")]
    public class Bot : ScriptableObject
    {
    public Sprite avatar ;
    public new string name;
    public int points;
    public int energy;
    public Hardnest diff;
   
    }
