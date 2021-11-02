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
    public double ts_planning_start;
    public double ts_transition_start;
    public double ts_navigation_start;
    public double ts_end;

    public bool practice = false;

    public List<string> rewards_present;
    public List<Fixation> fixations;
    public List<Record> records;
    public List<string> rewards_collected;

    public MapDef map;
    public int map_index;

    public SessionTrial(string session_id, int id, MapDef map, int map_index, bool practice) {
        this.session_id = session_id;
        this.map = map;
        this.map_index = map_index;
        this.trial_id = id;
        this.reward = 0;
        this.rewards_present = new List<string>();
        this.practice = practice;
        this.fixations = new List<Fixation>();
        this.records = new List<Record>();
        this.rewards_collected = new List<string>();
        this.randomizeRewardPresence();
    }

    public void randomizeRewardPresence() {
        // Currently just sets first to rewards to be present (so fixed per map)
        for (int i=0; i<this.map.reward_slot_ids.Count; i++) {
            string reward_id = this.map.reward_slot_ids[i];
            if (i < Constants.REWARDS_PER_TRIAL) this.rewards_present.Add(reward_id);
        }
    }

    public bool allRewardsCollected() {
        return this.reward >= Constants.REWARDS_PER_TRIAL;
    }

    public void rewardCollected(string id) {
        this.reward += 1;
        this.rewards_collected.Add(id);
    }

    public double navigationSecondsRemaining() {
        return (Constants.NAVIGATION_SECONDS - (Util.timestamp() - this.ts_navigation_start));
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

    public bool addFixation(string modeChar, string objectName, double start, double stop) {
        this.fixations.Add(new Fixation(modeChar, objectName, start, stop));
        return true;
    }
}
