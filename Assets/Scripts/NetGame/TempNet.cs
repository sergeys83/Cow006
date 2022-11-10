using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

    public class TempNet : MonoBehaviour
    {
         bool isTrigger = false;
            public List<CardNet> _list;
            int players;
          
            private void Start()
            {
                players = GameManagerNet.S.numberPlayers;
            }
            void Update()
            {
                if (isTrigger)
                {
                    isTrigger = false;
                    GameManagerNet.S.phase = GameManagerNet.TurnPhase.endRound;
                }
            }
            public void OnGetChildren()
            {
                int i = transform.childCount;
        
                    for (int j = 0; j < i; j++)
                    { 
                        _list.Add(transform.GetChild(j).GetComponent<CardNet>());
                    }
                    isTrigger = true;
            }
            
            private void Sorting(ref List<CardNet> tlist)
            {
                var _cardsT = new List<CardNet>(tlist.OrderByDescending(item=>item.Attack)).ToArray();
                var list = new List<CardNet>(_cardsT);
                tlist = list;
            }
            
            public void TurnCards()
            {
               // OnGetChildren();
                Sorting(ref _list);
                
                float cardWidth = 87f;
                Vector3 pos = transform.position;
                for (int i = 0; i < _list.Count; i++)
                {
                    _list[i].transform.DOMove(new Vector3(pos.x + cardWidth / 3 * players - i * cardWidth, pos.y, pos.z), 0.2f, false)
                        .OnComplete(()=>
                        {
                            GameManagerNet.S.turnManager.SendMove(_list[i].Attack, true);
                        });
                    _list[i].transform.DORotate(Vector3.up*360, 0.2f, RotateMode.LocalAxisAdd);
                    _list[i].faceUp = true;
                }
            }
    }
