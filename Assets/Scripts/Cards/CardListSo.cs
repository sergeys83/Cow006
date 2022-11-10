
using System.Collections.Generic;
using Cards;
using UnityEngine;
using UnityEditor;

public class CardListSo : MonoBehaviour
{
    public List<CardData> CardDataList = new List<CardData>();

    public List<GameObject> prefubs = new List<GameObject>();
    public List<Sprite> SpritesLogo = new List<Sprite>();
    public CardListSc SO;
   
    [ContextMenu("SO")]
    public void SoData()
    {
      //  AssetDatabase.OpenAsset(SO);
        SO.origCards = new List<CardData>();
    //    EditorUtility.SetDirty(SO);
     //   AssetDatabase.SaveAssets();
        
    }
    [ContextMenu("SetData")]
    public void SetData()
    {
        foreach (var item in CardDataList)
        {
            item.HideLogo = CardDataList[0].Logo;
        }
    }
    
    [ContextMenu("ChangeeCardPrefub")]
    public void ChangeeCardPrefub()
    {
        foreach (var data in CardDataList)
        {
            data.CardPrefub = prefubs[0];
            
            if (data.Attack%5 ==0)
            {
               
                data.CardPrefub = prefubs[1];
                data.Logo = SpritesLogo[1];
            }
            if (data.Attack%10 ==0)
            {
               
                data.CardPrefub = prefubs[2];
                data.Logo = SpritesLogo[0];
            }
            if (data.Attack%11 ==0)
            {
               
                data.CardPrefub = prefubs[3];
                data.Logo = SpritesLogo[1];
            }
            if (data.Attack%55 ==0)
            {
               
                data.CardPrefub = prefubs[4];
                data.Logo = SpritesLogo[2];
            }
                
           
        }
    }
    
    [ContextMenu("AddElement")]
    public void AddElement()
    {
        foreach (var data in CardDataList)
        {
            if (data.Attack%5 ==0)
            {
                data.Points = 2;
                data.CardPrefub = prefubs[1];
                data.Logo = SpritesLogo[1];
            }
            if (data.Attack%10 ==0)
            {
                data.Points = 3;
                data.CardPrefub = prefubs[2];
                data.Logo = SpritesLogo[0];
            }
            if (data.Attack%11 ==0)
            {
                data.Points = 5;
                data.CardPrefub = prefubs[3];
                data.Logo = SpritesLogo[1];
            }
            if (data.Attack%55 ==0)
            {
                data.Points = 7;
                data.CardPrefub = prefubs[4];
                data.Logo = SpritesLogo[2];
            }
        }
    }
    
}
