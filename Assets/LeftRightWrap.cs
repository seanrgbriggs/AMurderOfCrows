using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRightWrap : MonoBehaviour {

    public float rightX;
    public float topY;

	// Update is called once per frame
	void Update () {
        if(transform.position.x > rightX)
        {
            transform.position += Vector3.left * 2 * rightX;
        }else if (transform.position.x < -rightX)
        {
            transform.position += Vector3.right* 2 * rightX;
        }

        if(transform.position.y > topY)
        {
            transform.position += Vector3.down * 2 * topY;
        }
        else if(transform.position.y < -topY)
        {
            transform.position += Vector3.up * 2 * topY;
        }
    }
}
