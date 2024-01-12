using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public LayerMask hitLayers;
    public float timeBetweenExplosion = 4.0f;
    public float timeBeforeExplosion = 0.2f;
    public GameObject attackPrefab = null;
    private float timer = 0f;
    private bool activated = false;
    private bool canExpload = true;

    private void Update()
    {
        //Time only advence when barrel is activated or is on cooldown
        if (activated || !canExpload)
            timer += Time.deltaTime;

        //Expload only if can :D
        if (canExpload && timer >= timeBeforeExplosion)
            Explode();

        //Can expload aggain after somme time
        if (timer >= timeBetweenExplosion)
        {
            timer = 0f;
            canExpload = true;
        }
    }

    private void Explode()
    {
        Debug.Log("Boom");
        timer = 0f;
        activated = false;
        canExpload = false;

        if (attackPrefab == null)
            return;

        GameObject.Instantiate(attackPrefab, transform.position, transform.rotation);
    }

    private void ApplyHit(Attack attack)
    {
        if(attack == null) 
            return;

        if(attack.element == Element.Fire)
        {
            activated = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((hitLayers & (1 << collision.gameObject.layer)) == (1 << collision.gameObject.layer))
        {
            // Collided with hitbox
            Attack attack = collision.gameObject.GetComponent<Attack>();
            ApplyHit(attack);
        }
    }
}
