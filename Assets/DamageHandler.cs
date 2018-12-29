using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour {

    void Attack(Object objectName) {
        Instantiate(objectName,transform.position, transform.rotation);
    }
}
