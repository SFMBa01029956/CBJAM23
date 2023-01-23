using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Parameters")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float lightHealthRegen = 0.1f;
    [SerializeField] private float darkHealthLoss = 0.1f;
    [SerializeField] private float healthChangeRate = 0.1f;
    
    
    [Header("Health Bar")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private float healthBarLength;
    [SerializeField] private float healthBarHeight;
    [SerializeField] private float healthBarX;
    [SerializeField] private float healthBarY;
    public static Action<float> OnTakeDamage;
    public static Action<float> OnDamage;
    public static Action<float> OnHeal;
    
    [Header("Light Range Parameters")]
    [SerializeField] private float lightRange = 5f;
    [SerializeField] private LayerMask lightMask;
    [SerializeField] private LayerMask obstructionMask;
    
    private Transform lightTransform;
    [SerializeField] private bool onLight;
    private bool dead;
    private Animator animator;
    PlayerController playerController;

    [SerializeField] private AudioSource damageSoundEffect;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        StartCoroutine(UpdateHealth());
        StartCoroutine(LightFOVRoutine());
    }

    private void OnEnable()
    {
        OnTakeDamage += ApplyDamage;
    }
    
    private void OnDisable()
    {
        OnTakeDamage -= ApplyDamage;
    }

    private void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        OnDamage?.Invoke(currentHealth);
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

    }
    
    private void ApplyHealing(float healing)
    {
        currentHealth += healing;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        OnHeal?.Invoke(currentHealth);
    }

    private void Die()
    {
        currentHealth = 0;
        dead = true;
        StopCoroutine(UpdateHealth());
        
        animator.SetTrigger("Die");
        playerController.LockMovement();
        // TODO CALL TO THE ANIMATIONS & STOP MOVEMENT
    }
    
    private IEnumerator UpdateHealth()
    {
        yield return new WaitForSeconds(5f);
        WaitForSeconds timeToWait = new WaitForSeconds(healthChangeRate);

        while (!dead)
        {
            if (onLight)
            {
                ApplyHealing(lightHealthRegen);
                damageSoundEffect.Stop();
            }
            else
            {
                ApplyDamage(darkHealthLoss);
                if (!damageSoundEffect.isPlaying)
                {
                    damageSoundEffect.Play();
                }
            }
                

            yield return timeToWait;
        }
    }
    
    
    private IEnumerator LightFOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        float range = lightRange;
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, range, lightMask);
        
        if (rangeChecks.Length != 0)
        {
            lightTransform = rangeChecks[0].transform;
            Vector2 directionToTarget = (lightTransform.transform.position - transform.position).normalized;
            
            float distanceToTarget = Vector2.Distance(transform.position, lightTransform.transform.position);

            if (distanceToTarget <= range)
            {
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    onLight = true;
                else
                {
                    onLight = false;
                }
            }
         
        }
        else
        {
            onLight = false;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lightRange);

        if (onLight)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, (lightTransform.position - transform.position).normalized * lightRange);    
        }
    }

}
