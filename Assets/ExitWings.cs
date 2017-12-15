using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitWings : MonoBehaviour {

    public int radius;
    List<Player> allLivingInnocents
    {
        get
        {
            return GameController.instance.players.FindAll(player => GameController.instance.alive[player.playerID] && !player.isMurderer);
        }
    }
	
	// Update is called once per frame
	void Update () {

        System.Predicate<Player> nearTheWings = player => Vector2.Distance(transform.position, player.transform.position) <= radius;
        int numPresent = allLivingInnocents.FindAll(nearTheWings).Count;

        if(numPresent == allLivingInnocents.Count)
        {
            int numKeys = 0;
            allLivingInnocents.ForEach(player => numKeys += player.keysCollected);
            if(numKeys == 4)
            {
                print("win!");
                allLivingInnocents.ForEach(player => player.isFree = true);
                Destroy(gameObject);
            }
        }
        		
	}
}
