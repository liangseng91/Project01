using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isAttacking = true;
        }
        else if (col.gameObject.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        isAttacking = false;
    }

}
