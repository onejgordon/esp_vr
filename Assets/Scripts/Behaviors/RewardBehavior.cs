﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardBehavior : MonoBehaviour
{
    public string id;
    private bool present = false;
    public GameObject goCylinder;
    public GameObject goPlaceholder;
    public ExperimentRunner experimentRunner;
    public AudioSource rewardAudio;


    void Start()
    {
        this.experimentRunner = GameObject.Find("World").GetComponent<ExperimentRunner>();
        this.rewardAudio = GetComponent<AudioSource>();
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
        if (this.present) {
            // Only consume present rewards
            SessionTrial st = experimentRunner.getCurrentTrial();
            goCylinder.SetActive(false);
            st.rewardCollected(this.id);
            this.rewardAudio.Play();
            Debug.Log("Consumed reward, new trial score: " + st.reward.ToString());
        }
    }

}
