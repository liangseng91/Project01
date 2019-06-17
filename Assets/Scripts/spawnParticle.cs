using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticle : MonoBehaviour
{
    public void Spawn(string particleName) {
        GameObject particleToSpawn = Resources.Load<GameObject>("VFXPrefabs/" + particleName);
        GameObject particleSpawned = Instantiate(particleToSpawn, transform.position, transform.rotation);
        particleSpawned.transform.parent = gameObject.transform;
    }
}
