using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Egame
{
    public static bool _endGame { get; private set; }
    public static int rank { get; private set; }
    public static  int money { get; private set; }

    public static string InfoMessage = null;
    public static void SetMenu(bool i)
    {
        _endGame = i;
       
    }
}
