using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;
using PathologicalGames;

public class Zombie : Character
{
    public int this_health;
    Animator anim;
    public bool dead;
    float Timer;
    Vector3 goal;
    NavMeshAgent agent;
    public float SPEED;
    FireSource fire;
    private SpawnManager spawnManager;

    protected override void Initialize()
    {
        Debug.Log("Initialized!");
        setHealth(100);
        dead = false;
        removeArrows();
        anim = GetComponentInChildren<Animator>();
        goal = FindObjectOfType<Player>().gameObject.transform.position;
        spawnManager = FindObjectOfType<SpawnManager>();
        agent = GetComponent<NavMeshAgent>();
        fire = GetComponent<FireSource>();
        if (!anim.isInitialized)
        {
            anim.Rebind();
        }
        fire.isBurning = false;
        agent.speed = SPEED;
        agent.destination = goal;
        agent.isStopped = false;
        //ResetAnimations();
        if (Random.Range(0, 2) > 2)
        {
            anim.SetBool("OtherWalk", true);
        }
        else
        {
            anim.SetBool("OtherWalk", false);
        }
    }      

    void ResetAnimations()
    {
        anim.SetBool("FireDeath", false);
        anim.SetBool("ProjectileDeath", false);
    }

    void removeArrows()
    {
        Arrow[] arrows = GetComponentsInChildren<Arrow>();
        for (int i = 0; i < arrows.Length; i++)
        {
            Destroy(arrows[i]);
        }
    }

    protected override void Enabled()
    {
        
    }

    protected override void Disabled()
    {
        //Maybe unassign some event handlers here...
    }

    protected override void OnUpdate()
    {
        
        agent.destination = FindObjectOfType<Player>().gameObject.transform.position;
        if (fire.isBurning && getHealth() >= 1)
        {
            setHealth(-1);
        }

        if (!dead && getHealth() <= 0)
        {
            agent.isStopped = true;
            if (fire.isBurning)
            {
                anim.SetBool("FireDeath", true);
            }
            else
            {
                anim.SetBool("ProjectileDeath", true);
            }
            PoolManager.Pools["Enemies"].Despawn(this.transform, 3.5f);
            spawnManager.DecrementLiveZombies();
            dead = true;
        }
        this_health = getHealth();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("projectile"))
        {
            setHealth(-50);
            Destroy(collision.gameObject);
        }
    }

    protected override void Destroy()
    {
        Disabled();
    }
}
