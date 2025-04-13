using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityPooling : MonoBehaviour
{
    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]

        public List<GameObject> pooledObjects;
        public GameObject box;
        public static VisibilityPooling instance;

        [Header("Debug")]
        public bool debugLogs = false;
        public float checkInterval = 1f;
        public float activationDistance = 20f;

    #endregion

    #region LIFECYCLE

        private void Awake()
        {
            SingletonUtility.MakeSingleton(ref instance, this, false);
        }

        private void Start()
        {
            StartCoroutine(CheckVisibilityRoutine());
        }

    #endregion

    #region METHODS

        IEnumerator CheckVisibilityRoutine()
        {
            while (true)
            {
                // CheckObjects();
                Mapper();
                yield return new WaitForSeconds(checkInterval);
            }
        }

        private void CheckObjects()
        {
            if (pooledObjects == null || pooledObjects.Count == 0) return;

            for (int i = pooledObjects.Count - 1; i >= 0; i--)
            {
                GameObject obj = pooledObjects[i];

                if (obj == null)
                {
                    pooledObjects.RemoveAt(i);
                    continue;
                }

                bool shouldBeActive = IsNearCamera(obj.transform.position);

                if (obj.activeInHierarchy != shouldBeActive)
                {
                    obj.SetActive(shouldBeActive);

                    if (debugLogs)
                        Debug.Log($"Set {obj.name} active: {shouldBeActive}");
                }
            }
        }

        void Mapper()
        {
            CellularAutomataAlgorithm CA = CellularAutomataAlgorithm.instance;
            
            for (int i = 0; i < CA.width; i++)
            {
                for (int j = 0; j < CA.height; j++)
                {
                    if(CA._grid[i,j] == 0) continue;

                    Vector3 worldPos = new Vector3(i - (CA.width / 2) + CA.offsetX, j - CA.height + CA.offsetY, 0);

                    if(IsNearCamera(worldPos))
                    {
                        if((CA._grid[i,j] == 1) && (CA._isUsedGrid[i,j] == 0)) 
                        {
                            //Instantiate(box, worldPos, Quaternion.identity);
                            
                            if((i >= 1) && (i <= CA.width - 2) && (j >= 1) && (j <= CA.height - 2) && (!CA.IsFull(i-1,j)) && (!CA.IsFull(i+1,j)) && (!CA.IsFull(i,j-1)) && (!CA.IsFull(i,j+1)))
                            {
                                CA._currentGrid[i,j] = Instantiate(CA.cells[1].prefab, worldPos, Quaternion.identity); 
                            }
                            else if((i >= 1) && (i <= CA.width - 2) && (j >= 1) && (j <= CA.height - 2) && (!CA.IsFull(i-1,j+1)) && (!CA.IsFull(i,j+1)) && (!CA.IsFull(i-1,j)))
                            {
                                CA._currentGrid[i,j] = Instantiate(CA.cells[2].prefab, worldPos, Quaternion.identity); 
                            }
                            else if((i >= 1) && (i <= CA.width - 2) && (j >= 1) && (j <= CA.height - 2) && (!CA.IsFull(i,j+1)) && (!CA.IsFull(i+1,j)) && (!CA.IsFull(i+1,j+1)))
                            {
                                CA._currentGrid[i,j] = Instantiate(CA.cells[3].prefab, worldPos, Quaternion.identity); 
                            }
                            else if((i >= 1) && (i <= CA.width - 2) && (j >= 1) && (j <= CA.height - 2) && (!CA.IsFull(i-1,j)) && (!CA.IsFull(i-1,j-1)) && (!CA.IsFull(i,j-1)))
                            {
                                CA._currentGrid[i,j] = Instantiate(CA.cells[7].prefab, worldPos, Quaternion.identity); 
                            }
                            else if((i >= 1) && (i <= CA.width - 2) && (j >= 1) && (j <= CA.height - 2) && (!CA.IsFull(i,j-1)) && (!CA.IsFull(i+1,j)) && (!CA.IsFull(i+1,j-1)))
                            {
                                CA._currentGrid[i,j] = Instantiate(CA.cells[8].prefab, worldPos, Quaternion.identity); 
                            }
                            else if((i >= 1) && (i <= CA.width - 2) && (j >= 1) && (j <= CA.height - 2) && (!CA.IsFull(i,j+1)))
                            {
                                CA._currentGrid[i,j] = Instantiate(CA.cells[4].prefab, worldPos, Quaternion.identity); 
                            }
                            else if((i >= 1) && (i <= CA.width - 2) && (j >= 1) && (j <= CA.height - 2) && (!CA.IsFull(i-1,j)))
                            {
                                CA._currentGrid[i,j] = Instantiate(CA.cells[5].prefab, worldPos, Quaternion.identity); 
                            }
                            else if((i >= 1) && (i <= CA.width - 2) && (j >= 1) && (j <= CA.height - 2) && (!CA.IsFull(i+1,j)))
                            {
                                CA._currentGrid[i,j] = Instantiate(CA.cells[6].prefab, worldPos, Quaternion.identity); 
                            }
                            else if((i >= 1) && (i <= CA.width - 2) && (j >= 1) && (j <= CA.height - 2) && (!CA.IsFull(i,j-1)))
                            {
                                CA._currentGrid[i,j] = Instantiate(CA.cells[9].prefab, worldPos, Quaternion.identity); 
                            }
                            else
                            {
                                CA._currentGrid[i,j] = Instantiate(CA.cells[0].prefab, worldPos, Quaternion.identity); 
                            }
                            
                            // pooledObjects.Add(_currentGrid[i,j]);
                            CA._isUsedGrid[i,j] = 1;

                            PlatformerCell cellScript = CA._currentGrid[i,j].GetComponent<PlatformerCell>();
                            cellScript.posX = i;
                            cellScript.posY = j;
                            // _currentGrid[i,j].transform.SetParent(GameObject.Find("Cells (Collection)").transform);
                        }
                        else if((CA._grid[i,j] == 1) && (CA._isUsedGrid[i,j] == 1)) 
                        {
                            CA._currentGrid[i, j].SetActive(true);
                        }
                    }
                    else
                    {
                        if ((CA._currentGrid[i, j] != null) && (CA._currentGrid[i, j].activeInHierarchy))
                            CA._currentGrid[i, j].SetActive(false);
                    }
                }
            }
        }

        private bool IsNearCamera(Vector3 pos)
        {
            if (Camera.main == null) return false;

            float distance = Vector2.Distance(Camera.main.transform.position, pos);
            return distance <= activationDistance;
        }

    #endregion
}