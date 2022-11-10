
using System;
using UnityEngine;

public class GetLineNet : MonoBehaviour
{
    public GameObject GameLine;
    public void OnBtnRow1()
    {
      GameManagerNet.S.BtnLine = 1;
    //  Debug.Log($"{GameManagerScr.S.BtnLine} =  1");
      GameLine.SetActive(false);
    }

    public void OnBtnRow2()
    {  
        GameManagerNet.S.BtnLine =2;
    //    Debug.Log($"{GameManagerScr.S.BtnLine} = 2");
        GameLine.SetActive(false);
    }
        public void OnBtnRow3()
    {
        GameManagerNet.S.BtnLine =3;
     //   Debug.Log($"{GameManagerScr.S.BtnLine} = 3");
        GameLine.SetActive(false);
    }
    public void OnBtnRow4()
    {
        GameManagerNet.S.BtnLine =4;
     //   Debug.Log($"{GameManagerScr.S.BtnLine} = 4");
        GameLine.SetActive(false);
    }

    private void Awake()
    {
       GameLine.transform.SetAsLastSibling();
    }

    // Start is called before the first frame update
    void Start()
    {
       GameLine.SetActive(true);
    }
    
 }
