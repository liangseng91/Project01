using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            isAttacking = true;
        }
        else if (col.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(col.collider, GetComponent<Collider2D>());
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        isAttacking = false;
    }
}
