using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour {

    public Boid boidPrefab;
    public float interval;
    float elapsed;
    public bool shouldSpawn = true;
    int spawned = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        elapsed += Time.deltaTime;
        if(elapsed > interval && shouldSpawn)
        {
            elapsed = 0;
            spawned++;

            Boid boid = Instantiate(boidPrefab, transform.position, Quaternion.identity);
            boid.spawnNum = spawned;

            Boid.BoidType[] types = (Boid.BoidType[]) System.Enum.GetValues(typeof(Boid.BoidType));

            boid.flockID = types[Random.Range(0,types.Length-1)];
            switch (boid.flockID)
            {
                case Boid.BoidType.Black:
                    boid.spriteRenderer.color = Color.black;
                    boid.light.color = Color.Lerp(Color.black, Color.magenta, 0.3f);
                    break;
                case Boid.BoidType.Red:
                    boid.spriteRenderer.color = boid.light.color = Color.red;
                    break;
                case Boid.BoidType.Blue:
                    boid.spriteRenderer.color = boid.light.color = Color.blue;
                    break;
                case Boid.BoidType.Green:
                    boid.spriteRenderer.color = boid.light.color = Color.green;
                    break;
                case Boid.BoidType.White:
                    boid.spriteRenderer.color = boid.light.color = Color.white;
                    break;
            }

            boid.rb.velocity = Random.insideUnitCircle.normalized;

        }
	}
}
