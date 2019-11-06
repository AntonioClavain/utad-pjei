using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;

    public string[] tagsMakeDamage;

    public string[] tagsHealing;

    public delegate void OnHealthChanged(float currentHealth);
    public OnHealthChanged healthDelegate;

    public delegate void OnCharacterDead();
    public OnCharacterDead deathDelegate;

    private float _currentHealth;

    void Start()
    {
        _currentHealth = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        _currentHealth -= damageAmount;

        if(_currentHealth <= 0)
        {
            _currentHealth = 0;
            Death();
        }

        healthDelegate.Invoke(_currentHealth / maxHealth);
    }

    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;

        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);

        healthDelegate.Invoke(_currentHealth / maxHealth);
    }

    public void Death()
    {
        deathDelegate.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach(string tag in tagsMakeDamage)
        {
            if (other.CompareTag(tag))
            {
                DamageData data = other.GetComponent<DamageData>();
                Damage(data.damageAmount);
                return;
            }
        }

        foreach (string tag in tagsHealing)
        {
            if (other.CompareTag(tag))
            {
                HealData data = other.GetComponent<HealData>();
                Heal(data.healingAmount);
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (string tag in tagsMakeDamage)
        {
            if (collision.CompareTag(tag))
            {
                DamageData data = collision.GetComponent<DamageData>();
                Damage(data.damageAmount);
                return;
            }
        }

        foreach (string tag in tagsHealing)
        {
            if (collision.CompareTag(tag))
            {
                HealData data = collision.GetComponent<HealData>();
                Heal(data.healingAmount);
                return;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (string tag in tagsMakeDamage)
        {
            if (collision.transform.CompareTag(tag))
            {
                DamageData data = collision.transform.GetComponent<DamageData>();
                Damage(data.damageAmount);
                return;
            }
        }

        foreach (string tag in tagsHealing)
        {
            if (collision.transform.CompareTag(tag))
            {
                HealData data = collision.transform.GetComponent<HealData>();
                Heal(data.healingAmount);
                return;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (string tag in tagsMakeDamage)
        {
            if (collision.transform.CompareTag(tag))
            {
                DamageData data = collision.transform.GetComponent<DamageData>();
                Damage(data.damageAmount);
                return;
            }
        }

        foreach (string tag in tagsHealing)
        {
            if (collision.transform.CompareTag(tag))
            {
                HealData data = collision.transform.GetComponent<HealData>();
                Heal(data.healingAmount);
                return;
            }
        }
    }
}
