using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Basic Attack")]
public class BasicAttack : ScriptableObject
{
    public float animationDuration;
    public float cooldown;
    public float critRate;
    public float critDamage;
    public int knockbackValue;
}
