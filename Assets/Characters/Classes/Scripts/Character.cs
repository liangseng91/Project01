using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    [SerializeField] private float movementSpeed;
    private HPStatus health;
    [SerializeField] private float initialHP;

    Rigidbody2D myRigidBody;
    Animator animator;

    protected bool isAttacking;

    void Start ()
    {
        isAttacking = false;
        myRigidBody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        // TODO set initial current & max HP
    }

    void Update()
    {
        Action();
    }

    public void Action()
    {
        if (isAttacking == false)
        {
            myRigidBody.velocity = new Vector2(1 * movementSpeed, myRigidBody.velocity.y); // TODO if Enemy, move to left
            animator.SetTrigger("Walk");
        }
        else if (isAttacking == true)
        {
            myRigidBody.velocity = Vector3.zero;
            animator.SetTrigger("Attack");
        }
    }

}
