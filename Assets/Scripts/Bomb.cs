using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject attackPrefab = null;
    public float timeToExplode = 5;

    private float time = 0;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= timeToExplode) 
        {
            Explode();
        }
    }

    private void Explode()
    {
        Debug.Log("Booom !!");
        if (attackPrefab == null)
            return;

        GameObject.Instantiate(attackPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
