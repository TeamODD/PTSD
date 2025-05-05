using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseChaser : MonoBehaviour
{
    public Transform ob2;
    Vector3 pos;

    // Update is called once per frame
    void FixedUpdate()
    {
        // ob2
        pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9f));

        if (ob2 != null)
            ob2.position = pos;

    }
}
