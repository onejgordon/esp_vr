using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Valve.VR;

public class MapBehavior : MonoBehaviour
{
    public MapDef map;
    public BaseMapDef baseMapDef;
    public bool baseMapLoaded = false;
    private List<GameObject> tiles; 
    //private List<GameObject> walls;
    private List<GameObject> rewards; 

    // Prefabs
    //public GameObject prWall;
    public GameObject prTile;
    public GameObject prReward;
    public Transform trSceneLight;
    public Camera sceneCamera;
    public CameraFollowBehavior rigFollow;
    public bool highlightGaze;


    void Start()
    {
    }

    void Update()
    {

    }

    public void load(int map_num) {
        string path = string.Format("./ExperimentData/Maps/{0}.json", map_num);
        this.maybeClearMap();
        if (File.Exists(path))
        {
            string dataAsJson = File.ReadAllText(path);
            this.map = JsonUtility.FromJson<MapDef>(dataAsJson);
            Debug.Log(string.Format("Loaded map from {0}", path));
            if (!this.baseMapLoaded) {
                // Load baseMapDef only once
                string baseMapPath = string.Format("./ExperimentData/Maps/{0}", this.map.base_map);
                Debug.Log("Load baseMapRef from " + baseMapPath);
                string baseDataAsJson = File.ReadAllText(baseMapPath);
                this.baseMapDef = JsonUtility.FromJson<BaseMapDef>(baseDataAsJson);
                this.baseMapDef.init();
                this.baseMapLoaded = true;
            }
            this.initialize(0.5f);
        } else Debug.Log(string.Format("{0} doesn't exist", path));
    }


    public void maybeClearMap() {
        Debug.Log("Clear map");
        if (this.tiles != null) {
            for (int i=0; i<this.tiles.Count; i++) {
                Destroy(this.tiles[i].gameObject);
            }
            this.tiles.Clear();
        }
        if (this.rewards != null) {
            for (int i=0; i<this.rewards.Count; i++) {
                Destroy(this.rewards[i].gameObject);
            }
            this.rewards.Clear();
        }

    }

    public void initialize(float uncertainty) {
        Debug.Log("Map init");
        // this.walls = new List<GameObject>();
        this.tiles = new List<GameObject>();   
        this.rewards = new List<GameObject>();   
        // for (int i=0; i<this.map.wall_ids.Count; i++) {
        //     Wall wall = this.baseMapDef.getWall(this.map.wall_ids[i]);
        //     Transform trWall = this.addWall(wall);
        // }
        for (int j=0; j<this.map.tile_types.Count; j++) {
            Tile tile = this.baseMapDef.getTile(j.ToString());
            int tile_type = this.map.tile_types[j];
            Transform trTile = this.addTile(tile, tile_type);
        }
        for (int i=0; i<this.map.reward_slot_ids.Count; i++) {
            this.addReward(this.map.reward_slot_ids[i]);
        }
    }

    public void setupCameraForPlanning(Transform trCamera) {
        trCamera.SetPositionAndRotation(new Vector3(this.baseMapDef.center[0], Constants.CAM_PLANNING_HEIGHT, this.baseMapDef.center[1]),
            Quaternion.Euler(90, 30, 0) // Look down
        );
        trSceneLight.position.Set(this.baseMapDef.center[0], Constants.SCENE_LIGHT_HEIGHT, this.baseMapDef.center[1]);
        this.sceneCamera.orthographic = true;
    }

    public void setupAgentForPlanning(Transform trAgent) {
        trAgent.rotation = Quaternion.Euler(0, 30, 0);
        trAgent.position = new Vector3(this.baseMapDef.start[0], 2.0f, this.baseMapDef.start[1]) + 3*trAgent.localScale.z * trAgent.forward;
        this.sceneCamera.orthographic = false;
    }

    public void setupCameraForNavigation() {
        this.rigFollow.jumpTo(); // Avoid lerping nauseatingly
    }

    public void setupAgentForNavigation(Transform trAgent) {
        // trAgent.position = new Vector3(this.baseMapDef.start[0], 2.0f, this.baseMapDef.start[1] + 3*trAgent.localScale.z);
    }


    public void showExistingRewards(List<string> rewards_present) {
        foreach (GameObject reward in this.rewards) {
            RewardBehavior rb = reward.GetComponent<RewardBehavior>();
            rb.setPresence(rewards_present.Contains(rb.id));
        }
    }

    private Transform addTile(Tile tile, int tile_type) {
        GameObject goNewTile = Instantiate(this.prTile);
        if (tile_type == Constants.LAND || tile_type == Constants.WALL) {
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
        if (this.highlightGaze) {
            goNewTile.transform.GetChild(1).gameObject.AddComponent<MyHighlightAtGaze>();
            goNewTile.transform.GetChild(2).gameObject.AddComponent<MyHighlightAtGaze>();
        }
        this.tiles.Add(goNewTile);
        return trNewTile;
    }

    private Transform addReward(string at_tile_id) {
        Tile tile = this.baseMapDef.getTile(at_tile_id);
        Vector3 pos = new Vector3(tile.centroid.x, Constants.REWARD_HEIGHT, tile.centroid.y);
        GameObject goReward = Instantiate(this.prReward, pos, Quaternion.identity, gameObject.transform);
        Transform trReward = goReward.transform;
        RewardBehavior rb = goReward.GetComponent<RewardBehavior>();
        rb.setup(at_tile_id);
        trReward.name = "Reward" + at_tile_id;
        this.rewards.Add(goReward);
        return trReward;
    }
}
