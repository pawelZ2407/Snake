using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Settings", order = 1)]
public class SettingsSO : ScriptableObject
{
    public string obstacleTag;
    public string playerTag;
    public string scoreTag;

    public float moveTickSpeed;

}
