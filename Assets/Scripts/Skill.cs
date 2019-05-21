using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Skill")]
public class Skill : ScriptableObject
{
    public string abilityName;
    public Texture abilityIcon;
    public AnimationClip Animation;
    public float animationDuration;
    public float cooldown;
    
}
