//Script by Andy Noworol /twitter => @andynoworol
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadZombie : MonoBehaviour {

    public int animState = 1;

    private void Awake() {
        GetComponent<Animator>().SetInteger("State", animState);
    }
}
