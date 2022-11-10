
    using UnityEngine;
    using Photon.Pun;
    using Photon.Realtime;
    using UnityEngine.UI;

    public class DeckNet:Deck
    {
       /* public override Card MakeCard(string s, string n, int attack, int point)
        {
            GameObject CardGO =PhotonNetwork.InstantiateRoomObject("Prefubs/"+CardPref.name,deckAnchor.position,deckAnchor.rotation);
            CardGO.transform.SetParent(deckAnchor,false);
            Card card = CardGO.GetComponent<Card>();
       
            card.Name = s;
            card.Attack = attack;
            card.Points = point;
            card.HideLogo.sprite = Resources.Load<Sprite>("Cards/0_0");
            card.Logo.sprite = Resources.Load< Sprite > (n);
       
            return card;
        }*/

        [PunRPC]
            public override void InitDeck(GameType type , int players)
            {
               // MakeCards();
               
                switch (type)
                {
                    case GameType.none :
                    case GameType.classic:
                        if (players==3)
                        {
                            Destroy(cards[cards.Count-1].gameObject);
                            cards.RemoveAt(cards.Count-1);
                        }
                        break;
                    case GameType.original:
                        if (players==3)
                        {
                            Destroy(cards[cards.Count-1].gameObject);
                            cards.RemoveAt(cards.Count-1);
                        }
                        break;
                    case GameType.logic:
                    case GameType.tactic:
                        int index = players * 10 + 4;
                        int count = cards.Count - index;
                        
                        for (int i = index; i <cards.Count; i++)
                        {
                            Destroy(cards[i].gameObject);
                        }
                        cards.RemoveRange(index,count);
                        break;
                }
            }
    }
