using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



[System.Serializable]
public class Point {

    public string id;
    public float x;
    public float y;

    public Point() {
    }

    public Vector2 toVector() {
        return new Vector2(this.x, this.y);
    }
}


[System.Serializable]
public class Tile {

    public string id;
    public string pid1;
    public string pid2;
    public string pid3;
    public Vector2 centroid;

    public Tile() {

    }
}


// [System.Serializable]
// public class Wall {

//     public string id;
//     public string pid1;
//     public string pid2;

//     public Wall() {

//     }
// }

[System.Serializable]
public class MapDef {

    // public List<string> wall_ids;
    public int side;
    public List<int> tile_types;
    public string base_map;
    public List<string> reward_slot_ids;

    public MapDef() {
        // this.wall_ids = new List<string>();
        this.side = 10;
        this.tile_types = new List<int>();
        this.base_map = null;
        this.reward_slot_ids = new List<string>();
    }
}


[System.Serializable]
public class BaseMapDef {

    // public List<Wall> walls;
    public List<Point> points;
    public List<Tile> tiles;

    public List<float> center;  // x,y
    public List<float> start;  // x,y

    public Dictionary<string, Point> pointLookup;
    // public Dictionary<string, Wall> wallLookup;
    public Dictionary<string, Tile> tileLookup;

    public BaseMapDef() {
        // this.walls = new List<Wall>();
        this.tiles = new List<Tile>();
        this.points = new List<Point>();
        this.center = new List<float>();
        this.start = new List<float>();
        // this.wallLookup = new Dictionary<string, Wall>();
        this.pointLookup = new Dictionary<string, Point>();
        this.tileLookup = new Dictionary<string, Tile>();
    }

    public void init() {

        // Populate lookups
        // foreach (Wall wall in this.walls) {
        //     this.wallLookup[wall.id] = wall;
        // }
        foreach (Point point in this.points) {
            this.pointLookup[point.id] = point;
        }
        foreach (Tile tile in this.tiles) {
            this.tileLookup[tile.id] = tile;
        }
    }

    // public Wall getWall(string id) {
    //     return this.wallLookup[id];
    // }

    public Point getPoint(string id) {
        return this.pointLookup[id];
    }


    public Tile getTile(string id) {
        return this.tileLookup[id];
    }

}

