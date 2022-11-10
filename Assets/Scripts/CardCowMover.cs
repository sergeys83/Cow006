using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardCowMover : MonoBehaviour, IPointerClickHandler
{
    public CardState state = CardState.drawpile;
    public int playerNum;
    public List<Vector3> bezPts;
    public List<Quaternion> bezRot;
    public CardView cv;
    private bool canMove = false;
    public bool onPlace = false;

    public AnimationCurve m_move = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    private void Awake()
    {
        cv = GetComponent<CardView>();
    }
    public void MoveTo(Vector3 ePos, Quaternion eRot)
    {
        bezPts = new List<Vector3>();
        bezPts.Add(transform.position);
        bezPts.Add(ePos);

        bezRot = new List<Quaternion>();
        bezRot.Add(transform.rotation);
        bezRot.Add(eRot);
        canMove = true;
    }
    
    public void MoveTo(Vector3 ePos)
    {
        MoveTo(ePos, Quaternion.identity);
    }

    public void MoveX(Vector3 start, Vector3 stop, float dur)
    {
        canMove = false;
        transform.DOMove(stop, dur, false)
            .SetEase(m_move).OnComplete(() =>
            {
                OnMoveEnd(state);
            });
    }

    public void MoveXLocal(Vector3 start, Vector3 stop, float dur)
    {
        canMove = false;
        transform.DOLocalMove(stop, dur, false)
            .SetEase(m_move).OnComplete(() =>
            {
                switch (state)
                {
                    case CardState.toHand:
                        state = CardState.hand;
                        break;
                    case CardState.toTempRaw:
                        state = CardState.Raw;
                        break;
                }
            });
    }

    void Update()
    {
        if (canMove)
        {
            switch (state)
            {
                case CardState.toHand:
                    MoveXLocal(bezPts[0], bezPts[bezPts.Count - 1], CardView.CARD_DURATION);
                    break;

                case CardState.toTarget:
                    MoveX(bezPts[0], bezPts[bezPts.Count - 1], CardView.CARD_DURATION);
                    Debug.Log($"target {playerNum}");
                    break;

                case CardState.toDrowpile:
                case CardState.to:
                    MoveX(bezPts[0], bezPts[bezPts.Count - 1], CardView.CARD_DURATION);
                    break;

                case CardState.toRaw:
                    MoveX(bezPts[0], bezPts[bezPts.Count - 1], CardView.CARD_DURATION);
                    break;
                case CardState.toTempRaw:
                    MoveXLocal(bezPts[0], bezPts[bezPts.Count - 1], CardView.CARD_DURATION);
                    break;
                case CardState.toGraveyard:
                    MoveX(bezPts[0], bezPts[bezPts.Count - 1], CardView.CARD_DURATION);
                    break;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManagerScr.S.CardClicked(this);
    }

    public void OpenAndMove(int num, int players)
    {
        transform.DOLocalMove(new Vector3(CardView.CARD_WIDTH / 3 * players - num * CardView.CARD_WIDTH, 0f, 0f), 0.2f, false).OnComplete(() =>
        {
            cv.faceUp = true;
        });
    }

    private void OnMoveEnd(CardState oldstate)
    {
        switch (oldstate)
        {
            case CardState.toHand:
                state = CardState.hand;
                break;
            case CardState.toTarget:
                float x = 0.7f;
                while (x>0)
                {
                    x -= Time.deltaTime;
                }

                state = CardState.target;
                MessageCardMoveToTarget msgToTarget = new MessageCardMoveToTarget(this);
                EventManager.Instance.SendEvent(EventId.CardMoveToTarget, msgToTarget);

                break;

            case CardState.toDrowpile:
                state = CardState.drawpile;
                break;
            case CardState.to:
                state = CardState.idle;
                break;
            case CardState.toRaw:
                
                MessageCardMoveToRaw msg = new MessageCardMoveToRaw(this, transform.parent);
                EventManager.Instance.SendEvent(EventId.CardMoveToRow, msg);
                onPlace = true;
                state = CardState.Raw;
                break;

            case CardState.Raw:
                break;
            case CardState.toTempRaw:

                onPlace = true;
                state = CardState.Raw;
                break;
            
            case CardState.toGraveyard:
                   
                    Debug.Log($"LALA toGraveyard card ={cv.Attack}");
                    
                    GameManagerScr.S.players[playerNum-1 ].plStack.Add(this);
                    Score.Sc.SetScore(playerNum, cv.Points);
                   
                    state = CardState.Graveyard; 
                    onPlace = true;
                    gameObject.SetActive(false);
               
                break;
            
            case CardState.Graveyard:
                break;
        }
    }
}
