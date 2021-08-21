using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehavior : MonoBehaviour
{
    private string id;
    private bool is_goal;
    private int type;
    public Material matWater;
    public Material matDWater;
    public Material matLand;


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
        this.type = tile_type;
        gameObject.GetComponent<Renderer>().material = this.tileMaterial();
    }

    public Material tileMaterial() {
        if (this.type == 0) return this.matWater;
        if (this.type == 1) return this.matDWater;
        if (this.type == 2) return this.matLand;
        return this.matLand;
    }

    public void makeMesh(Vector2[] points) {
        float extrusionHeight = .1f;
        bool is3D = true;

        gameObject.transform.parent = this.transform;
        gameObject.name = "Tile";

        // add PolyExtruder script to newly created GameObject,
        // keep track of its reference
        PolyExtruder polyExtruder = gameObject.AddComponent<PolyExtruder>();

        // configure display of outline (before running the poly extruder)
        // polyExtruder.isOutlineRendered = true;    // default: false
        // polyExtruder.outlineWidth = 0.1f;         // default: 0.01f
        // polyExtruder.outlineColor = Color.blue;   // default: Color.black

        // run poly extruder according to input data
        polyExtruder.createPrism(gameObject.name, extrusionHeight, points, Color.grey, is3D);        
    }

}
