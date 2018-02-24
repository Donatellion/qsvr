﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using Leap.Unity.Interaction;

public class MemorySpaces : MonoBehaviour {

    //Corutine setup memory space
    IEnumerator setupMemorySpaceCorutine;

    //Intantiate the photograph prefabs used in Memory Space 1
    public Rigidbody polaroid;
    public Transform m_parent;
    public Material[] PictureSet1;
    public Material[] PictureSet2;
    public Material[] PictureSet3;
    public Rigidbody Diary;

    //leapmotion interaction manager must be added to instantiated game objects at runtime
    private InteractionBehaviour LMInteractionBehaviour;

    /*
    Logic for each memory space goes into this function
    */
    public void MemorySpaceIsReady(float memberSpaceNumber)
    {
        
        //First memory space logic
        if (memberSpaceNumber == 1)
        {
            //PauseVideos();
            //TODO: This is temp
            StartCoroutine(ExitMemorySpace(25.5f, "ExitMemorySpaceOne"));

            //Trigger Memory space 1 audio
            GetComponent<SoundManager>().MemorySpaceOne();

            //Populate the box with a random set of potographs
            //Instantiate a polaroid prefab and assign materials from our random array set ( 1of 3)
            //TODO: Pick a random set
            Material[] randomSet = PictureSet1;
            foreach (Material photo in randomSet)
            {
                Rigidbody polaroidInstance;
                polaroidInstance = Instantiate(polaroid, m_parent.position, m_parent.rotation) as Rigidbody;
                //parent it
                polaroidInstance.transform.parent = m_parent;
                //assign our photo from the array to the front of polaroid which is the first child in prefab
                polaroidInstance.transform.GetChild(0).GetComponent<Renderer>().material = photo;
                //if using Leapmotion, add interaction manager
                if (GetComponent<SceneController>().LeapMotion)
                {
                    LMInteractionBehaviour = polaroidInstance.gameObject.AddComponent<InteractionBehaviour>();
                    LMInteractionBehaviour.allowMultiGrasp = true;
       
                }
            }
        }

        //Second memory space logic
        if (memberSpaceNumber == 2)
        {
            //Trigger Memory space 2 audio
            GetComponent<SoundManager>().MemorySpaceTwo();

            //Instantiate the diary
            Rigidbody diaryInstance;
            diaryInstance = Instantiate(Diary, m_parent.position, m_parent.rotation) as Rigidbody;
            //make a child of the memory space object
            diaryInstance.transform.parent = m_parent;
            //if using Leapmotion, add interaction manager
            if (GetComponent<SceneController>().LeapMotion)
            {
                diaryInstance.gameObject.AddComponent<InteractionBehaviour>();
                diaryInstance.transform.GetChild(5).gameObject.AddComponent<InteractionBehaviour>();
                diaryInstance.transform.GetChild(6).gameObject.AddComponent<InteractionBehaviour>();
            }
               
            GetComponent<SoundManager>().initVoiceover();
            //TODO: This is temp
            StartCoroutine(ExitMemorySpace(25.5f, "ExitMemorySpaceTwo"));
        }
    }
    //~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~




    public void enterMemorySpace(float memberSpaceNumber)
    {
        setupMemorySpaceCorutine = SetupMemorySpace(4.5f, memberSpaceNumber); // create an IEnumerator object
        StartCoroutine(setupMemorySpaceCorutine);
    }

    public void PauseVideos()
    {
        //GetComponent<SceneController>().VideoPlayer.GetComponent<MediaPlayer>().Control.Pause();   
        //GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control.Pause();
        //GetComponent<SceneController>().Blue.GetComponent<MediaPlayer>().Control.Pause();
    }

    IEnumerator SetupMemorySpace(float wait, float memberSpaceNumber)
    {
        yield return new WaitForSeconds(wait);
        MemorySpaceIsReady(memberSpaceNumber);
        StopCoroutine(setupMemorySpaceCorutine);
    }

    IEnumerator ExitMemorySpace(float wait, string triggerLabel)
    {
        yield return new WaitForSeconds(wait);
        //destroy objects available in memory space
        foreach (Transform child in m_parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        EventManager.TriggerEvent(triggerLabel);
    }
}
