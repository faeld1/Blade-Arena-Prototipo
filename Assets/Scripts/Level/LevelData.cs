using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    public List<RoundData> rounds = new List<RoundData>();
}

[System.Serializable]
public class RoundData
{
    public List<GameObject> enemies = new List<GameObject>();
}
