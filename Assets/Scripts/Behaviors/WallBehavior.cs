using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehavior : MonoBehaviour
{
    private string id;
    private bool is_goal;
    private int position;


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

    public void setID(string id) {
        this.id = id;
    }

    public void setIsGoal(bool is_goal) {
        this.is_goal = is_goal;
    }

    public void setPosition(int pos) {
        // L: 0, R: 1
        this.position = pos;
    }

}
