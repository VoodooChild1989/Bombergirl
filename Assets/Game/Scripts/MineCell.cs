using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public enum OreType
{
    None, Coal, Crystal, Gemstone
}

public class PlatformerCell : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public string cellName;
            public int health;
            public GameObject prefab;
            public GameObject destructionVFX;
            [ShowOnly] public OreType curOreType;
            [ShowOnly] public int posX;
            [ShowOnly] public int posY;

    #endregion
    
    #region CUSTOM METHODS

        public void DamageCell(int damageAmount)
        {
            health -= damageAmount;

            if(health <= 0) DestroyCell();
        }

        [ContextMenu("DestroyCell")]
        public void DestroyCell()
        {
            if(cellName == "Empty") DataManager.instance.AddCoins(1);
            else if(cellName == "Coal") DataManager.instance.AddCoins(2);
            else if(cellName == "Crystal") DataManager.instance.AddCoins(4);
            else if(cellName == "Gemstone") DataManager.instance.AddCoins(6);

            FindObjectOfType<CellularAutomataAlgorithm>().RemoveCell(posX, posY, destructionVFX);
        }

    #endregion

}