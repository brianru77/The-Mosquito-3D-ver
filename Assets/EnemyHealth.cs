using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public event System.Action OnDeath;
    public int maxHP = 5;
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }
    void Update()
    {
        Debug.Log("현재 모기 체력" + currentHP);
    }
    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        if (currentHP == 0)
        {
            Die();
        }
    }

    void Die()
    {
        OnDeath?.Invoke(); //모기 죽으면
        Destroy(gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            WeaponDamage weapon = other.GetComponent<WeaponDamage>();
            int damageAmount = (weapon != null) ? weapon.damage : 1;
            TakeDamage(damageAmount);
        }
    }
}
