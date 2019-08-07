using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_RotateX : MonoBehaviour
{
    [SerializeField] float rotateSpeedX = 100f;
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        obbyRotate();
    }
    void obbyRotate()
    {
        transform.Rotate(Vector3.forward, rotateSpeedX * Time.deltaTime);
    }
}

