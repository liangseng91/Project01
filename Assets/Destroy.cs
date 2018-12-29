using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {

    public float destroyDelay;

    void Update()
    {
        Destroy(gameObject, destroyDelay);
    }
}
