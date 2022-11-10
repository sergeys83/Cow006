
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TempPoint : MonoBehaviour
{
    bool isTrigger = false;
    public List<CardCowMover> _list;
    int players;
  
    private void Awake()
    {
        players = PlayerPrefs.GetInt("players");
    }
    void Update()
    {
        if (isTrigger && _list.Count == players)
        {
            isTrigger = false;
           // GameManagerScr.S.phase = TurnPhase.endRound;
        }
    }
    public void OnTransformChildrenChanged()
    {
        int i = transform.childCount;

        if (i == players)
        {
            for (int j = 0; j < i; j++)
            { 
                _list.Add(transform.GetChild(j).GetComponent<CardCowMover>());
            }
            isTrigger = true;
        }
    }
  /*  private void Sorting(ref List<CardCowMover> tlist)
    {
        var _cardsT = new List<CardCowMover>(tlist.OrderByDescending(item=>item.cv.Attack)).ToArray();
        var list = new List<CardCowMover>(_cardsT);
        tlist = list;
    }
    
    public void TurnCards()
    {
        Sorting(ref _list);
        Vector3 pos = transform.localPosition;
        for (int i = 0; i < _list.Count; i++)
            _list[i].OpenAndMove(i, players );
    }*/
}


  

  
    
