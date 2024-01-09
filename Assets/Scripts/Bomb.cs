using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int damages = 5;
    public float knockbackSpeed = 6;
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

        Destroy(gameObject);
    }
}
