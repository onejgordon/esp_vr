using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// using Valve.VR.Extras;
using System.IO;
using Tobii.XR;

public class ExperimentRunner : MonoBehaviour
{
    // Configs / params
    private Color SKY_DEFAULT = new Color(.8f, .8f, .8f);
    private Color DBLUE = new Color(.1f, .1f, 1f);
    public int N_CHIMES = 3;
    private Color DGREEN = new Color(.1f, .6f, .1f);
    private Material goalMat = null;
    public int planningSeconds = 10;
    public int transitionSeconds = 5;
    public int endTrialPauseSecs = 3;

    public SessionSaver session;
    public bool QUICK_DEBUG = true;
    public int N_TRIALS = 10; // Set to 0 for production. Just for short debug data collection
    public int N_MAPS = 20;
    public int N_MANUAL_MAPS = 3;
    public int practice_rounds = 2;
    public bool left_handed = false;
    public bool record = false;

    // State
    private double ts_exp_start = 0; // Timestamp
    private int trial_index = 0;
    private int practice_remaining = 0;
    private SessionTrial current_trial;
    private bool recording = false;
    private bool practicing = false;
    private int chimesPlayed = 0;
    private string mode = "start";
    private double ts_next_chime_check = 0;
    private double ts_next_record = 0;

    // Session specs
    private string session_id;
    private List<int> map_order = new List<int>();
    private List<MapDef> maps = new List<MapDef>();

    // Main experiment objects

    private Transform trCameraRig;
    public Transform trAgent;
    private Transform controller;
    public Transform trSceneLight;
    public Camera sceneCamera;


    // Main experiment behaviors
    private MapBehavior mapBehavior;

    // Other scene objects
    public UIBehavior ui;
    public GameObject goMap;
    public AudioSource chimeAudio;

    // public SteamVR_LaserPointer laserPointer;

    void Start()
    {
        this.goalMat =  Resources.Load("GoalMat", typeof(Material)) as Material;
        this.goMap = GameObject.Find("Map");
        if (QUICK_DEBUG) {
            practice_rounds = 0;
            N_TRIALS = 5;
            this.session_id = "DEBUG";
        } else {
            this.session_id = ((int)Util.timestamp()).ToString();
        }
        if (practice_rounds > 0) this.practicing = true;
        this.session.data.session_id = this.session_id;
        this.session.data.left_handed = this.left_handed;
        this.practice_remaining = this.practice_rounds;
        this.sceneCamera = GameObject.Find("Camera").GetComponent<Camera>();
        this.ui = GameObject.Find("UICanvas").GetComponent<UIBehavior>();
        this.trCameraRig = GameObject.Find("[CameraRig]").GetComponent<Transform>();
        this.controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Transform>();
        this.chimeAudio = GetComponent<AudioSource>();
        this.mapBehavior = this.goMap.GetComponent<MapBehavior>();
        this.maps = new List<MapDef>();
        // TobiiXR_Settings tobii_settings = new TobiiXR_Settings();
        // tobii_settings.FieldOfUse = FieldOfUse.Analytical;
        // TobiiXR.Start(tobii_settings); Performed by TobiiXR_Initializer?
        this.BeginExperiment();
    }

    // Update is called once per frame
    void Update()
    {
        double ts = Util.timestamp();
        if (recording && this.current_trial != null && this.recordingMode()) {
            if (ts > ts_next_record) {
                Vector3 gazeOrigin = new Vector3();
                Vector3 gazeDirection = new Vector3();
                float convDistance = -1.0f; // Default when not valid
                bool eitherEyeClosed = false;
                var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
                if (eyeTrackingData.GazeRay.IsValid) {
                    gazeOrigin = eyeTrackingData.GazeRay.Origin;
                    gazeDirection = eyeTrackingData.GazeRay.Direction;
                }
                eitherEyeClosed = eyeTrackingData.IsLeftEyeBlinking || eyeTrackingData.IsRightEyeBlinking;
                if (eyeTrackingData.ConvergenceDistanceIsValid) {
                    convDistance = eyeTrackingData.ConvergenceDistance;
                }
                Record record = new Record(
                    this.modeChar(),  // First letter of mode in [p, t, n]
                    this.trAgent,
                    trCameraRig,
                    controller,
                    gazeOrigin,
                    gazeDirection,
                    convDistance,
                    eitherEyeClosed);
                this.current_trial.addRecord(record);
                ts_next_record = ts + 0.1;  // 10hz?
            }
        }
        if (ts > ts_next_chime_check) {
            this.maybePlayChimes();
            ts_next_chime_check = ts + 0.5;  // 2hz
        }
    }

    public bool recordingMode() {
        return this.mode != "ended";
    }

    public int getChimesNeeded(SessionTrial trial) {
        int chimes_needed = 0;
        int seconds_from_end = 0;
        double ts = Util.timestamp();
        if (this.isPlanning()) {
            seconds_from_end = (int)(this.planningSeconds - (ts - trial.ts_planning_start));
            chimes_needed += this.N_CHIMES - seconds_from_end;
        } else if (this.isTransitioning()) {
            // seconds_from_end = (int)(this.transitionSeconds - (ts - trial.ts_transition_start));
            // chimes_needed += 2*this.N_CHIMES - seconds_from_end;
        } else if (this.isNavigating()) {
            seconds_from_end = (int)(Constants.NAVIGATION_SECONDS - (ts - trial.ts_navigation_start));
            chimes_needed += 2*this.N_CHIMES - seconds_from_end;
        }
        return chimes_needed;
    }
    public void maybePlayChimes() {
        SessionTrial trial = this.getCurrentTrial();
        if (trial != null) {
            int needed = this.getChimesNeeded(trial);
            if (needed > chimesPlayed) {
                // Play chime
                this.chimeAudio.Play();
                // Debug.LogFormat("Playing chime {0} of {1}", this.chimesPlayed, needed);
                this.chimesPlayed += 1;
            }

        }
    }

    public string modeChar() {
        return this.mode[0].ToString();
    }

    public bool isNavigating() {
        return this.mode == "navigation";
    }

    
    public bool isPlanning() {
        return this.mode == "planning";
    }

    public bool isTransitioning() {
        return this.mode == "transition";
    }

    void RandomizeTrialOrder() {
        this.map_order = Util.Shuffle<int>(Enumerable.Range(1, N_MAPS).ToList());
        // Practice maps start at 100
        for (int i=0; i<this.practice_rounds; i++) {
            this.map_order.Insert(0, 100 + this.practice_rounds - i - 1);
        }
        // Add manual maps at end (they start at 200)
        this.map_order.AddRange(Enumerable.Range(200, N_MANUAL_MAPS));
        Debug.Log(string.Join(", ", map_order));
    }

    public void BeginExperiment() {
        if (record) {
            this.recording = true;
        }
        this.RandomizeTrialOrder();
        this.ts_exp_start = Util.timestamp();
        this.session.data.ts_session_start = this.ts_exp_start;
        GotoNextTrial();
    }

    public void GotoNextTrial() {
        this.trial_index += 1;
        if ((N_TRIALS != 0 && this.trial_index > N_TRIALS) || this.trial_index > this.N_TRIALS + this.practice_rounds) Finish();
        else {
            RunOneTrial();
        }
    }

    void RunOneTrial() {
        this.ui.HideHUDScreen();
        bool first_real = false;
        int map_index = this.map_order[this.trial_index - 1];
        Debug.LogFormat("Running trial {0}, map {1}, practicing: {2}", this.trial_index, map_index, this.practicing);
        this.mapBehavior.load(map_index);
        this.SetMapVisibility(false);
        if (this.practicing) {
            if (this.practice_remaining == 0) {
                this.practicing = false;
                first_real = true;
            } else {
                this.practice_remaining -= 1;
            }
        }
        
        this.current_trial = new SessionTrial(this.session_id, this.trial_index, this.mapBehavior.map, this.mapBehavior.map_index, this.practicing);
        if (this.practicing) {
            ui.ShowHUDScreenWithConfirm(
                string.Format(
                    "This is practice round {0} of {1}. Your performance on these rounds won't affect your score. Click your controller trigger to proceed.",
                    this.practice_rounds - this.practice_remaining,
                    this.practice_rounds
                ),
                Color.black, "BeginTrial");
        } else {
            if (first_real) {
                // Show message indicating we're starting real trials
                ui.ShowHUDScreenWithConfirm("Great job. Practice rounds finished. All remaining trials are real and will be scored. Click your controller trigger to proceed.",
                DBLUE, "BeginTrial");
            } else {
                BeginTrial();
            }
        }
    }

    void BeginTrial() {
        this.StartPlanningPhase();
    }

    void StartPlanningPhase() {
        Debug.Log("Start planning...");
        this.mode = "planning";
        this.SetMapVisibility(true);
        this.trAgent.gameObject.SetActive(true);
        this.mapBehavior.setupCameraForPlanning(this.trCameraRig);
        this.mapBehavior.setupAgentForPlanning(this.trAgent);
        this.getCurrentTrial().ts_planning_start = Util.timestamp();

        // Set timeout to start navigation
        StartCoroutine(WaitThenTransition(this.planningSeconds));
    }

    IEnumerator WaitThenTransition(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        StartTransitionPhase();
    }

    void SetMapVisibility(bool visible) {
        // Also agent
        foreach (Renderer r in this.goMap.GetComponentsInChildren<Renderer>()) {
            r.enabled = visible;
        }
        foreach (Renderer r in this.trAgent.gameObject.GetComponentsInChildren<Renderer>()) {
            r.enabled = visible;
        }
    }

    void StartTransitionPhase() {
        Debug.Log("Start transition...");
        this.mode = "transition";
        this.SetMapVisibility(false);
        this.getCurrentTrial().ts_transition_start = Util.timestamp();
        // TODO: Make everything invisible, but preserve fixation capture
        StartCoroutine(WaitThenNavigate(this.transitionSeconds));
    }

    IEnumerator WaitThenNavigate(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        StartNavigationPhase();
    }

    void StartNavigationPhase() {
        Debug.Log("Start navigation...");
        this.mode = "navigation";        
        this.mapBehavior.setupAgentForNavigation(this.trAgent);
        this.mapBehavior.setupCameraForNavigation();
        this.SetMapVisibility(true);
        this.mapBehavior.showExistingRewards(this.getCurrentTrial().rewards_present);
        this.getCurrentTrial().ts_navigation_start = Util.timestamp();
        this.chimesPlayed = 0;
        if (Constants.NAVIGATION_SECONDS > 0) {
            StartCoroutine(EndTrialAfterNavigation(Constants.NAVIGATION_SECONDS, this.current_trial.trial_id));
        }
    }

    IEnumerator EndTrialAfterNavigation(float waitTime, int currentTrialId) {
        yield return new WaitForSeconds(waitTime);
        if (this.current_trial != null && this.current_trial.trial_id == currentTrialId) {
            Debug.Log("Trial timeout...");
            EndTrial();
        }
    }

    public void EndTrial() {
        this.mode = "ended";
        this.trAgent.gameObject.SetActive(false); // Hide agent
        ui.ClearCountdown();
        ui.ShowHUDScreen("Trial finished, starting next trial shortly...", Color.blue);

        this.current_trial.Finished();
        this.current_trial.SaveToFile();
        this.current_trial.CleanUpData(); // Deletes large data once saved
        session.AddTrial(this.current_trial);
        this.current_trial = null;
        if (Constants.NAVIGATION_SECONDS != -1) {
            // Time limit, clear timer to avoid double GoTo
            CancelInvoke();
        }
        Invoke("GotoNextTrial", this.endTrialPauseSecs);
    }


    public SessionTrial getCurrentTrial() {
        return this.current_trial;
    }

    void Finish() {
        Debug.Log("Done, saving...");
        session.data.ts_session_end = Util.timestamp();
        session.SaveToFile();
        if (this.recording) this.recording = false;
        this.mapBehavior.maybeClearMap();
        string results = "All trials finished!\n\n";
        int total_points_possible = this.session.data.getPointsPossible();
        double percent = this.session.data.total_points / (double) total_points_possible;
        results += string.Format("Final score is {0} points of of {1} points possible.\nYour final success rate is {2:0.0}%.\n\nYour experimenter will help you take off the VR headset.",
            this.session.data.total_points,
            total_points_possible,
            100.0 * percent
        );
        ui.ShowHUDScreen(results, DGREEN);
        TobiiXR.Stop();
    }
}
