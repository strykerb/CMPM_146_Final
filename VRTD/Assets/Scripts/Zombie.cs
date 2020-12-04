using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;
using PathologicalGames;

public class Zombie : Character
{
    Animator anim;
    bool dead;
    float Timer;
    Vector3 goal;
    NavMeshAgent agent;
    public float SPEED;
    FireSource fire;
    private SpawnManager spawnManager;

    protected override void Initialize()
    {
        setHealth(100);
        dead = false;
        removeArrows();
        anim = GetComponentInChildren<Animator>();
        goal = FindObjectOfType<Player>().gameObject.transform.position;
        spawnManager = FindObjectOfType<SpawnManager>();
        agent = GetComponent<NavMeshAgent>();
        fire = GetComponent<FireSource>();
        Debug.Log("Before reset: " + this + " Animation Booleans: " + anim.GetBool("FireDeath") + anim.GetBool("ProjectileDeath") + anim.GetBool("OtherWalk"));
        if (!anim.isInitialized)
        {
            anim.Rebind();
        }
        fire.isBurning = false;
        agent.speed = SPEED;
        agent.destination = goal;
        agent.isStopped = false;
        //ResetAnimations();
        if (Random.Range(0, 2) > 0)
        {
            anim.SetBool("OtherWalk", true);
        }
        else
        {
            anim.SetBool("OtherWalk", false);
        }

        Debug.Log("Spawned " + this + "Animation Booleans: " + anim.GetBool("FireDeath") + anim.GetBool("ProjectileDeath") + anim.GetBool("OtherWalk"));
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
        if (fire.isBurning)
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
