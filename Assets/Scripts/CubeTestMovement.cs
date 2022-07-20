using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTestMovement : MonoBehaviour
{

    public float speed;
    public float radius, angle;

    void Start()
    {
        
    }


    void Update()
    {
        transform.position = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius, 0f);

        angle += Time.deltaTime * speed;
        angle = angle >= 360f ? 0 : angle;
        //TODO:
    }
}
