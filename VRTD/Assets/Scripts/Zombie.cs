using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;
using PathologicalGames;

public class Zombie : Character
{
    Animator anim;
    Vector3 goal;
    GameObject[] torches;
    NavMeshAgent agent;
    private SpawnManager spawnManager;
    FireSource fire;
    public AudioClip[] sounds;
    AudioSource zombie_audio;
    GameObject target;

    public bool dead;
    bool hasDealtDamage;
    float attackTimer;
    float soundTimer;
    public float SPEED;
    public int MAX_HEALTH;

    // This distance seems to look natural
    float stopping_dist = 1f;
    
    bool isAttacking;

    protected override void Initialize()
    {
        // Just Assigning refrences
        spawnManager = FindObjectOfType<SpawnManager>();
        agent = GetComponent<NavMeshAgent>();
        fire = GetComponent<FireSource>();
        zombie_audio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();

        // Health and Speed defined on Prefab in editor
        setHealth(MAX_HEALTH);
        agent.speed = SPEED;

        // set dafault bool values
        dead = false;
        isAttacking = false;
        hasDealtDamage = false;
        fire.isBurning = false;
        agent.isStopped = false;

        // Assign torch object reference array
        torches = spawnManager.torches;

        // Send Navmesh Agent on its merry way to the closest torch
        target = FindTarget();
        SetDestination();
        
        // May not be necessary: resets AC upon spawn
        if (!anim.isInitialized)
        {
            anim.Rebind();
        }
    }
    
    // Returns reference to the closest torch to self
    GameObject FindTarget()
    {
        float least_dist = 10000f;
        GameObject closest_torch = null;

        foreach (GameObject torch in torches)
        {
            if (!torch.activeSelf)
            {
                continue;
            }
            float curr_dist = Vector3.Distance(this.transform.position, torch.transform.position);
            Debug.Log("distance from " + this +  " to " + torch + ": " + curr_dist);
            if (curr_dist < least_dist)
            {
                least_dist = curr_dist;
                closest_torch = torch;
            }
        }
        Debug.Log("closest torch from " + this + " is " + closest_torch);
        return closest_torch;
    }

    void SetDestination()
    {
        target = FindTarget();
        if (target != null)
        {
            goal = new Vector3(target.transform.position.x, 0, target.transform.position.z);
            agent.destination = goal;
        }
        agent.isStopped = false;
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

        //agent.destination = FindObjectOfType<Player>().gameObject.transform.position;
        soundTimer -= Time.deltaTime;
        if (fire.isBurning && getHealth() >= 1)
        {
            setHealth(getHealth() - 1);
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

        if (!target.activeSelf)
        {
            SetDestination();
            isAttacking = false;
        }

        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            //transform.LookAt(goal * Time.deltaTime);
            if (!hasDealtDamage && attackTimer > 1)
            {
                target.GetComponent<Torch>().TakeDamage();
                hasDealtDamage = true;
            }
            if (attackTimer > 2)
                attack();
        }

        if (soundTimer < 0)
        {
            MakeSound();
        }

        if (!isAttacking && Vector3.Distance(this.transform.position, goal) < stopping_dist)
        {
            BeginAttack();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("projectile"))
        {
            setHealth(getHealth() - 100);
            Destroy(collision.gameObject);
        }
    }

    void attack()
    {
        attackTimer = 0;
        hasDealtDamage = false;
        if (Random.Range(0,1) > 0)
        {
            anim.SetBool("doAttack2", true);
            anim.SetBool("doAttack1", false);
        }
        else
        {
            anim.SetBool("doAttack1", true);
            anim.SetBool("doAttack2", false);
        }
    }

    void BeginAttack()
    {
        isAttacking = true;
        agent.isStopped = true;
        attack();
    }

    void MakeSound()
    {
        soundTimer = 3 + Random.Range(0, 7);
        int audio_idx = Random.Range(0, sounds.Length);
        zombie_audio.clip = sounds[audio_idx];
        zombie_audio.Play();
    }

    protected override void Destroy()
    {
        Disabled();
    }
}
