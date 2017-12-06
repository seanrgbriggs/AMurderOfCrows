using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour {

    public Boid boidPrefab;
    public float interval;
    float elapsed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        elapsed += Time.deltaTime;
        if(elapsed > interval)
        {
            elapsed = 0;

            Boid boid = Instantiate(boidPrefab, transform.position, Quaternion.identity);

            Boid.BoidType[] types = (Boid.BoidType[]) System.Enum.GetValues(typeof(Boid.BoidType));
            boid.flockID = types[Random.Range(0,types.Length)];
            switch (boid.flockID)
            {
                case Boid.BoidType.Black:
                    boid.sr.color = Color.black;
                    break;
                case Boid.BoidType.Red:
                    boid.sr.color = Color.red;
                    break;
                case Boid.BoidType.Blue:
                    boid.sr.color = Color.blue;
                    break;
                case Boid.BoidType.Green:
                    boid.sr.color = Color.green;
                    break;
                case Boid.BoidType.White:
                    boid.sr.color = Color.white;
                    break;
            }

            boid.rb.velocity = Random.insideUnitCircle.normalized;

        }
	}
}
