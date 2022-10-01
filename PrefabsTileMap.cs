using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class PrefabsTileMap
{
    //public event EventHandler OnLoad;
    GridScript<TileMapObjects> grid;
    LevelMakerVisual levelMakerVisuals;

    List<ButtonAndTiles> buttonDataList;


    public PrefabsTileMap(int width, int height, float cellSize, Vector3 origin)
    {
        grid = new GridScript<TileMapObjects>(width, height, cellSize, origin, (GridScript<TileMapObjects> g, int x, int z) => new TileMapObjects(g, x, z));
        //setLevelManagerGrid();
    }
    public PrefabsTileMap(int width, int height, float cellSize)
    {
        grid = new GridScript<TileMapObjects>(width, height, cellSize, Vector3.zero, (GridScript<TileMapObjects> g, int x, int z) => new TileMapObjects(g, x, z));
    }

    public void SetTileMapVisual(LevelMakerVisual visual)
    {
        this.levelMakerVisuals = visual;
        visual.SetGrid(grid);
    }

    public void SetGridToPathfinder(PathFinding pathFinding)
    {
        pathFinding.SetTileMapGrid(grid);
    }

    //public void setLevelManagerGrid()
    //{
    //    LevelManager.instance.setGrid(grid);

    //}

    public void SetGridSprite(Vector3 worldPosition, TileMapObjects.TileMapPrefab sprite)
    {
        TileMapObjects tileMapObject = grid.GetGridObject(worldPosition);
        if (tileMapObject != null)
        {

            tileMapObject.SetTilePrefab(sprite);
        }
    }

    public TileMapObjects.TileMapPrefab GetGrideSprite(Vector3 worldPosition)
    {
        TileMapObjects tileMapObject = grid.GetGridObject(worldPosition);
        return tileMapObject.GetTilePrefab();
    }


    //save-Load functions
    #region save-Load

    public class SaveObject
    {
        public TileMapObjects.SaveObject[] saveArr;
        public ButtonAndTiles[] btnNtileArr;
    }

    public void Save()
    {
        if (levelMakerVisuals.hasFirstCell && levelMakerVisuals.hasLastCell)
        {
            buttonDataList = new List<ButtonAndTiles>();
            List<TileMapObjects.SaveObject> saveObjectList = new List<TileMapObjects.SaveObject>();
            bool s1 = true;
            bool s2 = true;
            bool s3 = true;
            bool s4 = true;
            bool h1 = true;
            bool h2 = true;
            bool h3 = true;
            bool h4 = true;


            if (levelMakerVisuals.hasSoftButton)
            {
                s1 = addButtonsToSaveData(TileMapObjects.TileMapPrefab.SoftButton1, TileMapObjects.TileMapPrefab.SoftTile1);
            }
            if (levelMakerVisuals.hasSoftButton2)
            {
                s2 = addButtonsToSaveData(TileMapObjects.TileMapPrefab.SoftButton2, TileMapObjects.TileMapPrefab.SoftTile2);
            }
            if (levelMakerVisuals.hasSoftButton3)
            {
                s3 = addButtonsToSaveData(TileMapObjects.TileMapPrefab.SoftButton3, TileMapObjects.TileMapPrefab.SoftTile3);
            }
            if (levelMakerVisuals.hasSoftButton4)
            {
                s4 = addButtonsToSaveData(TileMapObjects.TileMapPrefab.SoftButton4, TileMapObjects.TileMapPrefab.SoftTile4);
            }

            if (levelMakerVisuals.hasHardButton)
            {
                h1 = addButtonsToSaveData(TileMapObjects.TileMapPrefab.HardButton1, TileMapObjects.TileMapPrefab.HardTile1);
            }
            if (levelMakerVisuals.hasHardButton2)
            {
                h2 = addButtonsToSaveData(TileMapObjects.TileMapPrefab.HardButton2, TileMapObjects.TileMapPrefab.HardTile2);
            }
            if (levelMakerVisuals.hasHardButton3)
            {
                h3 = addButtonsToSaveData(TileMapObjects.TileMapPrefab.HardButton3, TileMapObjects.TileMapPrefab.HardTile3);
            }
            if (levelMakerVisuals.hasHardButton4)
            {
                h4 = addButtonsToSaveData(TileMapObjects.TileMapPrefab.HardButton4, TileMapObjects.TileMapPrefab.HardTile4);
            }

            if(s1 && s2 && s3 && s4 && h1 && h2 && h3 && h4)
            {
                for (int x = 0; x < grid.GetWidth; x++)
                {
                    for (int z = 0; z < grid.GetHeight; z++)
                    {
                        TileMapObjects tileMapObject = grid.GetGridObject(x, z);
                        TileMapObjects.TileMapPrefab tileMapPrefab = tileMapObject.GetTilePrefab();

                        if (tileMapPrefab != TileMapObjects.TileMapPrefab.none)
                        {
                            saveObjectList.Add(tileMapObject.Save());
                        }
                    }
                }

                SaveObject saveObject = new SaveObject
                {
                    saveArr = saveObjectList.ToArray(),
                    btnNtileArr = buttonDataList.ToArray()
                };
                SaveSystemScript.SaveObject(saveObject);
                GridManager.instance.ShowSaveStatus("Saved!");
            }
            else
            {
                GridManager.instance.ShowSaveStatus("Empty Button is present");
            }

        }
        else
        {
            GridManager.instance.ShowSaveStatus("Starting point & End point not set");
        }
            //Debug.Log("Starting point & End point not set ");
    }

    bool addButtonsToSaveData(TileMapObjects.TileMapPrefab buttonName, TileMapObjects.TileMapPrefab tileName)
    {
        //Vector2Int buttonPos = Vector2Int.zero;
        int buttonX = 0, buttonZ = 0;
        //List<Vector2Int> tilePos = new();
        List<int> tilePositions = new();
        for (int x = 0; x < grid.GetWidth; x++)
        {
            for (int z = 0; z < grid.GetHeight; z++)
            {
                TileMapObjects tileMapObject = grid.GetGridObject(x, z);
                TileMapObjects.TileMapPrefab tileMapPrefab = tileMapObject.GetTilePrefab();
                if (tileMapPrefab == buttonName)
                {
                    //buttonPos = new Vector2Int(x, z);
                    buttonX = x;
                    buttonZ = z;
                }
                if (tileMapPrefab == tileName)
                {
                    //tilePos.Add(new Vector2Int(x, z));
                    tilePositions.Add(x);
                    tilePositions.Add(z);
                }
            }
        }
        if (tilePositions.Count == 0)
        {
            //GridManager.instance.ShowSaveStatus("Empty Button is present");
            Debug.LogError(tileName + " is not present ");
            return false;
        }

        ButtonAndTiles buttonAndTiles = new ButtonAndTiles { BtnX = buttonX, BtnZ = buttonZ, tile = tilePositions.ToArray() };
        buttonDataList.Add(buttonAndTiles);
        return true;
    }

    public async Task<bool> LoadForGame(int num)
    {
        //clearMap();

        string fileName = "save_" + num;
        bool result;
        SaveObject saveObject = LevelsData.LoadObject<SaveObject>(fileName);

        if (saveObject != null)
        {
            //var task = new Task(() => {
                foreach (TileMapObjects.SaveObject SavedObjectInArr in saveObject.saveArr)
                {
                    TileMapObjects tileMapObject = grid.GetGridObject(SavedObjectInArr.x, SavedObjectInArr.z);

                    tileMapObject.LoadForGame(SavedObjectInArr);
                }

                float y = -0.1f;
                Dictionary<Vector3, GameObject[]> buttonDic = new Dictionary<Vector3, GameObject[]>();

                if (saveObject.btnNtileArr != null)
                {
                    foreach (ButtonAndTiles buttonAndTilesInArr in saveObject.btnNtileArr)
                    {
                        int tileCount = buttonAndTilesInArr.tile.Length / 2;
                        TileMapObjects[] mapObj = new TileMapObjects[tileCount];
                        GameObject[] obj = new GameObject[tileCount];
                        for (int i = 0; i < tileCount; i++)
                        {
                            mapObj[i] = grid.GetGridObject(buttonAndTilesInArr.tile[2 * i], buttonAndTilesInArr.tile[2 * i + 1]);
                            obj[i] = mapObj[i].currentPrefab;
                        }
                        buttonDic.Add(new Vector3(buttonAndTilesInArr.BtnX, y, buttonAndTilesInArr.BtnZ), obj);
                    }
                    LevelManager.instance.SetButtonDictionary(buttonDic);
                }
            //});

            await Task.Yield();
            result = true;
        }
        else
        {
            //await SomeFunc();
            //bool p = await Task.Run(() => {
            //    ViewManager.ShowLast();
            //    LevelManager.instance.StopGame();
            //    Debug.Log("Level doest not exist");
            //    return false;
            //});

            //ViewManager.show<MainMenuView>(false);
            //LevelManager.instance.StopGame();
            Debug.Log("Level doest not exist");


            await Task.Yield();

            //return false;
            result = false;
        }

        return result;
        //await Task.Yield();
        //OnLoad?.Invoke(this, EventArgs.Empty);
    }


    public void LoadForBuilder(int num)
    {
        string fileName = "save_" + num;
        SaveObject saveObject = SaveSystemScript.LoadObject<SaveObject>(fileName);
        foreach (TileMapObjects.SaveObject SavedObjectInArr in saveObject.saveArr)
        {
            TileMapObjects tileMapObject = grid.GetGridObject(SavedObjectInArr.x, SavedObjectInArr.z);

            tileMapObject.Load(SavedObjectInArr);
        }
    }

    public void ClearMapForGame()
    {
        for (int x = 0; x < grid.GetWidth; x++)
        {
            for (int z = 0; z < grid.GetHeight; z++)
            {
                TileMapObjects tileMapObject = grid.GetGridObject(x, z);

                TileMapObjects.TileMapPrefab prefab = tileMapObject.GetTilePrefab();
                if (prefab != TileMapObjects.TileMapPrefab.none)
                {
                    ObjectPoolForGame.ObjectPool.ReleaseObject(prefab, tileMapObject.currentPrefab);
                    tileMapObject.currentPrefab = null;
                    tileMapObject.SetTilePrefab(TileMapObjects.TileMapPrefab.none);
                }
            }
        }
        //ObjectPoolForGame.ObjectPool.FirstCell.SetActive(false);
        //ObjectPoolForGame.ObjectPool.LastCell.SetActive(false);
        ObjectPoolForGame.ObjectPool.DisposeAll();
    }


    public void ClearMapForBuilder()
    {
        ObjectPoolForGrid.ObjectPool.DisableFirstnLast();
        for (int x = 0; x < grid.GetWidth; x++)
        {
            for (int z = 0; z < grid.GetHeight; z++)
            {
                TileMapObjects tileMapObject = grid.GetGridObject(x, z);

                TileMapObjects.TileMapPrefab prefab = tileMapObject.GetTilePrefab();
                if (prefab != TileMapObjects.TileMapPrefab.none)
                {
                    ObjectPoolForGrid.ObjectPool.ReleaseObject(prefab, tileMapObject.currentPrefab);
                    tileMapObject.currentPrefab = null;
                    tileMapObject.SetTilePrefab(TileMapObjects.TileMapPrefab.none);
                }
            }
        }
        GridManager.instance.ResetGridMap();

        //ObjectPoolScript.ObjectPool.DisposeAll();
    }

    #endregion

    public int GetEnumLength()
    {
        return Enum.GetNames(typeof(PrefabsTileMap.TileMapObjects.TileMapPrefab)).Length;
    }

    [Serializable]
    public class ButtonAndTiles
    {
        public int BtnX;
        public int BtnZ;
        public int[] tile;
    }

    public class TileMapObjects
    {
        private GridScript<TileMapObjects> grid;
        public int x;
        public int z;

        //public int[,] pointedTo;

        public enum TileMapPrefab
        {
            none,
            FirstCell,
            Concrete,
            Glass,
            SoftButton,
            HardButton,
            NewSpawnTile,
            LastCell,

            SoftButton1, HardButton1, SoftTile1, HardTile1,
            SoftButton2, HardButton2, SoftTile2, HardTile2, 
            SoftButton3, HardButton3, SoftTile3, HardTile3,
            SoftButton4, HardButton4, SoftTile4, HardTile4,
            
            WeakDemon, StrongDemon
        }

        private TileMapPrefab tileMapPrefab;
        public GameObject currentPrefab;

        public TileMapObjects(GridScript<TileMapObjects> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetTilePrefab(TileMapPrefab prefab)
        {

            this.tileMapPrefab = prefab;
            grid.TriggerOnGridObjectChange(x, z);

        }

        public TileMapPrefab GetTilePrefab()
        {
            return tileMapPrefab;
        }

        public override string ToString()
        {
            //return tileMapPrefab.ToString();
            return " ";
        }

        [System.Serializable]
        public class SaveObject
        {
            public TileMapPrefab Tile;
            public int x;
            public int z;

        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                Tile = tileMapPrefab,
                x = x,
                z = z
            };
        }

        public void Load(SaveObject saveObject)
        {
            tileMapPrefab = saveObject.Tile;
            switch (tileMapPrefab)
            {
                //case TileMapPrefab.none:
                //    currentPrefab = null;
                //    //Debug.Log("none");
                //    break;
                case TileMapPrefab.Concrete:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetConcreteCell;
                    currentPrefab.transform.position = grid.GetWorldPosition(x,z);
                    //Debug.Log("getConcreteCell");
                    break;
                case TileMapPrefab.Glass:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetGlassCell;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z) ;
                    //Debug.Log("getGlassCell");
                    break;

                case TileMapPrefab.FirstCell:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.FirstCell;
                    //Debug.Log("FirstCell");
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z) ;
                    currentPrefab.SetActive(true);
                    break;
                case TileMapPrefab.LastCell:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.LastCell;
                    //Debug.Log("LastCell");
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z) ;
                    currentPrefab.SetActive(true);
                    break;
                case TileMapPrefab.SoftButton1:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetSoftButton1;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getSoftButton");
                    break;
                case TileMapPrefab.HardButton1:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetHardButton1;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getHardButton");
                    break;
                case TileMapPrefab.SoftButton2:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetSoftButton2;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getSoftButton");
                    break;
                case TileMapPrefab.HardButton2:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetHardButton2;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getHardButton");
                    break;
                case TileMapPrefab.SoftButton3:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetSoftButton3;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getSoftButton");
                    break;
                case TileMapPrefab.HardButton3:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetHardButton3;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getHardButton");
                    break;
                case TileMapPrefab.SoftButton4:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetSoftButton4;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getSoftButton");
                    break;
                case TileMapPrefab.HardButton4:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetHardButton4;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getHardButton");
                    break;
                case TileMapPrefab.SoftTile1:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetSoftTile1;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getSoftButton");
                    break;
                case TileMapPrefab.HardTile1:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetHardTile1;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getHardButton");
                    break;
                case TileMapPrefab.SoftTile2:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetSoftTile2;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getSoftButton");
                    break;
                case TileMapPrefab.HardTile2:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetHardTile2;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getHardButton");
                    break;
                case TileMapPrefab.SoftTile3:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetSoftTile3;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getSoftButton");
                    break;
                case TileMapPrefab.HardTile3:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetHardTile3;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getHardButton");
                    break;
                case TileMapPrefab.SoftTile4:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetSoftTile4;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getSoftButton");
                    break;
                case TileMapPrefab.HardTile4:
                    currentPrefab = ObjectPoolForGrid.ObjectPool.GetHardTile4;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //Debug.Log("getHardButton");
                    break;
            }
        }

        public void LoadForGame(SaveObject saveObject)
        {
            tileMapPrefab = saveObject.Tile;
            switch (tileMapPrefab)
            {
                case TileMapPrefab.Concrete:
                    currentPrefab = ObjectPoolForGame.ObjectPool.GetConcreteCell;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //currentPrefab.transform.parent = LevelManager.instance.setBaseHolder;
                    //Debug.Log("getConcreteCell");
                    break;
                case TileMapPrefab.Glass:
                    currentPrefab = ObjectPoolForGame.ObjectPool.GetGlassCell;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    //currentPrefab.transform.parent = LevelManager.instance.setBaseHolder;
                    //Debug.Log("getGlassCell");
                    break;

                case TileMapPrefab.FirstCell:
                    currentPrefab = ObjectPoolForGame.ObjectPool.GetConcreteCell;
                    //Debug.Log("FirstCell");
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    LevelManager.instance.SetFirstCellPos(currentPrefab);
                    tileMapPrefab = TileMapPrefab.Concrete;
                    //currentPrefab.transform.parent = LevelManager.instance.setBaseHolder;
                    //currentPrefab.SetActive(true);
                    break;
                case TileMapPrefab.LastCell:
                    currentPrefab = ObjectPoolForGame.ObjectPool.GetConcreteCell;
                    //Debug.Log("LastCell");
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    LevelManager.instance.SetLastCell(currentPrefab);
                    tileMapPrefab = TileMapPrefab.Concrete;
                    //currentPrefab.transform.parent = LevelManager.instance.setBaseHolder;
                    //currentPrefab.SetActive(true);
                    break;

                case TileMapPrefab.SoftButton1:
                case TileMapPrefab.SoftButton2:
                case TileMapPrefab.SoftButton3:
                case TileMapPrefab.SoftButton4:
                    currentPrefab = ObjectPoolForGame.ObjectPool.GetSoftButton;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    tileMapPrefab = TileMapPrefab.SoftButton;
                    // currentPrefab.transform.parent = LevelManager.instance.setBaseHolder;
                    break;

                case TileMapPrefab.HardButton1:
                case TileMapPrefab.HardButton2:
                case TileMapPrefab.HardButton3:
                case TileMapPrefab.HardButton4:
                    currentPrefab = ObjectPoolForGame.ObjectPool.GetHardButton;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    tileMapPrefab = TileMapPrefab.HardButton;
                    //currentPrefab.transform.parent = LevelManager.instance.setBaseHolder;
                    break;

                case TileMapPrefab.SoftTile1:
                case TileMapPrefab.SoftTile2:
                case TileMapPrefab.SoftTile3:
                case TileMapPrefab.SoftTile4:
                case TileMapPrefab.HardTile1:
                case TileMapPrefab.HardTile2:
                case TileMapPrefab.HardTile3:
                case TileMapPrefab.HardTile4:
                    currentPrefab = ObjectPoolForGame.ObjectPool.GetNewSpawnTile;
                    currentPrefab.transform.position = grid.GetWorldPosition(x, z);
                    tileMapPrefab = TileMapPrefab.NewSpawnTile;
                    //currentPrefab.transform.parent = LevelManager.instance.setBaseHolder;
                    break;

                case TileMapPrefab.WeakDemon:
                    currentPrefab = ObjectPoolForGame.ObjectPool.GetConcreteCell;
                    Vector3 wPos = grid.GetWorldPosition(x, z);
                    currentPrefab.transform.position = wPos;
                    tileMapPrefab = TileMapPrefab.Concrete;
                    //GameObject wDemon = ObjectPoolForGame.ObjectPool.GetWeakDemon;
                    //wPos.y = 0.5f;
                    //wDemon.transform.position = wPos;
                    LevelManager.instance.SetWeakDemon(wPos);
                    break;

                case TileMapPrefab.StrongDemon:
                    currentPrefab = ObjectPoolForGame.ObjectPool.GetConcreteCell;
                    Vector3 sPos = grid.GetWorldPosition(x, z);
                    currentPrefab.transform.position = sPos;
                    tileMapPrefab = TileMapPrefab.Concrete;
                    //GameObject sDemon = ObjectPoolForGame.ObjectPool.GetStrongDemon;
                    //sPos.y = 0.5f;
                    //sDemon.transform.position = sPos;
                    LevelManager.instance.SetStrongDemon(sPos);
                    break;

            }
        }
    }
}
