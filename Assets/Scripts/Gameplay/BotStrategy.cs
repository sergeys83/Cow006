using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Hardnest
{
    stupid = 0,
    easy = 1,
    hard = 2
}

[Serializable]
public class BotStrategy
{
    private Hardnest _diff = Hardnest.hard;
    private CardCowMover _card;
    private int _row;
    private int[] _rowsPoints = new int[4];
    private List<Transform> tempArray;
    private List<int> _rowIndex = new List<int>();
    public int Row
    {
        get => _row;
        set => _row = value;
    }

    public CardCowMover Card
    {
        get => _card;
        set => _card = value;
    }

    private List<CardCowMover> _handList = new List<CardCowMover>();

    private CardCowMover MincardPos()
    {
        CardCowMover[] temp = _handList.ToArray();
        temp = _handList.OrderBy(item => item.cv.Attack).ToArray();
        //  Debug.Log($"tempcard = {temp[0]}");
        return _handList.Find(t => t.cv.Attack == temp.Min(item => item.cv.Attack));

    }
    public BotStrategy(Hardnest difficulty, List<CardCowMover> handList, List<Transform> rowsTr)
    {
        _diff = difficulty;
        _handList = handList;
       DesicionCard(rowsTr);
      // Card =   rowsTr[1].GetComponent<TargetRowFinder>().lastCard();
    }
    public BotStrategy(Hardnest difficulty, int[] row)
    {
        _diff = difficulty;
        _rowsPoints = row;
        RawDesicion();
    }

    private List<int> RowIndex(int temp)
    {
        _rowIndex.Add(temp);
        return _rowIndex;
    }

    private void ClearRow<T>(List<T> _list)
    {
        _list.Clear();
    }
    private List<CardCowMover> MinCardList(List<Transform> rows)
    {
        List<CardCowMover> lastCards = new List<CardCowMover>();
        int[] listT = new int[4];
        for (int i = 0; i < rows.Count; i++)
        {
            listT[i] = rows[i].GetComponent<TargetRowFinder>()._list.Count;
            if (listT[i] < 5)
            {
                lastCards.Add(rows[i].GetComponent<TargetRowFinder>().lastCard());
                RowIndex(i);
            }
          //   Debug.Log($"{i}=RowIndex");
        }
        /*foreach (var item in lastCards)
        {
            Debug.Log($"{item.name}");
        }*/
        
        return lastCards;
    }
    private void CardStrategy(List<Transform> list)
    {
        int _cardIndex = 0;
        List<CardCowMover> temp = new List<CardCowMover>(MinCardList(list));
        int min = MinRowCount(list);
        Debug.Log($" Hand = {_handList.Count} , temp = {temp.Count}");
        if (_handList.Count == 0)
        {
            return;
        }
    /*    if (temp.Count == 0)
        {
            _card = MincardPos();
            return;
        }
       int lastCad = list[min].GetComponent<TargetRowFinder>().lastCard().cv.Attack;
       Debug.Log($"Attack = {lastCad} , min = {min}");
       int[] g = new int[_handList.Count];

        for (int i = 0; i < _handList.Count; i++)
        {
            g[i] = _handList[i].cv.Attack - lastCad;
            if (g[i] <= 0)
            {
                g[i] = 1000;
            }
        }
        
        int miDelta = g.Min();*/

        switch (_diff)
        {
            case Hardnest.stupid:
            case Hardnest.easy:
            case Hardnest.hard:
                _cardIndex = Random.Range(0, _handList.Count - 1);
                _card = _handList[_cardIndex];
                break;
            
           
                
                int[,] t = new int[temp.Count,_handList.Count];
                List<CardCowMover> tempDes = new List<CardCowMover>();
                int minDelta = 2;
                for (int i = 0; i < temp.Count; i++)
                {
                    for (int j = 0; j < _handList.Count; j++)
                    {
                        t[i,j] = _handList[j].cv.Attack - temp[i].cv.Attack;
                        if (t[i,j]>0)
                        {
                            tempDes.Add(_handList[j]);
                        }
                    }
                }

                if (tempDes.Count==0)
                {
                    var index = Random.Range(0, _handList.Count - 1);
                    if (index >= 0 && _handList.Count > index) _card = _handList[index];
                    break;
                }
                for (int i = 0; i < temp.Count; i++)
                {
                    for (int j = 0; j < tempDes.Count; j++)
                    {
                        if (t[i,j]<minDelta)
                        {
                            minDelta = t[i, j];
                            _cardIndex = j;
                        }
                    }
                }
                _card = tempDes[_cardIndex];
                break;

        }
    }

    private int MinRowCount(List<Transform> list)
    {
        int maxCount = 5;
        int[] dsp = new int[4];
        int[] pts = new int[4];

        tempArray = new List<Transform>(list);

        for (int i = 0; i < list.Count; i++)

        {
            dsp[i] = list[i].GetComponent<TargetRowFinder>()._list.Count;
            pts[i] = list[i].GetComponent<TargetRowFinder>().RowPoints();
            //   Debug.Log($"DSP row{i+1} = {dsp[i]}");
        }

        int minRowCount = dsp.Min();
        int minPointRow = pts.Min();
        //   Debug.Log($"Min ROW ={minRowCount} , Min Points ={minPointRow} ");
        for (var i = 0; i < dsp.Length; i++)
        {
            if (dsp[i] < maxCount)
            {
                tempArray.Add(list[i]);
                if (minPointRow.Equals(pts[i]))
                {
                    minPointRow = i;
                }
            }
        }

        if (tempArray.Count == 0)
        {
            minRowCount = minPointRow;
        }
        else
        {
            minRowCount = Random.Range(0, tempArray.Count - 1);
        }
        // Debug.Log($"minRow = {minRowCount} ");
        return minRowCount;
    }

    private void RowStrategy()
    {
        switch (_diff)
        {
            case Hardnest.stupid:
            case Hardnest.easy:
            case Hardnest.hard:

                int min = _rowsPoints.Min();
                _row = Array.IndexOf(_rowsPoints, min);
                break;
        }
    }

    private void DesicionCard(List<Transform> rows)
    {
        CardStrategy(rows);
        ClearRow(_rowIndex);
        ClearRow(tempArray);
        Card = _card;
    }

    private void RawDesicion()
    {
        RowStrategy();
        Row = _row;
    }

}
