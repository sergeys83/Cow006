using System.Collections.Generic;
using System.Linq;
using Cards;
using Photon.Pun;
using UnityEngine;

public class PlayerExt : Player
{
    public new List<CardNet> hand;
    public void AddCard(CardNet eCard)
    {
        if (hand == null)
            hand = new List<CardNet>();
        hand.Add(eCard);
        eCard.playerNum = handSlotDef.player;
        FanHand();
    }

    public override void Sorting()
    {
        CardNet[] _cardsT = new List<CardNet>(hand.OrderBy(item => item.Attack)).ToArray();
        hand.Clear();
        hand = new List<CardNet>(_cardsT);
    }

    public override void FanHand()
    {
        float startRot = 0;
        startRot = handSlotDef.rot;
        if (hand.Count > 1)
            Sorting();
        Vector3 pos=default;

        for (int i = 0; i < hand.Count; i++)
        {
            if (playerNum == 1 )
            {
                pos = Vector3.one * CardView.CARD_HEIGHT / 2f;
                pos += handSlotDef.pos;
                pos.z = -0.1f * i;
                pos.x += CardView.CARD_WIDTH * i + 10;
                pos.y -= 40f;
            }

            if (playerNum == 3 || playerNum == 4|| playerNum == 2)
                pos = Vector3.zero;

            hand[i].state = CardState.toHand;
            hand[i].MoveTo(pos);
            
            if (PhotonNetwork.LocalPlayer.IsLocal)
                hand[i].faceUp = true;
            else
                hand[i].faceUp = false;
        }
    }
    public void RemoveCard(CardNet eCd)
    {
        if (hand != null && hand.Contains(eCd))
            hand.Remove(eCd);
    }
}
