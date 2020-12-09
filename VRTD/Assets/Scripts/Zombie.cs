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
    NavMeshAgent agent;
    private SpawnManager spawnManager;
    FireSource fire;
    public AudioClip[] sounds;
    AudioSource zombie_audio;

    public bool dead;
    float attackTimer;
    float soundTimer;
    public float SPEED;
    public int MAX_HEALTH;
    
    
    bool isAttacking;

    protected override void Initialize()
    {
        Debug.Log("Initialized!");
        setHealth(MAX_HEALTH);
        dead = false;
        isAttacking = false;
        //removeArrows();
        anim = GetComponentInChildren<Animator>();
        //goal = FindObjectOfType<Player>().gameObject.transform.position;
        goal = new Vector3(0, 0, 0);
        spawnManager = FindObjectOfType<SpawnManager>();
        agent = GetComponent<NavMeshAgent>();
        fire = GetComponent<FireSource>();
        zombie_audio = GetComponent<AudioSource>();
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

        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            transform.LookAt(new Vector3(0,0,0) * Time.deltaTime);
            if (attackTimer > 2)
                attack();
        }

        if (soundTimer < 0)
        {
            MakeSound();
        }

        if (!isAttacking && this.transform.position.x < 2 && this.transform.position.x > -2)
        {
            if (this.transform.position.z < 2 && this.transform.position.z > -2)
            {
                BeginAttack();
            }
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
        if (Random.Range(0,1) > 0)
        {
            anim.SetBool("doAttack1", true);
            anim.SetBool("doAttack2", false);
        }
        else
        {
            anim.SetBool("doAttack2", true);
            anim.SetBool("doAttack1", false);
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
