using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cards;

public class CardSorter : MonoBehaviour
{
    public List<CardCowMover> hand = new List<CardCowMover>();

    private void OnTransformChildrenChanged()
    {
        if (hand != null)
        {
            hand.Clear();
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            hand.Add(transform.GetChild(i).GetComponent<CardCowMover>());
        }

        if (hand.Count > 1)
        {
            Sorting();
            PlaceSortedCards(hand);
        }
    }

    private void Sorting()
    {
        var _cardsT = new List<CardCowMover>(hand.OrderBy(item => item.cv.Attack)).ToArray();
        hand.Clear();
        hand = new List<CardCowMover>(_cardsT);
    }

    private void PlaceSortedCards(List<CardCowMover> hand)
    {
        Vector3 pos = Vector3.zero; 
        for (int i = 0; i < hand.Count; i++)
        {
            pos = Vector3.one;
            pos.z = 0f * i;
            pos.x += CardView.CARD_WIDTH * i;

            hand[i].state = CardState.toHand;
            hand[i].MoveTo(pos);
        }
    }
}
