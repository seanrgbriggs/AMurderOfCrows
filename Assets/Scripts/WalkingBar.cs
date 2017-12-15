using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingBar : MonoBehaviour {

    public float TotalTime;
    public float TimeLeft;

	void Start () {
        TimeLeft = TotalTime;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            LowerTime();
        }
	}

    void LowerTime()
    {
        if(TimeLeft > 0)
        {
            TimeLeft -= 1;
            transform.localScale = new Vector3((TimeLeft / TotalTime), 1, 1);
        }
    }
}
