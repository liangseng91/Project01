using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Skill")]
public class Skill : ScriptableObject
{
    public string abilityName;
    public Texture abilityIcon;
    public float animationDuration;
    public float cooldown;
    public int skill_SP;
    public float critRate;
    public float critDamage;
    public int knockbackValue;
}
