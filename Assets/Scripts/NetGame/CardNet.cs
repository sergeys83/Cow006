using System.Collections.Generic;
using Cards;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CardNet : CardView,IPunObservable , IPointerClickHandler
  {
      [System.NonSerialized]
    public Player callbackPlayer = null;
    public CardState state = CardState.drawpile;
    public int playerNum;
    public List<Vector3> bezPts;
    public List<Quaternion> bezRot;
    public AnimationCurve m_move = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
    
    public void MoveTo(Vector3 ePos, Quaternion eRot)
    {
        bezPts = new List<Vector3>();
        bezPts.Add(transform.position);
        bezPts.Add(ePos);

        bezRot = new List<Quaternion>();
        bezRot.Add(transform.rotation);
        bezRot.Add(eRot);
    }

    public void MoveX(Vector3 start, Vector3 stop, float dur)
    {
        transform.DOMove(stop, dur, false)
         .SetEase(m_move).OnComplete(()=>
         {
         });
    }

    public void MoveXLocal(Vector3 start, Vector3 stop, float dur)
    {
        
     transform.DOLocalMove(stop, dur, false)
       .SetEase(m_move).OnComplete(() =>
       {
       });
    }
    
    public void MoveTo(Vector3 ePos)
    {
        MoveTo(ePos, Quaternion.identity);
    }
    
   void Update()
    {
       switch (state)
        {
            case CardState.toHand:
                MoveXLocal(bezPts[0], bezPts[bezPts.Count - 1], CARD_DURATION); 
                state = CardState.hand;
                if (playerNum ==PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    faceUp = true;
                }
                break;

            case CardState.toTarget:
               MoveX(bezPts[0], bezPts[bezPts.Count - 1], CARD_DURATION);
               state = CardState.target;
               break;
            case CardState.toDrowpile:
                MoveX(bezPts[0], bezPts[bezPts.Count - 1], CARD_DURATION);
                state = CardState.drawpile;
                break;
            case CardState.to:
                MoveX(bezPts[0], bezPts[bezPts.Count - 1], CARD_DURATION);
                state = CardState.idle;
                break;
            case CardState.toRaw:
                MoveX(bezPts[0], bezPts[bezPts.Count - 1], CARD_DURATION);
                state = CardState.Raw;
                break;
            case CardState.toGraveyard:
                MovetoGraveyard(bezPts[bezPts.Count - 1], CARD_DURATION);
                state = CardState.Graveyard;
                break;
        }
    }
   public void OnPointerClick(PointerEventData eventData)
   {
       if (playerNum!=PhotonNetwork.LocalPlayer.ActorNumber)
       {
           Debug.Log("Its not your card");
           return;
       }
           GameManagerNet.S.CardClicked(this);
           print(name);
   }

   public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {}
   
   [PunRPC]
   public void MakeRemoteDeck(int Attack, int Points, string name)
   {
       this.name.text = name;
       this.Attack = Attack;
       this.Points = Points;
     
       HideLogo.sprite = Resources.Load<Sprite>("Cards/0_0");
       Avatar.sprite = Resources.Load< Sprite > ("Cards/"+name);
      
      transform.SetParent(GameManagerNet.S.Board,false); 
      GameManagerNet.S.deck.cards.Add(this);
      GameManagerNet.S.drawPile.Add(this);
   }
   
   [PunRPC]
   public void MoveToRawX(string r, Vector3 pos)
   {
       state = CardState.toRaw;
       MoveTo(pos);
       faceUp = true;
       Transform rw = GameManagerNet.S.cardsTr.Find(o => o.name == r);
       transform.SetParent(rw);
   }
   
   [PunRPC]
   public void MoveToTarget()
   {
       state = CardState.toTarget;
       MoveTo(GameManagerNet.S.layout.drawPile.pos);
       transform.SetParent(GameManagerNet.S.Temp);
   }
   
   [PunRPC]
   public void MovetoGraveyard(Vector3 stop, float dur)
   {
       transform.DOMove(stop, dur, false);
   }
   
   [PunRPC]
   public void MoveToRaw(string r)
   {
    state = CardState.toRaw;
    Transform rw = GameManagerNet.S.cardsTr.Find(o => o.name == r);
    MoveTo(rw.position);
    transform.SetParent(rw);
    faceUp = true;
   }
  }
