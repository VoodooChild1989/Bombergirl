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
            if(cellName == "Empty")
            {
                DataManager.instance.emptyCell++;
            }       
            else if(cellName == "Iron")
            {
                DataManager.instance.ironCell++;
            }      
            else if(cellName == "Metal")
            {
                DataManager.instance.metalCell++;
            }      
            else if(cellName == "Gold")
            {
                DataManager.instance.goldCell++;
            }      

            FindObjectOfType<CellularAutomataAlgorithm>().RemoveCell(posX, posY, destructionVFX);
        }

    #endregion

}