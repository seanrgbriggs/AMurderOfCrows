using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Boid : MonoBehaviour {

    public enum BoidType
    {
        Red,Blue,Green,White,Black
    }

    public BoidType flockID;
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    public int spawnNum;

    [Header("Flocking Values")]
    public float cohesion;
    public float alignment;
    public float separation;
    public float visionRadius;
    public float maxVelocity;
    public bool shouldMove = true;
    public bool isDone = false;

    // Update is called once per frame
    void Update () {
        if (shouldMove) {
            List<Boid> neighbors = getNeighbors();
            List<Boid> flockNeighbors;
            List<Boid> nonFlockNeighbors;
            siftForFlock(neighbors, out flockNeighbors, out nonFlockNeighbors);

            Vector2 flockVelocity = Vector2.zero;
            flockVelocity += cohesion * cohesionVector(flockNeighbors);
            flockVelocity += alignment * alignmentVector(flockNeighbors);
            flockVelocity += separation * separationVector(neighbors);

            rb.velocity += flockVelocity;
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);

            transform.rotation = Quaternion.FromToRotation(Vector2.up + Vector2.left, rb.velocity);
        }
    }

    //The force vector which makes boids of a feather flock together 
    Vector2 cohesionVector(List<Boid> neighbors)
    {
        Vector2 centerOfMass = rb.position; //The center of mass of the flock
        foreach(Boid boid in neighbors)
        {
            centerOfMass += boid.rb.position;
        }
        centerOfMass /= neighbors.Count + 1;

        return centerOfMass - rb.position;
    }

    //The force vector which makes boids conform to peer pressure
    Vector2 alignmentVector(List<Boid> neighbors)
    {
        //If there are no nearby neighbors, the alignment force is the boid's speed
        if (neighbors.Count == 0)
            return rb.velocity;

        Vector2 averageVelocity = Vector2.zero; //The average velocity of the flock
        foreach(Boid boid in neighbors)
        {
            averageVelocity += boid.rb.velocity;
        }
        averageVelocity /= neighbors.Count;

        return averageVelocity;// - rb.velocity;
    }

    //The force vector which makes boids care about personal space
    Vector2 separationVector(List<Boid> neighbors)
    {
        //If there are no nearby neighbors, the separation force is zero
        if (neighbors.Count == 0)
            return Vector2.zero;

        Vector2 repelForce = Vector2.zero; //The repellant force from nearby boids
        foreach(Boid boid in neighbors)
        {
            //The Vector from this neighbor to the boid
            Vector2 delta = rb.position - boid.rb.position;
            
            //If the boid and neighbor overlap, repel each other, otherwise repel inversely to the distance
            repelForce += delta.magnitude == 0 ? Vector2.one * maxVelocity : delta.normalized * Mathf.Pow(delta.magnitude, -1);
        }
        repelForce /= neighbors.Count;

        return repelForce;
    }

    //All boids in this boids' vision range
    List<Boid> getNeighbors()
    {
        List<Boid> output = new List<Boid>(FindObjectsOfType<Boid>());
        output = output.FindAll(boid => Vector2.Distance(boid.transform.position, transform.position) <= visionRadius);
        output.Remove(this);
        return output;
    }

    //Separates boids into two lists, one for those in the same flock, and one for the rest
    void siftForFlock(List<Boid> members, out List<Boid> flockMembers, out List<Boid> nonMembers)
    {
        flockMembers = members.FindAll(boid => boid.flockID == this.flockID);
        nonMembers = members.FindAll(boid => boid.flockID != this.flockID);
    }
}
