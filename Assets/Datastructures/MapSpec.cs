using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



[System.Serializable]
public class Tile {

    public string id;
    public float x;
    public float y;
    public bool reward;
    public string type;

    public Tile() {
        this.reward = false;
    }
}


[System.Serializable]
public class Wall {

    public string id;
    public float x;
    public float y;

    public Wall() {

    }
}

[System.Serializable]
public class MapSpec {

    public List<Wall> walls;
    public List<Tile> tiles;

    public MapSpec() {
        this.walls = new List<Wall>();
        this.tiles = new List<Tile>();
    }
}
