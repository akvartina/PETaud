using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class TaskManager : MonoBehaviour
{   //Text 
    public TextManager textman; //access to the script TextManager
    public Vector3 position0;
    public Vector3 position1;

    //Fixation
    public GameObject fixationPoint; 
    public float fixationTime = 1.5f; // time in seconds for the fixation point to remain visible
    private bool fixation = false;
    
    //Images
    public GameObject[] imagePairs;
    private GameObject currentPairImage;
    
    //Videos
    public VideoPlayer videoPlayer;
    public List<VideoClip> videoClips = new List<VideoClip>(); //Assign manually in Unity
    private Queue<VideoClip> videoQueue = new Queue<VideoClip>();

    private int currentPairIndex = 0;

    //Data lists
    private List<string> response = new List<string>(); //Collects responses
    private int[] trialCounter = new int[] //Number of trials array
        { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
    private List<int> corr = new List<int>(); //Collects correctness points
    private List<float> time = new List<float>(); //Collects _trialTime data
    List<string> allData = new List<string>(); //Collects all data for CSV
    private List<string> answers = new List<string>() //Answers array
    { "A", "A", "B", "A", "B", "B", "B", "A", "B", "A", "A", "B", "A", "B", "B", "B", "A", "B" };
    public string currData;
    private int currTr = 0;
    private int[] cond = new int[] //Condition
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

    //Time
    public TimeManager timeman;
    
    //CSV
    public WriteCSV csv;
    private string header = "Trial Number, Condition, Response, Correctness, RT"; //header
    

    void Start()
    {
        //Text settings
        textman.screenText.text = "Hello! \n" +
                                  "Thank you for participating in our experiment.\n" +
                                  " \n" +
                                  "You will hear a number of audio recordings of a natural environment \n" +
                                  "that capture the movement of the vehicles behind you as you are riding a bike.\n" +
                                  " \n" +
                                  "There will be two options of pictures A and B presented.\n" +
                                  "Please listen carefully and choose the picture that descibes\n " +
                                  "the situation better in your opinion by pressing\n " +
                                  "the <color=red>'S'</color> or <color=red>'K'</color> button on the keyboard respectively.\n" +
                                  " \n" +
                                  "Press <color=red>SPACE</color> to start the experiment";
        // Set the position of the text object
        textman.screenText.transform.position = position0;
        
        //Images inactive -- assign them in unity
        foreach (GameObject imagePair in imagePairs)
        {
            imagePair.SetActive(false);
        }

        fixationPoint.SetActive(false);
        
        //Assign videos to a queue
        foreach (VideoClip clip in videoClips)
        {
            videoQueue.Enqueue(clip);
        }
        videoPlayer.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currTr >= 1 && currTr != 10 && currTr < 20)
        {
            response.Add("0");
        }
        else if (Input.GetKeyDown(KeyCode.S) && currTr >= 1 && currTr != 10 && currTr < 20)
        {
            Debug.Log("A");
            response.Add("A");
        }
        else if (Input.GetKeyDown(KeyCode.K) && currTr >= 1 && currTr != 10 && currTr < 20)
        {
            Debug.Log("B");
            response.Add("B");
        }
        
        if ( //all cases
            ( !fixation && currTr < 20 && Input.GetKeyDown(KeyCode.Space) ||
             Input.GetKeyDown(KeyCode.S) ||
             Input.GetKeyDown(KeyCode.K)))
        {
            currTr++;
            Debug.Log(currTr);

            if (currTr == 1 || currTr == 11)
            {
                //Start first trial
                textman.screenText.text = "";
                StartCoroutine(Sequence());
            }
            
            else if (currTr == 10)
            {
                Stop();
                
                timeman._timeResponse = Time.realtimeSinceStartup; //the time of the response
                timeman.callTime(); //calculate the difference
                time.Add(timeman._trialTime);

                textman.screenText.text = "End of Trial 1.\n " +
                                          " \n" +
                                          "In the next Trial the voice assistant will try to help you to be aware of the situation on the road. " +
                                          "Please pay attention to the audio and choose the picture that describes the situation better.\n" +
                                          " \n" +
                                          "Press <color=red>SPACE</color> to start Trial 2.";
                // Get the RectTransform component of the text object
                textman.screenText.transform.position = position0;
            }
            
            else if (currTr == 20) //fix
            {
                Stop();
                
                timeman._timeResponse = Time.realtimeSinceStartup; //the time of the response
                timeman.callTime(); //calculate the difference
                time.Add(timeman._trialTime);

                textman.screenText.text = "End of the experiment";
                // Get the RectTransform component of the text object
                textman.screenText.transform.position = position0;
                CheckCorrect();
                SaveTrialData();
                csv.MakeCSV(allData, header);

                //Debug.Log(string.Join(", ", trialCounter));
                //Debug.Log(string.Join(", ", response));
                //Debug.Log(string.Join(", ", corr));
                //Debug.Log(corr.Count);
                //Debug.Log(string.Join(", ", time));
            }
            
            else
            {
                //Start of a new trial
                Stop();
                timeman._timeResponse = Time.realtimeSinceStartup; //the time of the response
                timeman.callTime(); //calculate the difference
                time.Add(timeman._trialTime);

                textman.screenText.text = "";
                StartCoroutine(Sequence());
            }
        }

    }

    IEnumerator Sequence()
    {

        fixation = true;
        yield return StartCoroutine(_fixation());
        fixation = false;
        yield return StartCoroutine(_videoPlayer());
    }

    IEnumerator _fixation()
    {
        // show first fixation point
        fixationPoint.SetActive(true);
        yield return new WaitForSeconds(fixationTime);
        fixationPoint.SetActive(false);
    }

    IEnumerator _videoPlayer()
    {
        timeman._timeStart = Time.realtimeSinceStartup; //set the new timer
        
        textman.screenText.text = "Listen carefully and choose the picture A or B that " +
                                  "describes the situation in the audio better. 'S' - stands for A, 'K' - stands for B.\n " +
                                  "Press Space if not sure.";
        // Get the RectTransform component of the text object
        textman.screenText.transform.position = position1;

        //show images
        // Enable the current image pair
        currentPairImage = imagePairs[currentPairIndex];
        currentPairImage.SetActive(true);
        
        // play video
        ChangePlayVideo();

        // Increment the current pair index, wrapping around if necessary
        currentPairIndex = (currentPairIndex + 1) % imagePairs.Length;
        yield return null;
    }

    void Stop()
    {
        currentPairImage.SetActive(false); //cancel the previous
        videoPlayer.Pause();
        videoPlayer.enabled = false;
    }

    void ChangePlayVideo()
    {
        if (videoQueue.Count > 0)
        {
            VideoClip nextClip = videoQueue.Dequeue();
            videoPlayer.clip = nextClip;
            //Debug.Log("Video is assigned");
            videoPlayer.enabled = true;
            videoPlayer.Play();
        }
    }

    public void CheckCorrect()
    {
        for (int i = 0; i < response.Count && i < answers.Count; i++)
        {
            if (response[i].Equals(answers[i]))
            {
                corr.Add(1);
            }
            else
            {
                corr.Add(0);
            }
        }
    }
    
    public void SaveTrialData()
    {
        for (int a = 0; a <= 17; a++)
        {
            currData += trialCounter[a].ToString() +  "," + cond[a].ToString() + 
                        "," + response[a] + "," + corr[a].ToString() + "," 
                        + time[a].ToString() + Environment.NewLine; 
            //trial number, condition, response, correct, RT
        }
        
        Debug.Log(currData);
        allData.Add(currData);
    }

}
