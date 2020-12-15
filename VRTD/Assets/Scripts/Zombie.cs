using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;
using PathologicalGames;

public class Zombie : Character
{
    Animator anim;
    Vector3 goal;                           // Coordinate that the navmesh agent moves towards
    GameObject[] torches;                   // candidates for goal
    NavMeshAgent agent;
    private SpawnManager spawnManager;      // so that we can access torch [] without FindObj... for each zombie
    FireSource fire;
    public AudioClip[] sounds;              // Assigned in editor
    public AudioClip[] deathSounds;         // Assigned in editor
    AudioSource zombie_audio;
    GameObject target;                      // Reference to the closest torch, coords -> goal
    CapsuleCollider hitbox;                 // hitbox for arrow collisions
    CapsuleCollider fireHitbox;             // hitbox for fire collisions, slightly larger than ^

    public bool dead;                       // Ensures that it only dies once
    bool isAttacking;                       // Used for animation logic
    bool hasDealtDamage;                    // Ensures that it only damages target once per animation
    float attackTimer;                      // Countdown until damage is dealt
    float soundTimer;                       // time until next zombie noise 
    public float SPEED;
    public int MAX_HEALTH;

    // This distance seems to look natural
    float stopping_dist = 1f;
    
    
    // called instead of Start() due to prefab Cloning/Reusing
    protected override void Initialize()
    {
        // Just Assigning refrences
        spawnManager = FindObjectOfType<SpawnManager>();
        agent = GetComponent<NavMeshAgent>();
        fire = GetComponent<FireSource>();
        zombie_audio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        hitbox = gameObject.GetComponent<CapsuleCollider>();
        fireHitbox = fire.GetComponent<CapsuleCollider>();

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
            if (curr_dist < least_dist)
            {
                least_dist = curr_dist;
                closest_torch = torch;
            }
        }
        return closest_torch;
    }

    // Directs navmesh agent to new goal
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

    protected override void Enabled()
    {
        
    }

    protected override void Disabled()
    {
        //Maybe unassign some event handlers here...
    }

    protected override void OnUpdate()
    {

        // Despawn zombie if all torches are destroyed
        if (target == null)
        {
            PoolManager.Pools["Enemies"].Despawn(this.transform, 3.5f);
            return;
        }

        soundTimer -= Time.deltaTime;

        // if on fire, take 1 damage per frame
        if (fire.isBurning && getHealth() >= 1)
        {
            setHealth(getHealth() - 1);
        }

        // Handle zombie death
        if (!dead && getHealth() <= 0)
        {
            agent.isStopped = true;
            
            // different animations for different causes of death
            if (fire.isBurning)
            {
                anim.SetBool("FireDeath", true);
            }
            else
            {
                anim.SetBool("ProjectileDeath", true);
            }
            
            // Despawn instead of destroy to reuse in pool manager
            PoolManager.Pools["Enemies"].Despawn(this.transform, 3.5f);
            spawnManager.DecrementLiveZombies();
            dead = true;
        }

        // If current target torch is destroyed, redirect
        if (target != null && !target.activeSelf)
        {
            SetDestination();
            isAttacking = false;
        }

        // Deal damage partway through the animation
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (!hasDealtDamage && attackTimer > 1)
            {
                target.GetComponent<Torch>().TakeDamage();
                hasDealtDamage = true;
            }
            // 2.617 is the length of the attack animation
            if (attackTimer > 2.617)
                attack();
        }

        // time to groan
        if (soundTimer < 0)
        {
            MakeSound();
        }

        // we are close to target. Transition from walking to attacking
        if (!isAttacking && Vector3.Distance(this.transform.position, goal) < stopping_dist)
        {
            BeginAttack();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // We have been struck by an arrow; take 100 damage
        if (collision.gameObject.CompareTag("projectile"))
        {
            setHealth(getHealth() - 100);
            int rand_idx = Random.Range(0, deathSounds.Length - 1);
            zombie_audio.PlayOneShot(deathSounds[rand_idx], 0.5f);
            Destroy(collision.gameObject);
        }
    }

    void attack()
    {
        attackTimer = 0;
        hasDealtDamage = false;
        // this does not work, it just plays one animation. Likely an AC issue.
        if (Random.Range(0,1) > 0.5)
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
        ResetHitbox();
        attack();
    }

    // Crawling zombie needs hitbox reset when he stands up and starts attacking
    void ResetHitbox()
    {
        // Set arrow hitbox
        hitbox.center = new Vector3(0, 1f, 0);
        hitbox.radius = 0.3f;
        hitbox.height = 1.8f;
        hitbox.direction = 1;

        // Set Fire hitbox
        fireHitbox.center = new Vector3(0, .15f, 0);
        fireHitbox.radius = 0.38f;
        fireHitbox.height = 3f;
        fireHitbox.direction = 1;
    }

    void MakeSound()
    {
        soundTimer = 5 + Random.Range(0, 10);
        int audio_idx = Random.Range(0, sounds.Length);
        zombie_audio.clip = sounds[audio_idx];
        zombie_audio.Play();
    }

    protected override void Destroy()
    {
        Disabled();
    }
}
