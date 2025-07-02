using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class EnemyGroundChecker : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public int field;
            private int objectsInTrigger = 0;

    #endregion

    #region CUSTOM METHODS

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Cell")) objectsInTrigger++;
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Cell"))
            {
                objectsInTrigger--;

                if (objectsInTrigger <= 0)
                {
                    transform.parent.GetComponent<Enemy>().Flip();
                }
            }
        }

    #endregion

}