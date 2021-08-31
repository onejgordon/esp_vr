using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardBehavior : MonoBehaviour
{
    public string id;
    private bool present = false;
    public GameObject goCylinder;
    public GameObject goPlaceholder;

    void Start()
    {

    }

    void Update()
    {

    }

    public void setup(string id) {
        this.id = id;
    }

    public void setPresence(bool present) {
        this.present = present;
        if (this.present) {
            this.goCylinder.SetActive(true);
        }
    }

    public void consume() {
        Debug.Log("Consumed reward");
        gameObject.SetActive(false);
    }

}
