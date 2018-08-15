using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarFollow : MonoBehaviour {

    [SerializeField] GameObject playerHPBar = null;

	// Use this for initialization
	void Start () {
        Instantiate(playerHPBar, transform.position, Quaternion.identity, transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
