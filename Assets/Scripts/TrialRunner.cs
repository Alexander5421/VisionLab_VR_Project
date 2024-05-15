using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class TrialRunner:MonoBehaviour
{
    public enum Direction
    {
        left,
        right
    }
    public Stimulus targetPrefab;
    public Stimulus distractionPrefab1;
    public Stimulus distractionPrefab2;
    public Stimulus distractionPrefab;
    public Stimulus targetPrefab2D;
    public Stimulus distractionPrefab2D;
    
    public Transform[] candidatePosition;
    public Transform[] candidatePosition2D;
    public int score;
    public int currRunIndex = 0;
    public int totalRunCount = 0;
    public Direction correctDirection = Direction.left;
    public Action finish;
    public List<Stimulus> stimuliList = new List<Stimulus>();
    private UserInput userInput;
    private Stopwatch stopwatch;
    private EntryInfo entryInfo;
    private string trialType;
    private float cooldown;
    [SerializeField]
    private CSVLogger csvLogger;
    private System.Random rand = new System.Random();
    public List<(int, int, int)> ExperimentCombinations = new List<(int, int, int)>();
    [SerializeField] public GameObject plane;
    private Vector3 plane_normal;

    public void Awake()
    {
        this.enabled = false;
        InitilizeExperimentCombinations();
        // create a new file
        string fileName = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";
        csvLogger.Initialize(fileName);
        InitializeInputs();
        plane_normal =  plane.transform.TransformDirection(Vector3.up);
        ProjectSpheresOntoPlane();
    }

    void ProjectSpheresOntoPlane()
    {
        // Get the normal vector of the plane
        Vector3 planeNormal = plane.transform.up;

        // Iterate through all spheres
        foreach (Transform sphere in candidatePosition)
        {
            // Calculate the vector from the sphere's position to any point on the plane
            Vector3 sphereToPlane = plane.transform.position - sphere.position;

            // Calculate the signed distance from the sphere to the plane
            float signedDistance = Vector3.Dot(sphereToPlane, planeNormal);

            // Calculate the projected position of the sphere onto the plane
            Vector3 projectedPosition = sphere.position + signedDistance * planeNormal;

            // Set the sphere's position to the projected position
            sphere.position = projectedPosition;
            print(sphere.position);
        }
    }

    public void InitilizeExperimentCombinations()
    {
        ExperimentCombinations.Add((1, 0, 0));
        ExperimentCombinations.Add((1, 1, 0));

        ExperimentCombinations.Add((5, 0, 0));
        ExperimentCombinations.Add((5, 0, 1));
        ExperimentCombinations.Add((5, 1, 0));
        ExperimentCombinations.Add((5, 1, 1));

        ExperimentCombinations.Add((10, 0, 0));
        ExperimentCombinations.Add((10, 0, 1));
        ExperimentCombinations.Add((10, 1, 0));
        ExperimentCombinations.Add((10, 1, 1));
        string sceneName = SceneManager.GetActiveScene().name;
        if (! sceneName.Contains("10position"))
        {
            ExperimentCombinations.Add((20, 0, 0));
            ExperimentCombinations.Add((20, 0, 1));
            ExperimentCombinations.Add((20, 1, 0));
            ExperimentCombinations.Add((20, 1, 1));
        }
    }

    public static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void InitializeTrials(int totalCount,string trialType)
    {
       
        this.enabled = true;
        score = 0;
        currRunIndex = 0;
        totalRunCount = totalCount;
        this.trialType = trialType;
        // first run
        NewRun();
    }

    private void InitializeInputs()
    {
        userInput = new UserInput();
        userInput.gameplay.Left.performed += ctx =>
        {
            ReceiveInput(false);
            //print("input received");
        };

        userInput.gameplay.Right.performed += ctx =>
        {
            ReceiveInput(true);
            //print("input received");
        };
    }

    private void OnEnable()
    {
        userInput.Enable();
    }
    private void OnDisable()
    {
        userInput.Disable();
    }

    private void NewRun()
    {
        int combinations = ExperimentCombinations.Count;

        if (currRunIndex % combinations == 0)
        {
            Shuffle(ExperimentCombinations);
            print("New trial");
        }

        if (currRunIndex < totalRunCount)
        {
            StartNewRun(ExperimentCombinations[currRunIndex% combinations].Item1, ExperimentCombinations[currRunIndex%combinations].Item2, ExperimentCombinations[currRunIndex%combinations].Item3);
        }
        else
        {
            // finish
            finish.Invoke();
            ClearStimuli();
            // disable this script
            this.enabled = false;
        }
    }
    private void ReceiveInput(bool isRight)
    {
        stopwatch.Stop();
        entryInfo.respkeys = isRight ? "right" : "left";
        entryInfo.correctness = isRight == (correctDirection == Direction.right) ? "correct" : "incorrect";
        entryInfo.reactionTime = stopwatch.ElapsedMilliseconds.ToString();
        entryInfo.distractorType = (distractionPrefab == distractionPrefab1) ? "Brown" : "White";
        if (isRight)
        {
            if (correctDirection == Direction.right)
            {
                score++;
            }
        }
        else
        {
            if (correctDirection == Direction.left)
            {
                score++;
            }
        }
        // score += ((correctDirection==Direction.right)==isRight)?1:0;
        currRunIndex++;
        csvLogger.AddDataWithUserInfo(entryInfo.Serialize());
        ClearStimuli();
        NewRun();


    }


    private void StartNewRun(int count, int hand, int color)
    {
        print("Total Stimuli " + count + "  stimuli hand " + hand + " distracion color " + color );
        ClearStimuli();
        // first decide 2D or 3D
        // bool is3D = Random.Range(0, 2) == 1;
        if (color == 0)
        {
            distractionPrefab = distractionPrefab1;
        }
        else
        {
            distractionPrefab = distractionPrefab2;
        }
        int totalStimuli = count;
        //print(totalStimuli);

        bool is3D = true;
        var realCandidatePosition = is3D ? candidatePosition : candidatePosition2D;
        //int totalStimuli = Random.Range(realCandidatePosition.Length-2,realCandidatePosition.Length+1);
        // from candidatePosition array, randomly select totalStimuli and create a new array
        
        
        Transform[] stimuliPositions=realCandidatePosition.ToList().OrderBy(arg => Guid.NewGuid()).Take(totalStimuli).ToArray();
        
        
        int targetLocation= Random.Range(0,totalStimuli);
        // other stimuli are distraction
        Stimulus targetPrefabLocal;
        Stimulus distractionPrefabLocal;
        if (is3D)
        {
            targetPrefabLocal = targetPrefab;
             distractionPrefabLocal = distractionPrefab;
        }
        else
        {
            targetPrefabLocal = targetPrefab2D;
            distractionPrefabLocal = distractionPrefab2D;
        }
        for (int i = 0; i < totalStimuli; i++)
        {
            if (i == targetLocation)
            {
                // create target
                // as the child of stimuliPositions[i]
                Stimulus target = Instantiate(targetPrefabLocal,stimuliPositions[i]);
                stimuliList.Add(target);
                // set target direction randomly and set correctDirection
                bool isRight = (hand == 0 ? false: true);
                target.SetSphere(isRight);
                correctDirection = isRight ? Direction.right : Direction.left;
            }
            else
            {
                // create distraction
                Stimulus distraction = Instantiate(distractionPrefabLocal, stimuliPositions[i]);
                stimuliList.Add(distraction);
                distraction.SetSphere(Random.Range(0, 2) == 1);
            }
        }
        
        entryInfo = new EntryInfo();
        entryInfo.stimuli_locs = stimuliPositions.Select(arg => arg.GetSiblingIndex()).ToList();
        entryInfo.trialType = trialType;
        Scene currentScene = SceneManager.GetActiveScene();
        entryInfo.exp_name = currentScene.name;
        entryInfo.distractorType = (distractionPrefab == distractionPrefab1) ? "Brown" : "White";

        stopwatch = new Stopwatch();
        stopwatch.Start();
    }


    private void ClearStimuli()
    {
        // if stimuliList is not empty, destroy all
        if (stimuliList.Count > 0)
        {
            foreach (var stimulus in stimuliList)
            {
                Destroy(stimulus.gameObject);
            }

            stimuliList.Clear();
        }
    }
}