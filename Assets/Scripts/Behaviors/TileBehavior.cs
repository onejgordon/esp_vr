using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehavior : MonoBehaviour
{
    private string id;
    private bool is_goal;
    private int type;
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
        this.type = tile_type;
    }

    public Color tileColor() {
        Color color = Color.grey;
        if (this.type == Constants.WATER) color = Color.blue;
        if (this.type == Constants.DEEP_WATER) color = new Color(0, 0, 0.7f);
        if (this.type == Constants.LAND) color = Color.grey;
        return color;
    }

    public void makeMesh(Vector2[] points) {
        bool is3D = true;

        gameObject.transform.parent = this.transform;

        // add PolyExtruder script to newly created GameObject,
        // keep track of its reference
        PolyExtruder polyExtruder = gameObject.AddComponent<PolyExtruder>();

        // configure display of outline (before running the poly extruder)
        // polyExtruder.isOutlineRendered = true;    // default: false
        // polyExtruder.outlineWidth = 0.1f;         // default: 0.01f
        // polyExtruder.outlineColor = Color.blue;   // default: Color.black

        polyExtruder.createPrism(gameObject.name, this.height, points, this.tileColor(), is3D);        
    }

}
