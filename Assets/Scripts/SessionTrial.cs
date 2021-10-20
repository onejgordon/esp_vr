using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class SessionTrial
{
    private string session_id; // Unique id for each subject/session
    public int trial_id;
    public int reward = 0;
    public float reward_uncertainty;
    public double ts_start;
    public double ts_end;

    public bool practice = false;

    public List<string> rewards_present;
    public List<Fixation> fixations;
    public List<Record> records;

    public MapDef map;

    public SessionTrial(string session_id, int id, MapDef map, bool practice) {
        this.session_id = session_id;
        this.map = map;
        this.trial_id = id;
        this.ts_start = Util.timestamp();
        this.reward = 0;
        this.reward_uncertainty = 0.5f;
        this.rewards_present = new List<string>();
        this.practice = practice;
        this.fixations = new List<Fixation>();
        this.records = new List<Record>();
        this.randomizeRewardPresence();
    }

    public void randomizeRewardPresence() {
        // Currently just sets first to rewards to be present (so fixed per map)
        int N_REWARDS = 2;
        for (int i=0; i<this.map.reward_slot_ids.Count; i++) {
            string reward_id = this.map.reward_slot_ids[i];
            if (i < N_REWARDS) this.rewards_present.Add(reward_id);
        }
        // foreach (string reward_id in this.map.reward_slot_ids) {
        //     bool present = Random.value < this.reward_uncertainty;
        //     if (present) this.rewards_present.Add(reward_id);
        // }
    }

    public void Finished() {
        this.ts_end = Util.timestamp();
    }

    public void SaveToFile() {
        string json = JsonUtility.ToJson(this);
        string path = this.outfile();
        StreamWriter sw = File.CreateText(path);
        sw.Close();
        File.WriteAllText(path, json);
    }

    public void CleanUpData() {
        // Only to be run after we've saved
        this.records.Clear();
        this.fixations.Clear();
    }
    public string outfile() {
        return SessionSaver.OUTDIR + "session_" + this.session_id + "_trial_" + this.trial_id.ToString() + ".json";
    }

    public bool scored() {
        return !this.practice;
    }

    public bool addRecord(Record record) {
        this.records.Add(record);
        return true;
    }

    public bool addFixation(string objectName, double start, double stop) {
        this.fixations.Add(new Fixation(objectName, start, stop));
        return true;
    }
}
