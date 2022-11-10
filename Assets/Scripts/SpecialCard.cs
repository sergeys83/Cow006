using System.Collections.Generic;
using Cards;
using UnityEngine;
public class SpecialCard:CardView
{
    public Ability ability;
    public string description;

    SpecialCard(string name, string path, Ability ability, string description)
    {
        name = name;
        this.ability = ability;
        this.description = description;
        HideLogo.sprite = Resources.Load<Sprite>("Cards/0_0");
        Avatar.sprite = Resources.Load<Sprite>(path);
    }
    
}

public class Ability
{
    private int card;
    private int row;
    private int pos;

    Ability()
    {
        
    }
}
