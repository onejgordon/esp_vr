using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehavior : MonoBehaviour
{
    private string id;
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

    public void setup(Wall wall) {
        this.id = wall.id;
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

        // run poly extruder according to input data
        polyExtruder.createPrism(gameObject.name, this.height, points, Color.black, is3D);        
    }

}
