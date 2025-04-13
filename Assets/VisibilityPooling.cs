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
                Mapper();
                CheckObjects();
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
                if(!IsNearCamera(obj.transform.position))
                {
                    Destroy(obj);
                    pooledObjects.RemoveAt(i);
                    continue;
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
                    Vector3 worldPos = new Vector3(i - (CA.width / 2) + CA.offsetX, j - CA.height + CA.offsetY, 0);
                    
                    if(CA._grid[i,j] == 0) 
                    {
                        // Adding enemies
                        if((IsNearCamera(worldPos)) && (CA._isUsedGrid[i,j] == 0) && (i >= 1) && (i <= CA.width - 2) && (j >= 1) && (j <= CA.height - 2))
                        {
                            if(CA.IsValue(0.3f)) 
                            {   
                                CA._isUsedGrid[i,j] = 1;
                            }

                            // Ground enemies
                            if((!CA.IsFull(i-1,j)) && (!CA.IsFull(i,j)) && (!CA.IsFull(i+1,j)) && (CA.IsFull(i-1,j-1)) && (CA.IsFull(i,j-1)) && (CA.IsFull(i+1,j-1)))
                            {
                                if(CA.IsValue(0.15f)) 
                                {
                                    int randIndex = UnityEngine.Random.Range(0, CA.groundEnemies.Length);
                                    GameObject enemyInstance = Instantiate(CA.groundEnemies[randIndex], worldPos, Quaternion.identity);
                                    enemyInstance.name = CA.groundEnemies[randIndex].name;
                                    ProjectilePooling.instance.percyCounter++;
                                    
                                    CA._isUsedGrid[i,j] = 1;

                                    pooledObjects.Add(enemyInstance);
                                }
                            } // Air enemies
                            else if(CA.CountingFullNeighbours(i, j) == 0)
                            {   
                                if(CA.IsValue(0.01f)) 
                                {
                                    int randIndex = UnityEngine.Random.Range(0, CA.airEnemies.Length);
                                    GameObject enemyInstance = Instantiate(CA.airEnemies[randIndex], worldPos, Quaternion.identity);
                                    enemyInstance.name = CA.airEnemies[randIndex].name;
                                    ProjectilePooling.instance.fairyCounter++;
                                    
                                    CA._isUsedGrid[i,j] = 1;
                                    
                                    pooledObjects.Add(enemyInstance);
                                }
                            }
                        }

                        continue;
                    }

                    if(IsNearCamera(worldPos))
                    {
                        if((CA._grid[i,j] == 1) && (CA._isUsedGrid[i,j] == 0)) 
                        {
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
                            
                            CA._isUsedGrid[i,j] = 1;

                            PlatformerCell cellScript = CA._currentGrid[i,j].GetComponent<PlatformerCell>();
                            cellScript.health = (CA.height - j) / 10 + 1;
                            cellScript.posX = i;
                            cellScript.posY = j;
                            CA._currentGrid[i,j].transform.SetParent(GameObject.Find("Cells (Collection)").transform);

                            // Ore type
                            float initRandomValue = UnityEngine.Random.value;
                            if(initRandomValue <= 0.3f)
                            {
                                float randomValue = UnityEngine.Random.value;

                                if(randomValue <= 0.3f)
                                {
                                    cellScript.curOreType = OreType.Coal;
                                    cellScript.cellName = "Coal";
                                    CA.AddOre(CA._currentGrid[i,j], cellScript.curOreType, worldPos);
                                }   
                                else if(randomValue <= 0.5f)
                                {
                                    cellScript.curOreType = OreType.Crystal;
                                    cellScript.cellName = "Crystal";
                                    CA.AddOre(CA._currentGrid[i,j], cellScript.curOreType, worldPos);
                                }
                                else if(randomValue <= 0.7f)
                                {
                                    cellScript.curOreType = OreType.Gemstone;
                                    cellScript.cellName = "Gemstone";
                                    CA.AddOre(CA._currentGrid[i,j], cellScript.curOreType, worldPos);
                                }
                            }

                            // Adding grass
                            if((j < CA.height - 1) && (CA._grid[i,j+1] == 0))
                            {
                                CA.AddDecor(CA._currentGrid[i,j], CA.grass, worldPos);

                                int randomIndexDetail = UnityEngine.Random.Range(1, 8);
                                Vector3 posDetail = new Vector3(i - (CA.width / 2) + CA.offsetX, j - CA.height + CA.offsetY + 1, 0);

                                if(randomIndexDetail == 1)
                                {
                                    CA.AddDecor(CA._currentGrid[i,j], CA.grass_detail_1, posDetail);
                                }
                                else if(randomIndexDetail == 2)
                                {
                                    CA.AddDecor(CA._currentGrid[i,j], CA.grass_detail_2, posDetail);
                                }
                            }
                            if((j > 0) && (CA._grid[i,j-1] == 0))
                            {
                                CA.AddDecor(CA._currentGrid[i,j], CA.edge_bottom, worldPos);
                            }
                            if((i < CA.width - 1) && (CA._grid[i+1,j] == 0))
                            {
                                CA.AddDecor(CA._currentGrid[i,j], CA.edge_right, worldPos);
                            }
                            if((i > 0) && (CA._grid[i-1,j] == 0))
                            {
                                CA.AddDecor(CA._currentGrid[i,j], CA.edge_left, worldPos);
                            }
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