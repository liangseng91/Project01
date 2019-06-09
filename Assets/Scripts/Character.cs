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
    public int char_SPRegen;
    public int char_Level;
    public int char_EXP;
    public int char_maxHPGainPerLevel;
    public int char_maxSPGainPerLevel;
    public int char_ATKGainPerLevel;
    public int char_DEFGainPerLevel;
}
