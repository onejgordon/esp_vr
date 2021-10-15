using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehavior : MonoBehaviour
{
    private string id;
    private bool is_goal;
    private int tile_type;
    public float height;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public string getId() {
        return this.id;
    }

    public void setup(Tile tile, int tile_type) {
        this.id = tile.id;
        this.tile_type = tile_type;
    }

    public Color tileColor() {
        Color color = Color.grey;
        if (this.tile_type == Constants.WATER) color = Color.blue;
        if (this.tile_type == Constants.DEEP_WATER) color = new Color(0, 0, 0.7f);
        if (this.tile_type == Constants.LAND) color = Color.grey;
        if (this.tile_type == Constants.WALL) color = Color.black;
        return color;
    }

    public float tileVelocityMult() {
        float vel = 1.0f;
        if (this.tile_type == Constants.WATER) vel = 1.0f;
        if (this.tile_type == Constants.DEEP_WATER) vel = 0.75f;
        return vel;
    }

    public float tileHeight() {
        if (this.tile_type == Constants.LAND) {
            return 1.5f;
        } else if (this.tile_type == Constants.DEEP_WATER | this.tile_type == Constants.WATER) {
            return 1.0f;
        } else if (this.tile_type == Constants.WALL) {
            return 7.0f;
        } else {
            return 1.0f; // Catch
        }
    }

    public void makeMesh(Vector2[] points) {
        bool is3D = true;
        
        // add PolyExtruder script to newly created GameObject,
        // keep track of its reference
        PolyExtruder polyExtruder = gameObject.AddComponent<PolyExtruder>();
        
        // configure display of outline (before running the poly extruder)
        // polyExtruder.isOutlineRendered = true;    // default: false
        // polyExtruder.outlineWidth = 0.1f;         // default: 0.01f
        // polyExtruder.outlineColor = Color.blue;   // default: Color.black

        polyExtruder.createPrism(gameObject.name, this.tileHeight(), points, this.tileColor(), is3D);        
        gameObject.transform.Translate(0, -this.height, 0); // Water top at y=0
    }

}
