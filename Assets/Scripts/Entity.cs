using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float maxHealth;
    float health;

    public delegate void OnDamaged(float dmg, Vector3 point, Vector3 force);
    public OnDamaged Damaged;

    private void OnEnable() {
        health = maxHealth;

        Damaged += TakeDamage;
    }

    private void OnDisable() {
        Damaged -= TakeDamage;
    }

    void TakeDamage(float dmg, Vector3 point, Vector3 force){
        health -= dmg;

        Debug.Log(health);

        if(health <= 0){
            health = 0;
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
