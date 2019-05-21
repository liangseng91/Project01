using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Basic Attack")]
public class BasicAttack : ScriptableObject
{
    public AnimationClip Animation;
    public float animationDuration;
    public float cooldown;

}
