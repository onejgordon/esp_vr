using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapBehavior : MonoBehaviour
{
    public MapSpec map;
    private List<GameObject> tiles; // Triangles
    private List<GameObject> walls;

    // Prefabs
    public GameObject prWall;
    public GameObject prTile;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void load(int map_num) {
        string path = string.Format("./ExperimentData/Maps/{0}.json", map_num);
        if (File.Exists(path))
        {
            string dataAsJson = File.ReadAllText(path);
            this.map = JsonUtility.FromJson<MapSpec>(dataAsJson);
            Debug.Log(string.Format("Loaded map from {0}", path));
            this.initialize();
        } else Debug.Log(string.Format("{0} doesn't exist", path));
    }


    public void maybeClearMap() {
        for (int i=0; i<this.walls.Count; i++) {
            Destroy(this.walls[i].gameObject);
        }
        for (int i=0; i<this.tiles.Count; i++) {
            Destroy(this.tiles[i].gameObject);
        }
        this.walls.Clear();
        this.tiles.Clear();
    }

    public void initialize() {
        for (int i=0; i<this.map.walls.Count; i++) {
            Wall wall = this.map.walls[i];
            Transform trWall = this.addWall(wall.id.ToString(), wall.x, wall.y, 0);
            trWall.name = "Wall" + wall.id;
        }
    }

    private Transform addWall(string id, float x, float y, float z) {
        GameObject goNewWall = Instantiate(this.prWall, new Vector3(x * Constants.MAP_MULT, y * Constants.MAP_MULT, z), Quaternion.Euler(0, 90, 90));
        Transform trNewWall = goNewWall.transform;
        WallBehavior wb = trNewWall.GetComponent<WallBehavior>();
        wb.setID(id);
        trNewWall.SetParent(gameObject.transform);
        goNewWall.tag = "Grabbable";
        this.walls.Add(goNewWall);
        return trNewWall;
    }
}
