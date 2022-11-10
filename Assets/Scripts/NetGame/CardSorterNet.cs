using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

    public class CardSorterNet : MonoBehaviour
    {
        public List<CardNet> hand = new List<CardNet>();
        private void OnTransformChildrenChanged()
        {
        
            if (hand!=null)
            {
                hand.Clear();
     
            }

            for (int i = 0; i < transform.childCount; i++)
            {
               hand.Add(transform.GetChild(i).GetComponent<CardNet>());
            }

            if (hand.Count>1)
            {
                Sorting();
                NewSortedLine(hand);
            }
       
        }

        private void Sorting()
        {
            var _cardsT = new List<CardNet>(hand.OrderBy(item=>item.Attack)).ToArray();
            hand.Clear();
            hand = new List<CardNet>(_cardsT);
        }
        private void NewSortedLine(List<CardNet> hand)
        {
            Vector3 pos = Vector3.zero * CardNet.CARD_HEIGHT/ 2f;
            for (int i = 0; i < hand.Count; i++)
            {  
                pos.x = transform.position.x +(80f)+ 85f * i+10;    
                pos.y = hand[i].transform.position.y;
                pos.z = hand[i].transform.position.z;
                hand[i].transform.DOMove(pos, 0.1f, false);
            }
       
        }
   
    }
