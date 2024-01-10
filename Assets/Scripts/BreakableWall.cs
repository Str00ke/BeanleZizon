using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public LayerMask hitLayers;
    public int life = 5;
    public GameObject wallGo = null;
    public GameObject destroyedWallGo = null;

    // Start is called before the first frame update
    void Start()
    {
        if (wallGo == null || destroyedWallGo == null)
            return;

        wallGo.SetActive(true);
        destroyedWallGo.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ApplyHit(Attack attack)
    {
        life -= (attack != null ? attack.damages : 1);
        if (life <= 0)
        {
            if (wallGo == null || destroyedWallGo == null)
                return;

            destroyedWallGo.SetActive(true);
            wallGo.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((hitLayers & (1 << collision.gameObject.layer)) == (1 << collision.gameObject.layer))
        {
            Debug.Log("Hit");
            // Collided with hitbox
            Attack attack = collision.gameObject.GetComponent<Attack>();
            ApplyHit(attack);
        }
    }
}
