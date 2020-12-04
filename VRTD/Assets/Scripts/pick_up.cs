using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class pick_up : MonoBehaviour
    {
        public GameObject weapon;
        public Vector3 spawnPoint;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void interact()
        {
            Instantiate(this, spawnPoint, Quaternion.identity);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
