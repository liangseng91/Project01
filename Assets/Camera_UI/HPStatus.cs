using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPStatus : MonoBehaviour {

    private Image healthBar;

    private float currentFill;

    void Start()
    {
        healthBar = GetComponent<Image>();
    }

    void Update()
    {
        healthBar.fillAmount = currentFill;
    }

}
