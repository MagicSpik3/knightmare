using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    // Experience
    public int xpValue = 1;

    //Logic
    public float triggerLength = 3;  // How close before enemy starts chasing you
    public float chaseLength = 3;   // ? Probably is How far away you have to get before he stops chasing you?
    private bool chasing;           // status of enemy is chasing or not
    private bool collidingWithPlayer;
    private Transform playerTransform;  // This is the player's position
    private Vector3 startingPosition;   // This is the Enemy's starting position
    public Vector3 enemyVelocity;        // Speed and directon 
    public float distanceToPlayer; // new debug var

    // Hitbox
    public ContactFilter2D filter;
    private BoxCollider2D hitbox;
    private Collider2D[] hits = new Collider2D[10];

    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitbox = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        // Is the player in range of the Enemy Starting Position?
        // distanceToPlayer = Vector3.Distance(playerTransform.position, startingPosition); // this is a float (Duh!)
        distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position); // Current enemy Position

        if (distanceToPlayer < chaseLength)
        {
            // yes, the player is in range, so set 'chasing'
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLength)
                chasing = true;

            // Chasing is set, so...
            if (chasing)
            {
                // We haven't hit the player yet
                if (!collidingWithPlayer)
                {
                    //enemyVelocity = ((playerTransform.position - transform.position).normalized);
                    enemyVelocity = ((playerTransform.position - transform.position));
                    UpdateMotor(enemyVelocity);
                    //UpdateMotor((playerTransform.position - transform.position).normalized);
                }
            }
            // We are NOT chasing
            else
            {
                // Get rid of this
                Debug.Log("we got rid of this bit");
                //UpdateMotor(startingPosition - transform.position);
            }
        }
        // If the player is not in range it will not attack
        else
        {
            UpdateMotor(startingPosition - transform.position);
            chasing = false;
        }

        // Check for overlaps
        collidingWithPlayer = false;
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (hits[i].tag == "Fighter" && hits[i].name == "Player")
            {
                collidingWithPlayer = true;
            } 

            //Debug.Log(hits[i].name);

            //The array is not cleaned up, so we have to do it ourself
            hits[i] = null;
        }
    }

    protected override void Death()
    {
        // The text for when it dies
        Destroy(gameObject);
        GameManager.instance.GrantXp(xpValue);
        GameManager.instance.ShowText("+" + xpValue + "xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
    }
}
