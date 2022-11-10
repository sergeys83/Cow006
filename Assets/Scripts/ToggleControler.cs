using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleControler : MonoBehaviour
{

    [SerializeField]
    private List<Toggle> playerList = new List<Toggle>();
    private bool load = false;
    private CanvasGroup mCanvasGroup;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("players"))
        {
            PlayerPrefs.SetInt("players", 2);
        }

        mCanvasGroup = GetComponentInParent<CanvasGroup>();
    }

    private void Update()
    {
        if (mCanvasGroup.alpha == 0 && load)
        {
            load = false;
        }
        if (mCanvasGroup.alpha == 1f && !load)
        {
            load = true;

            int i = PlayerPrefs.GetInt("players");
            foreach (Toggle item in playerList)
            {
                if (item.name.EndsWith(i.ToString()))
                {
                    item.GetComponent<Toggle>();
                    item.isOn = true;
                    //    Debug.Log("{i} " +i+"  "+item.name+" +Name");
                }
            }
        }
    }
}
