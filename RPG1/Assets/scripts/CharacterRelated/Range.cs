using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour
{

    private Enemy parent;

    private void Start()
    {
        parent = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision) //düşmana etrafına eklenen trigger
    {
        if (collision.tag == "Player")
        {
            parent.SetTarget(collision.GetComponent<Character>());
        }
    }

}
