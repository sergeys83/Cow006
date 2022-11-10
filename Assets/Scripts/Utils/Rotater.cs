using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float rot = 1f;


// Start is called before the first frame update
  
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += new Vector3(0, 0,transform.position.y* Time.deltaTime*rot*speed);
    }
}
