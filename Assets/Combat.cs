using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

    private bool keyPressed = false;
    Animator m_Animator;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (m_Animator.GetInteger("attackCombo") == 0) {
            if (Input.GetKeyDown("q"))
            {
                m_Animator.SetTrigger("Attack");
            }
        }
    }

    public void WaitCombo() {
        StartCoroutine(WaitInput());
    }

    private IEnumerator WaitInput() {
        while (!keyPressed) {
            if (Input.GetKeyDown("q")) {
                NextCombo();
                break;
            }
        }
        yield return 0;
    }

    private void NextCombo() {
        keyPressed = true;
        int i = m_Animator.GetInteger("attackCombo");
        m_Animator.SetInteger("attackCombo", i + 1);
        
    }

    public void ResetCombo()
    {
        m_Animator.SetInteger("attackCombo", 0);

    }
}
