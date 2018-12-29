using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateDamage : MonoBehaviour {

    public float baseDamage;
    public float dmgMultiplier;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
    }

    public int CalculateFinal()
    {
        float finalDamage = baseDamage * dmgMultiplier;
        int finalDamage_Rounded = Mathf.RoundToInt(finalDamage);
        return finalDamage_Rounded;
    }
}
