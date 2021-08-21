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


    public Tile() {

    }
}


[System.Serializable]
public class Wall {

    public string id;
    public string pid1;
    public string pid2;

    public Wall() {

    }
}

[System.Serializable]
public class MapDef {

    public List<string> wall_ids;
    public List<int> tile_types;

    public MapDef() {
        this.wall_ids = new List<string>();
        this.tile_types = new List<int>();
    }
}


[System.Serializable]
public class BaseMapDef {

    public List<Wall> walls;
    public List<Point> points;
    public List<Tile> tiles;

    public Dictionary<string, Point> pointLookup;
    public Dictionary<string, Wall> wallLookup;
    public Dictionary<string, Tile> tileLookup;

    public BaseMapDef() {
        this.walls = new List<Wall>();
        this.tiles = new List<Tile>();
        this.points = new List<Point>();
        this.wallLookup = new Dictionary<string, Wall>();
        this.pointLookup = new Dictionary<string, Point>();
        this.tileLookup = new Dictionary<string, Tile>();
    }

    public void init() {

        // Populate lookups
        foreach (Wall wall in this.walls) {
            this.wallLookup[wall.id] = wall;
        }
        foreach (Point point in this.points) {
            this.pointLookup[point.id] = point;
        }
        foreach (Tile tile in this.tiles) {
            this.tileLookup[tile.id] = tile;
        }
    }

    public Wall getWall(string id) {
        return this.wallLookup[id];
    }

    public Point getPoint(string id) {
        return this.pointLookup[id];
    }


    public Tile getTile(string id) {
        return this.tileLookup[id];
    }

}

