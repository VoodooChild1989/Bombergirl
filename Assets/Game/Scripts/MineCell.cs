using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class PlatformerCell : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public string cellName;
            public float chance;
            public int health;
            public int posX;
            public int posY;
            public GameObject prefab;
            public GameObject destructionVFX;

    #endregion
    
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

        CellularAutomataAlgorithm.instance.RemoveCell(posX, posY, destructionVFX);
    }

}