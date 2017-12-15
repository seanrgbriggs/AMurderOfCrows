using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRightWrap : MonoBehaviour {

    public float rightX;
    public float topY;

	// Update is called once per frame
	void Update () {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if(transform.position.x > rightX)
        {
            //rb.velocity = Vector2.Reflect(rb.velocity, Vector2.left);
            //if(transform.position.x > rightX + 1)
                transform.position += Vector3.left * 2 * rightX;
        }else if (transform.position.x < -rightX)
        {
            //rb.velocity = Vector2.Reflect(rb.velocity, Vector2.right);
            //if(transform.position.x < -rightX - 1)
                transform.position += Vector3.right* 2 * rightX;
        }

        if (transform.position.y > topY)
        {
            //rb.velocity = Vector2.Reflect(rb.velocity, Vector2.down);
            //if(transform.position.y > topY + 1)
                transform.position += Vector3.down * 2 * topY;
        }
        else if(transform.position.y < -topY)
        {
            //rb.velocity = Vector2.Reflect(rb.velocity, Vector2.up);
            //if(transform.position.y < -topY - 1)
                transform.position += Vector3.up * 2 * topY;
        }
    }
}
