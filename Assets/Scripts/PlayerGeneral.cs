using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGeneral : MonoBehaviour
{
    [SerializeField] float startingHealth;
    [SerializeField] float knockbackDistance;

    Rigidbody2D rb2d;
    PlayerMovement playerMovement;

    public float CurrentHealth { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = startingHealth;
        playerMovement = GetComponent<PlayerMovement>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        DeathCheck();
    }

    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        CurrentHealth -= damage;
        playerMovement.canMove = false;
        playerMovement.Stop();

        Vector3 dirFromAttacker = (transform.position - attackerPosition).normalized;
        rb2d.velocity = dirFromAttacker * knockbackDistance;
        
        Debug.Log($"Player took {damage} and went from {CurrentHealth + damage}Hp to {CurrentHealth}. ");

        playerMovement.canMove = true;
    }

    void DeathCheck()
    {
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has died. ");
    }
}
