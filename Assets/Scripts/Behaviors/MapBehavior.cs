using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapBehavior : MonoBehaviour
{
    public MapDef map;
    public BaseMapDef baseMapDef;
    public bool baseMapLoaded = false;
    private List<GameObject> tiles; 
    private List<GameObject> walls;

    // Prefabs
    public GameObject prWall;
    public GameObject prTile;


    void Start()
    {
    }

    void Update()
    {

    }

    public void load(int map_num) {
        string path = string.Format("./ExperimentData/Maps/{0}.json", map_num);
        if (File.Exists(path))
        {
            string dataAsJson = File.ReadAllText(path);
            this.map = JsonUtility.FromJson<MapDef>(dataAsJson);
            Debug.Log(string.Format("Loaded map from {0}", path));
            if (!this.baseMapLoaded) {
                // Load baseMapDef only once
                string baseMapPath = "./ExperimentData/Maps/base_map_4.json";
                Debug.Log("Load baseMapRef from " + baseMapPath);
                string baseDataAsJson = File.ReadAllText(baseMapPath);
                this.baseMapDef = JsonUtility.FromJson<BaseMapDef>(baseDataAsJson);
                this.baseMapDef.init();
                this.baseMapLoaded = true;
            }
            this.initialize();
        } else Debug.Log(string.Format("{0} doesn't exist", path));
    }


    public void maybeClearMap() {
        Debug.Log("Clear map");
        if (this.walls != null) {
            for (int i=0; i<this.walls.Count; i++) {
                Destroy(this.walls[i].gameObject);
            }
            this.walls.Clear();
        }
        if (this.tiles != null) {
            for (int i=0; i<this.tiles.Count; i++) {
                Destroy(this.tiles[i].gameObject);
            }
            this.tiles.Clear();
        }
    }

    public void initialize() {
        Debug.Log("Map init");
        this.walls = new List<GameObject>();
        this.tiles = new List<GameObject>();   
        for (int i=0; i<this.map.wall_ids.Count; i++) {
            Wall wall = this.baseMapDef.getWall(this.map.wall_ids[i]);
            Transform trWall = this.addWall(wall);
        }
        for (int j=0; j<this.map.tile_types.Count; j++) {
            Tile tile = this.baseMapDef.getTile(j.ToString());
            int tile_type = this.map.tile_types[j];
            Transform trTile = this.addTile(tile, tile_type);
        }
    }

    public void setupCameraForPlanning(Transform trCamera) {
        trCamera.SetPositionAndRotation(new Vector3(this.baseMapDef.center[0], Constants.CAM_PLANNING_HEIGHT, this.baseMapDef.center[1]),
            Quaternion.Euler(90, 0, 0) // Look down
        );
    }

    public void setupCameraForNavigation(Transform trCamera) {
        trCamera.SetPositionAndRotation(new Vector3(this.baseMapDef.start[0], Constants.CAM_NAV_HEIGHT, this.baseMapDef.start[1]),
            Quaternion.Euler(90, 0, 0)
        );
    }

    public void setupAgentForNavigation(Transform trAgent) {
        trAgent.position = new Vector3(this.baseMapDef.start[0], 2.0f, this.baseMapDef.start[1] + trAgent.localScale.z);
    }

    private Transform addWall(Wall wall) {
        GameObject goNewWall = Instantiate(this.prWall);
        Transform trNewWall = goNewWall.transform;
        trNewWall.name = "Wall" + wall.id;
        WallBehavior wb = trNewWall.GetComponentInChildren<WallBehavior>();
        wb.setup(wall);
        string[] pids = new string[]{wall.pid1, wall.pid2};
        Vector2 p1 = this.baseMapDef.getPoint(wall.pid1).toVector();
        Vector2 p2 = this.baseMapDef.getPoint(wall.pid2).toVector();        
        wb.positionAndOrient(p1, p2);
        trNewWall.SetParent(gameObject.transform);
        this.walls.Add(goNewWall);
        return trNewWall;
    }

    private Transform addTile(Tile tile, int tile_type) {
        GameObject goNewTile = Instantiate(this.prTile);
        if (tile_type == Constants.LAND) {
            goNewTile.tag = "obstacle";
        } else {
            goNewTile.tag = "tile";
        }
        Transform trNewTile = goNewTile.transform;
        TileBehavior tb = trNewTile.gameObject.GetComponent<TileBehavior>();
        tb.setup(tile, tile_type);
        List<Vector2> points = new List<Vector2>();
        string[] pids = new string[]{tile.pid1, tile.pid2, tile.pid3};
        foreach (string pid in pids)
        {
            points.Add(this.baseMapDef.getPoint(pid).toVector());
        }
        tb.makeMesh(points.ToArray());
        trNewTile.SetParent(gameObject.transform);
        trNewTile.name = "Tile" + tile.id;
        this.tiles.Add(goNewTile);
        return trNewTile;
    }

}
