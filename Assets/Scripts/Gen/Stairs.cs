using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stairs : MonoBehaviour
{
    Bounds bounds;


    private void Start()
    {
        bounds = GetComponent<BoxCollider2D>().bounds;
    }

    private void Update()
    {
        var list = Physics2D.OverlapBoxAll(transform.position, Vector2.one, 0f);
        foreach (var item in list)
        {
            if(item.transform.parent.gameObject.name.Contains("Player"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            Debug.Log(item.gameObject.name);
        }
        if (bounds.Contains(Player.Instance.transform.position))
        {
            Debug.Log("Collision!!!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("Collision!!!");
    //    if (collision.transform.parent.CompareTag("Player"))
    //        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("Collision!!!");
    //    if (collision.transform.parent.gameObject.tag == "Player")
    //        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}
}
