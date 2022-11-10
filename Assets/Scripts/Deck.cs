using System;
using System.Collections.Generic;
using Cards;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour
{
    public List<CardView> cards;
    public Transform deckAnchor;
    public GameObject CardPref;
    public bool stface = false;
    public CardListSc CardList;
    public CardListSo CardListSo;

    public void shuffle(ref List<CardView> oCards)
    {
        List<CardView> TCards = new List<CardView>();
        int Rd;
        TCards = new List<CardView>();
        while (oCards.Count > 0)
        {
            Rd = Random.Range(0, oCards.Count);
            TCards.Add(oCards[Rd]);
            oCards.RemoveAt(Rd);
        }
        oCards = TCards;
    }

    public void ClearDeck()
    {
        foreach (var item in cards)
        {
            Destroy(item);
        }
        cards.Clear();
    }

    public virtual void InitDeck(GameType type, int players)
    {
        MakeNewCards();

        switch (type)
        {
            case GameType.none:
            case GameType.classic:
                if (players == 3)
                {
                    Destroy(cards[cards.Count - 1].gameObject);
                    cards.RemoveAt(cards.Count - 1);
                }
                break;
            case GameType.original:
                if (players == 3)
                {
                    Destroy(cards[cards.Count - 1].gameObject);
                    cards.RemoveAt(cards.Count - 1);
                }
                break;
            case GameType.logic:
            case GameType.tactic:
                int index = players * 10 + 4;
                int count = cards.Count - index;

                for (int i = index; i < cards.Count; i++)
                {
                    Destroy(cards[i].gameObject);
                }
                cards.RemoveRange(index, count);
                break;
        }
    }

    public virtual CardView cardView(CardData cardData)
    {
        GameObject CardGO = Instantiate(cardData.CardPrefub, deckAnchor.position, Quaternion.identity);
        CardGO.transform.SetParent(deckAnchor, false);
        CardGO.transform.localPosition = Vector3.zero;
       
        CardView card = CardGO.GetComponent<CardView>();
        card.SetCardUi(cardData);
        CardGO.name = cardData.Name;
        
        return card;
    }

    public void MakeSpecialCars()
    {
        
    }
    
    public void MakeNewCards()
    {
        for (int i = 1; i < CardListSo.CardDataList.Count; i++)
        {
          cards.Add(cardView(CardListSo.CardDataList[i]));
        }
    }
  
}
