using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayMode : MonoBehaviour
{
    // Start is called before the first frame update
    public void SetCAnvasMode()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
    }
}
