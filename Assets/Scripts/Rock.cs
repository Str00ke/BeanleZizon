using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public LayerMask hitLayers;
    public GameObject attackPrefab = null;

    private void ApplyHit(Attack attack)
    {
        if (attack == null)
            return;

        if (attack.element == Element.Earth)
        {
            if (attackPrefab == null)
                return;

            Destroy(gameObject);
            GameObject.Instantiate(attackPrefab, transform.position, attack.transform.rotation);
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
