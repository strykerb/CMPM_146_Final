using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public abstract class Character : MonoBehaviour
{
    int Health;
    float Speed;
    
    private void Awake() => Initialize();

    private void OnSpawned() => Initialize();
    private void OnEnable() => Enabled();
    private void OnDisable() => Disabled();
    private void Update() => OnUpdate();
    private void OnDestroy() => Destroy();
    //...Any other callback functions I might need for whichever purpose this class has...

    protected virtual void Initialize() { }
    protected virtual void Enabled() { }
    protected virtual void Disabled() { }
    protected virtual void OnUpdate() { }
    protected virtual void Destroy() { }

    public int getHealth()
    {
        return Health;
    }

    public void setHealth(int value)
    {
        Health = value;
    }

    public float getSpeed()
    {
        return Speed;
    }
}
