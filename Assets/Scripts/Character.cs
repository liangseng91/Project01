using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Player")]
public class Character : ScriptableObject
{
    public Texture char_Icon;
    public string char_Name;
    public int char_maxHP;
    public int char_maxSP;
    public int char_ATK;
    public int char_DEF;
    public float char_CDSpd;
    public float char_ATKSpd;
    public int char_Level;
    public int char_EXP;
}
