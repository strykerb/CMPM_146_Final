using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An object to load in at random points to serve as a spatial audio source for stress
public class SpatialStressor : MonoBehaviour
{
    AudioSource source;
    public AudioClip clip;
    float timeToExist = 10.0f;
    float timeAlive = 0.0f;
    bool hasActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Spatial stressor created!");
        float radius = Random.Range(10.0f, 16.0f);
        float angle = Random.Range(0.0f, 2.0f * Mathf.PI);
        gameObject.transform.Translate(new Vector3(Mathf.Cos(angle) * radius, 2.5f, Mathf.Sin(angle) * radius));
        gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // We need to wait until our first frame update for the stressor to actually activate.
        if (!hasActivated)
        {
            Activate();
        }

        timeAlive += Time.deltaTime;
        if (timeAlive > timeToExist)
        {
            Destroy(gameObject);
        }
    }

    public void Activate()
    {
        source = GetComponentInParent<AudioSource>();
        source.spatialize = true;
        source.PlayOneShot(clip, 0.5f);
        timeToExist = clip.length;
        hasActivated = true;
    }
}
