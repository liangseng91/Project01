using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnParticle : MonoBehaviour
{
    public void Spawn(string particleName) {
        GameObject particleToSpawn = Resources.Load<GameObject>("VFXPrefabs/" + particleName);

        Instantiate(particleToSpawn, transform.position, transform.rotation);
    }
}
