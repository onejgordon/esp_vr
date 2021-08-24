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
        return color;
    }

    public float tileVelocityMult() {
        float vel = 1.0f;
        if (this.tile_type == Constants.WATER) vel = 1.0f;
        if (this.tile_type == Constants.DEEP_WATER) vel = 0.5f;
        return vel;
    }

    public void makeMesh(Vector2[] points) {
        bool is3D = true;
        float true_height = this.height;
        if (this.tile_type == Constants.LAND) true_height *= 2.0f; // Raised land
        gameObject.transform.parent = this.transform;

        // add PolyExtruder script to newly created GameObject,
        // keep track of its reference
        PolyExtruder polyExtruder = gameObject.AddComponent<PolyExtruder>();

        // configure display of outline (before running the poly extruder)
        // polyExtruder.isOutlineRendered = true;    // default: false
        // polyExtruder.outlineWidth = 0.1f;         // default: 0.01f
        // polyExtruder.outlineColor = Color.blue;   // default: Color.black

        polyExtruder.createPrism(gameObject.name, true_height, points, this.tileColor(), is3D);        
    }

}
