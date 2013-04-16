using UnityEngine;
using System.Collections;

public class CamDragger : MonoBehaviour
{
    static Camera cam = Camera.main;

    Vector3 lastPos = new Vector3(0,0,0);
    // Use this for initialization
    void Start ()
    {

    }

    void LateUpdate ()
    {
        Vector3 pos = Input.mousePosition;
        Vector3 delta = (pos - lastPos)/20;

        if(Input.GetMouseButton(1)){
            cam.transform.position += delta;
        }

        lastPos = pos;
    }
}
