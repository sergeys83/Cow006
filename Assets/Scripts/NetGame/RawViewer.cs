
    using System.Collections.Generic;
    using Photon.Pun;
    using UnityEngine;

    public class RawViewer:MonoBehaviour
    {
        public AudioClip CrunchClip;
        public List<CardNet> _list = new List<CardNet>();
        public List<CardNet> ListToDelete = new List<CardNet>();
        public int Count;
        public void OnChildrenChanged()
        {
            Count = _list.Count;
        }
        private void Start()
        {
            GameManagerNet.S.onConditionChanged += OnChildrenChanged;
            GameManagerNet.S.onLineCleared += LineDestroyed;
        }
        private void OnDisable()
        {
            GameManagerNet.S.onConditionChanged -= OnChildrenChanged;
            GameManagerNet.S.onLineCleared -= LineDestroyed;
        }
        private void LineDestroyed()
        {
            _list.Clear();
        }
        
        public void CardRowRemover(int player)
        {
            RemoveCards();
            for (int i = 0; i < ListToDelete.Count; i++)
            {
                ListToDelete[i].gameObject.transform.SetParent(GameManagerNet.S.TargetPlayer(player));
                PhotonView PV =  ListToDelete[i].gameObject.GetPhotonView();
                if (ListToDelete[i]!=null)
                {
                    PV.RPC("MovetoGraveyard",RpcTarget.All,GameManagerNet.S.TargetPlayer(player).position, 0.5f);
                }
            }
            ListToDelete.Clear();
            
            MessageCardRemoved msg = new MessageCardRemoved(CrunchClip);
            EventManager.Instance.SendEvent(EventId.CardRemoved,msg);
        }
        
        public void AddCd(CardNet card)
        {
            _list.Add(card);
        }    
    
        public CardNet lastCard()
        { 
            return _list[_list.Count-1];
        }

        public void RemoveCards()
        {
            ListToDelete.AddRange( _list);
            _list.Clear();
        }
        public List<CardNet> RowList()
        {
            return _list;
        }

        public int RowPoints()
        {
            int Point = 0;
            int i = 1;
            List<CardNet> tmpList=new List<CardNet>(_list);
            while (tmpList.Count>0)
            {
                Point += tmpList[0].Points;
                tmpList.RemoveAt(0);
                i++;
            }
            return Point;
        }
        
    }
