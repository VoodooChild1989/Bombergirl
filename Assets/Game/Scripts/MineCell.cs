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
            [ShowOnly] public SpriteRenderer sr;
            // [ShowOnly] public Rigidbody2D rb;
            [ShowOnly] public Collider2D col;
            [ShowOnly] public Camera camera;
            [ShowOnly] public bool onScreen;
            [ShowOnly] public OreType curOreType;
            [ShowOnly] public int posX;
            [ShowOnly] public int posY;

    #endregion
    
    #region CUSTOM METHODS

        void Awake()
        {
            sr = FindObjectOfType<SpriteRenderer>();
            // rb = FindObjectOfType<Rigidbody2D>();
            col = FindObjectOfType<Collider2D>();
            camera = FindObjectOfType<Camera>();
        }

        void FixedUpdate()
        {
            // OnScreen();
        }

        void OnScreen()
        {
            float buffer = 0.1f; // 10% wider than screen

            Vector3 screenPoint = camera.WorldToViewportPoint(transform.position);
            onScreen = screenPoint.z > 0 &&
                    screenPoint.x > -buffer && screenPoint.x < 1 + buffer &&
                    screenPoint.y > -buffer && screenPoint.y < 1 + buffer;

            SetPhysicsActive(onScreen);
        }

        public void SetPhysicsActive(bool isActive)
        {
            sr.enabled = isActive;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(isActive);
            }
        }

        public void DamageCell(int damageAmount)
        {
            health -= damageAmount;

            if(health <= 0) DestroyCell();
        }

        [ContextMenu("DestroyCell")]
        public void DestroyCell()
        {
            if(cellName == "Empty") DataManager.instance.AddCoins(5);
            else if(cellName == "Coal") DataManager.instance.AddCoins(10);
            else if(cellName == "Crystal") DataManager.instance.AddCoins(15);
            else if(cellName == "Gemstone") DataManager.instance.AddCoins(20);

            VisibilityPooling.instance.pooledObjects.Remove(gameObject);
            FindObjectOfType<CellularAutomataAlgorithm>().RemoveCell(posX, posY, destructionVFX);
        }

    #endregion

}