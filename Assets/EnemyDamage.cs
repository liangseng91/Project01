using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour {

    public int finalDamage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnTriggerEnter(Collider col)
    {
        GameObject obj = col.gameObject;
        if (!obj.GetComponent<CalculateDamage>() && obj.gameObject.tag != "PlayerHit")
        {
            return;
        }

        finalDamage = obj.GetComponent<CalculateDamage>().CalculateFinal();

    }

}
