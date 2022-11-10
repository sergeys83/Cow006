using System.Collections.Generic;
using UnityEngine;
namespace Cards
{
    [CreateAssetMenu(menuName = "CardList", fileName = "NewList")]
    public class CardListSc : ScriptableObject
    {
        public List<CardData> origCards;
    }
}
