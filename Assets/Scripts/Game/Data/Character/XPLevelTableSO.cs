using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data.Character
{
    [CreateAssetMenu(menuName = "Data/Character/XP Level Table")]
    public class XPLevelTableSO : ScriptableObject
    {
        public List<double> xpPerLevel;
    }
}
