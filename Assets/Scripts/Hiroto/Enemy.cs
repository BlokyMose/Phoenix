using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Movement
{
    public bool Test;

    private Transform player;
    private Vector3 playerLastPos,startPos,movementPos;
    [SerializeField]
    private float chasespeed = 0.8f, turningDelay = 1f;
    private float lastFollowTime, turningTimeDelay = 1f;

    private Vector3 tempScalse;

    private Health enemyHealth;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerLastPos = player.position;
        startPos = transform.position;
        lastFollowTime = Time.time;

        turningTimeDelay *= turningDelay;

        enemyHealth = GetComponent<Health>();
    }

    // Update is called once per frame
   
    
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        if (!enemyHealth.IsAllive() || !player)
        {
            return;
        }

        TurnAround();

        ChaseingPlayer();

        
    }

    void ChaseingPlayer()
    {
        if (Test)
        {
            Chase();
        }
        else
        {
            movementPos = startPos - transform.position;

            if (Vector3.Distance(transform.position,startPos) < 0.1f)
            {
                movementPos = Vector3.zero;
            }
        }

        CharacterMovement(movementPos.x, movementPos.y);
    }

    void Chase()
    {
        if (Time.time - lastFollowTime > turningTimeDelay)
        {
            playerLastPos = player.transform.position;
            lastFollowTime -= Time.time;
        }

        if (Vector3.Distance(transform.position,playerLastPos) > 0.15f)
        {
            movementPos = (playerLastPos - transform.position).normalized * chasespeed;
        }
        else
        {
            movementPos = Vector3.zero;
        }
    }

    void TurnAround()
    {
        tempScalse = transform.localScale;

        if (Test)
        {
            if (player.position.x > transform.position.x)
            {
                tempScalse.x = Mathf.Abs(tempScalse.x);
            }

            if (player.position.x < transform.position.x)
            {
                tempScalse.x = -Mathf.Abs(tempScalse.x);
            }
        }

        transform.localScale = tempScalse;
    }

   
}
