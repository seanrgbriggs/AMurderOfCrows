using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitWings : MonoBehaviour {

    public int radius;
    List<Player> allPlayers
    {
        get
        {
            return GameController.instance.players;
        }
    }
	
	// Update is called once per frame
	void Update () {

        System.Predicate<Player> nearTheWings = player => Vector2.Distance(transform.position, player.transform.position) <= radius;
        int numPresent = allPlayers.FindAll(player => nearTheWings(player) && !player.isMurderer).Count;

        if(numPresent >= allPlayers.Count - 1)
        {
            int numKeys = 0;
            allPlayers.ForEach(player => numKeys += player.keysCollected);
            if(numKeys == 4)
            {
                print("win!");
                Destroy(gameObject);
            }
        }
        		
	}
}
