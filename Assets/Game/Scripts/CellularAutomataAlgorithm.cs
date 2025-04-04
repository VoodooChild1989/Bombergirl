using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// Cellular Automate is a  is a grid-based system where each cell has a state that evolves over discrete time steps according to a set of rules. 
/// The state of a cell at any given step depends on its previous state and the states of its neighboring cells.
/// </summary>
public class CellularAutomataAlgorithm : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Space(20)] [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public int width = 120;
            public int height = 80;
            private int[,] _grid;
            public float chanceOfFull = 0.5f;
            public int numberOfSmoothings = 5;
            public int seed = 1;
            public bool useRandomSeed = false;
            public static CellularAutomataAlgorithm instance;

            [Header("Extra")]
            public float offsetX = 0.5f;
            public float offsetY = 0.5f;
            public PlatformerCell[] cells;
            private GameObject[,] _currentGrid;

            [Header("Ores")]
            public GameObject coalObj;
            public GameObject crystalObj;
            public GameObject gemstoneObj;

            [Header("Decorations")]
            public GameObject grass;
            public GameObject grass_detail_1;
            public GameObject grass_detail_2;
            public GameObject edge_left;
            public GameObject edge_right;
            public GameObject edge_bottom;

            [Header("Camera Shake")]
            public CinemachineImpulseSource impulseSource;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        void Awake()
        {
            SingletonUtility.MakeSingleton(ref instance, this);
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        void Start()
        {
            InitializeSeed();
            InitializingMap();
            SmoothingMap();
            InstantiatingMap();
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.R)) 
            {
                InitializeSeed();
                InitializingMap();
                SmoothingMap();
                InstantiatingMap();
            }

            if(Input.GetKeyDown(KeyCode.T)) 
            {
                //RefreshingMap();
            }
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        void FixedUpdate()
        {
            // Add physics-related logic here.
            // Example: Rigidbody movement, applying forces, or collision detection.
        }

    #endregion

    #region CUSTOM METHODS

        /// <summary>
        /// Setting up a seed.
        /// </summary>
        void InitializeSeed()
        {
            if(useRandomSeed) seed = System.Guid.NewGuid().GetHashCode();
            
            UnityEngine.Random.InitState(seed);
        }

        /// <summary>
        /// Setting up a map.
        /// </summary>
        void InitializingMap()
        {    
            // Deleting the previous grid
            if (_currentGrid != null)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (_currentGrid[i, j] != null)
                        {
                            RemoveCell(i, j);
                        }
                    }
                }
            }

            // Setting up a new grid
            _grid = new int[width, height];
            _currentGrid = new GameObject[width, height];

            for(int i = 0; i < width; i++)
            {            
                for(int j = 0; j < height; j++)
                {
                    if((i == 0) || (i == width - 1) || (j == 0) || (j == height - 1))
                    {
                        _grid[i,j] = 1;  
                    }
                    else
                    {
                        _grid[i,j] = UnityEngine.Random.value < chanceOfFull ? 1 : 0; 
                    }
                }   
            }    
        }

        /// <summary>
        /// Smoothing the grid cells based on their neighbours.
        /// </summary>
        void SmoothingMap()
        {
            for(int k = 0; k < numberOfSmoothings; k++)
            {
                for(int i = 0; i < width; i++)
                {            
                    for(int j = 0; j < height; j++)
                    {
                        if((i == 0) || (i == width - 1) || (j == 0) || (j == height - 1)) continue;

                        if(CountingFullNeighbours(i, j) > 4) 
                        {
                            _grid[i,j] = 1;
                        }
                        else if(CountingFullNeighbours(i, j) < 4) 
                        {
                            _grid[i,j] = 0;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Counting the number of neighbouring cells that are full.
        /// </summary>
        /// <param name="x">The X coordinate of a cell.</param>
        /// <param name="y">The y coordinate of a cell.</param>
        int CountingFullNeighbours(int x, int y)
        {
            int count = 0;

            for(int i = x - 1; i <= x + 1; i++)
            {            
                for(int j = y - 1; j <= y + 1; j++)
                {
                    // We don't consider the cell itself
                    if((i == x) && (j == y)) continue;
             
                    // We don't consider the cell that are out of bounds
                    if((i < 0) || (i >= width) || (j < 0) || (j >= height)) continue;

                    count += _grid[i,j];
                }
            }

            return count;
        }

        void InstantiatingMap()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if(_grid[i,j] == 0) continue;
                    
                    Vector3 pos = new Vector3(i - (width / 2) + offsetX, j - height + offsetY, 0);

                    // Cell type
                    if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i-1,j)) && (!IsFull(i+1,j)) && (!IsFull(i,j-1)) && (!IsFull(i,j+1)))
                    {
                        _currentGrid[i,j] = Instantiate(cells[1].prefab, pos, Quaternion.identity); 
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i-1,j+1)) && (!IsFull(i,j+1)) && (!IsFull(i-1,j)))
                    {
                        _currentGrid[i,j] = Instantiate(cells[2].prefab, pos, Quaternion.identity); 
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i,j+1)) && (!IsFull(i+1,j)) && (!IsFull(i+1,j+1)))
                    {
                        _currentGrid[i,j] = Instantiate(cells[3].prefab, pos, Quaternion.identity); 
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i-1,j)) && (!IsFull(i-1,j-1)) && (!IsFull(i,j-1)))
                    {
                        _currentGrid[i,j] = Instantiate(cells[7].prefab, pos, Quaternion.identity); 
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i,j-1)) && (!IsFull(i+1,j)) && (!IsFull(i+1,j-1)))
                    {
                        _currentGrid[i,j] = Instantiate(cells[8].prefab, pos, Quaternion.identity); 
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i,j+1)))
                    {
                        _currentGrid[i,j] = Instantiate(cells[4].prefab, pos, Quaternion.identity); 
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i-1,j)))
                    {
                        _currentGrid[i,j] = Instantiate(cells[5].prefab, pos, Quaternion.identity); 
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i+1,j)))
                    {
                        _currentGrid[i,j] = Instantiate(cells[6].prefab, pos, Quaternion.identity); 
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i,j-1)))
                    {
                        _currentGrid[i,j] = Instantiate(cells[9].prefab, pos, Quaternion.identity); 
                    }
                    else
                    {
                        _currentGrid[i,j] = Instantiate(cells[0].prefab, pos, Quaternion.identity); 
                    }

                    // Saving the position
                    PlatformerCell cellScript = _currentGrid[i,j].GetComponent<PlatformerCell>();
                    cellScript.posX = i;
                    cellScript.posY = j;
                    _currentGrid[i,j].transform.SetParent(GameObject.Find("Cells (Collection)").transform);

                    // Ore type
                    float initRandomValue = UnityEngine.Random.value;
                    if(initRandomValue <= 0.3f)
                    {
                        float randomValue = UnityEngine.Random.value;

                        if(randomValue <= 0.3f)
                        {
                            cellScript.curOreType = OreType.Coal;
                            AddOre(_currentGrid[i,j], cellScript.curOreType, pos);
                        }   
                        else if(randomValue <= 0.5f)
                        {
                            cellScript.curOreType = OreType.Crystal;
                            AddOre(_currentGrid[i,j], cellScript.curOreType, pos);
                        }
                        else if(randomValue <= 0.7f)
                        {
                            cellScript.curOreType = OreType.Gemstone;
                            AddOre(_currentGrid[i,j], cellScript.curOreType, pos);
                        }
                    }

                    // Adding grass
                    if((j < height - 1) && (_grid[i,j+1] == 0))
                    {
                        AddDecor(_currentGrid[i,j], grass, pos);
                    
                        int randomIndexDetail = UnityEngine.Random.Range(1, 4);
                        Vector3 posDetail = new Vector3(i - (width / 2) + offsetX, j - height + offsetY + 1, 0);

                        if(randomIndexDetail == 1)
                        {
                            AddDecor(_currentGrid[i,j], grass_detail_1, posDetail);
                        }
                        else if(randomIndexDetail == 2)
                        {
                            AddDecor(_currentGrid[i,j], grass_detail_2, posDetail);
                        }
                    }
                    if((j > 0) && (_grid[i,j-1] == 0))
                    {
                        AddDecor(_currentGrid[i,j], edge_bottom, pos);
                    }
                    if((i < width - 1) && (_grid[i+1,j] == 0))
                    {
                        AddDecor(_currentGrid[i,j], edge_right, pos);
                    }
                    if((i > 0) && (_grid[i-1,j] == 0))
                    {
                        AddDecor(_currentGrid[i,j], edge_left, pos);
                    }       
                }
            }
        }

        void RefreshingMap(int x, int y)
        {
            for(int i = x - 1; i <= x + 1; i++)
            {            
                for(int j = y - 1; j <= y + 1; j++)
                {
                    if((i < 0) || (i >= width) || (j < 0) || (j >= height) || (_currentGrid[i,j] == null)) continue;

                    Vector3 pos = new Vector3(i - (width / 2) + offsetX, j - height + offsetY, 0);
                    SpriteRenderer sr = _currentGrid[i,j].GetComponent<SpriteRenderer>();
                    
                    // Cell type
                    if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i-1,j)) && (!IsFull(i+1,j)) && (!IsFull(i,j-1)) && (!IsFull(i,j+1)))
                    {
                        SpriteRenderer srPrefab = cells[1].prefab.GetComponent<SpriteRenderer>();
                        sr.sprite = srPrefab.sprite;
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i-1,j+1)) && (!IsFull(i,j+1)) && (!IsFull(i-1,j)))
                    {
                        SpriteRenderer srPrefab = cells[2].prefab.GetComponent<SpriteRenderer>();
                        sr.sprite = srPrefab.sprite;
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i,j+1)) && (!IsFull(i+1,j)) && (!IsFull(i+1,j+1)))
                    {
                        SpriteRenderer srPrefab = cells[3].prefab.GetComponent<SpriteRenderer>();
                        sr.sprite = srPrefab.sprite;
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i-1,j)) && (!IsFull(i-1,j-1)) && (!IsFull(i,j-1)))
                    {
                        SpriteRenderer srPrefab = cells[7].prefab.GetComponent<SpriteRenderer>();
                        sr.sprite = srPrefab.sprite;
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i,j-1)) && (!IsFull(i+1,j)) && (!IsFull(i+1,j-1)))
                    {
                        SpriteRenderer srPrefab = cells[8].prefab.GetComponent<SpriteRenderer>();
                        sr.sprite = srPrefab.sprite;
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i,j+1)))
                    {
                        SpriteRenderer srPrefab = cells[4].prefab.GetComponent<SpriteRenderer>();
                        sr.sprite = srPrefab.sprite;
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i-1,j)))
                    {
                        SpriteRenderer srPrefab = cells[5].prefab.GetComponent<SpriteRenderer>();
                        sr.sprite = srPrefab.sprite;
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i+1,j)))
                    {
                        SpriteRenderer srPrefab = cells[6].prefab.GetComponent<SpriteRenderer>();
                        sr.sprite = srPrefab.sprite;
                    }
                    else if((i >= 1) && (i <= width - 2) && (j >= 1) && (j <= height - 2) && (!IsFull(i,j-1)))
                    {
                        SpriteRenderer srPrefab = cells[9].prefab.GetComponent<SpriteRenderer>();
                        sr.sprite = srPrefab.sprite;
                    }
                    else
                    {
                        SpriteRenderer srPrefab = cells[0].prefab.GetComponent<SpriteRenderer>();
                        sr.sprite = srPrefab.sprite;
                    }

                    // Adding grass
                    if((j < height - 1) && (_grid[i,j+1] == 0))
                    {
                        AddDecor(_currentGrid[i,j], grass, pos);
                    
                        int randomIndexDetail = UnityEngine.Random.Range(1, 4);
                        Vector3 posDetail = new Vector3(i - (width / 2) + offsetX, j - height + offsetY + 1, 0);

                        if(randomIndexDetail == 1)
                        {
                            AddDecor(_currentGrid[i,j], grass_detail_1, posDetail);
                        }
                        else if(randomIndexDetail == 2)
                        {
                            AddDecor(_currentGrid[i,j], grass_detail_2, posDetail);
                        }
                    }
                    if((j > 0) && (_grid[i,j-1] == 0))
                    {
                        AddDecor(_currentGrid[i,j], edge_bottom, pos);
                    }
                    if((i < width - 1) && (_grid[i+1,j] == 0))
                    {
                        AddDecor(_currentGrid[i,j], edge_right, pos);
                    }
                    if((i > 0) && (_grid[i-1,j] == 0))
                    {
                        AddDecor(_currentGrid[i,j], edge_left, pos);
                    }       
                }
            }
        }

        void AddDecor(GameObject parentObj, GameObject childObj, Vector3 pos)
        {
            GameObject decorObj = Instantiate(childObj, pos, Quaternion.identity);
            decorObj.transform.SetParent(parentObj.transform);
        }

        void AddOre(GameObject parentObj, OreType oreType, Vector3 pos)
        {
            GameObject objToSpawn = null;

            if(oreType == OreType.Coal)
            {
                objToSpawn = coalObj;
            }
            else if(oreType == OreType.Crystal)
            {
                objToSpawn = crystalObj;
            }
            if(oreType == OreType.Gemstone)
            {
                objToSpawn = gemstoneObj;
            }

            GameObject oreObj = Instantiate(objToSpawn, pos, Quaternion.identity);
            oreObj.transform.SetParent(parentObj.transform);
        }

        public void RemoveCell(int x, int y, GameObject vfx = null)
        {
            if(vfx != null) 
            {
                Instantiate(vfx, _currentGrid[x, y].transform.position, Quaternion.identity);
                impulseSource.GenerateImpulse();
            }
            Destroy(_currentGrid[x, y]);
            _currentGrid[x, y] = null;
            _grid[x, y] = 0;
            RefreshingMapNeighbours(x, y);
        }

        void RefreshingMapNeighbours(int x, int y)
        {
            for(int i = x - 1; i <= x + 1; i++)
            {            
                for(int j = y - 1; j <= y + 1; j++)
                {   
                    RefreshingMap(i, j);
                }
            }
        }

        public bool IsFull(int x, int y)
        {
            return (_grid[x, y] == 1) ? true : false;
        }

        public GameObject GetCell(int x, int y)
        {
            return _currentGrid[x, y];
        }

        /*
        /// <summary>
        /// Visualizing the grid.
        /// </summary>
        void OnDrawGizmos()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Gizmos.color = _grid[i, j] == 1 ? Color.black : Color.white;
                    Gizmos.DrawCube(new Vector3(i - (width / 2) + offsetX, j - height + offsetY, 0), Vector3.one);
                }
            }
        }
        */

        /// <summary>
        /// An example coroutine that waits for 2 seconds.
        /// </summary>
        IEnumerator ExampleCoroutine()
        {
            // Wait for 2 seconds before executing further code.
            yield return new WaitForSeconds(2f);

            Debug.Log("Action after 2 seconds.");
        }

    #endregion

}