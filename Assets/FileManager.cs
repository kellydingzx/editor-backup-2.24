using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.IO;
#if UNITY_STANDALONE_WIN
using AnotherFileBrowser.Windows;
#endif

public class FileManager : MonoBehaviour
{
    public GameObject photo_url_display;
    public RawImage image;


    public GameObject project_path;
    public Controller controller;

    public string photo_path;
    public string video_path;
    public GameObject inputpanel;
    

    public VideoPlayer videoPlayer;
    public GameObject url_video;
    public GameObject back_button;

    public bool on_second;

    //Variables for switching videos
    private GameObject current_hotspot;
    private GameObject[] objs;
    private List<GameObject> needs_back;
    private long location;
    private string url_old_video;
    private Vector3 camera_pos;

    //Used for the choose photo button on input panel.
    public void choosePhoto() 
    {
        photo_path = OpenFileBrowser();
        if (photo_path != null)
        {
            WWW www = new WWW("file:///" + photo_path);
            image.texture = www.texture;
        }
        photo_url_display.GetComponent<Text>().text = photo_path;
    }

    public void PlaySecondVideo()
    {
        //Record information about the previous video
        location = videoPlayer.frame;
        camera_pos = Camera.main.transform.position;
        url_old_video = videoPlayer.url;
        videoPlayer.Stop();
        //Read and modify path of the new video
        video_path = url_video.GetComponent<Text>().text;
        if (Path.IsPathRooted(video_path))
        {
            videoPlayer.url = video_path;
        }
        else
        {
            videoPlayer.url = System.IO.Path.Combine(project_path.GetComponent<Text>().text, video_path);
            Debug.Log("video url");
            Debug.Log(video_path);
        }
        videoPlayer.Play();
        //Deactive all buttons from the previous video
        objs = GameObject.FindGameObjectsWithTag("Trigger");
        needs_back = new List<GameObject>();
        foreach (GameObject button in objs)
        {
            if (button.activeSelf)
            {
                needs_back.Add(button);
                if (button.GetInstanceID().ToString().Equals(controller.GetComponent<Controller>().current_id))
                {
                    current_hotspot = button;
                }
            }
            button.SetActive(false);
        }
        inputpanel.SetActive(false);
        back_button.SetActive(true);
    }


    public string getPhoto()
    {
        return photo_path;
    }

    public void loadVideoOntoPanel(string videoURL){
        WWW www = new WWW("file:///" + videoURL);
        image.texture = www.texture;
    }

    // reference: https://github.com/SrejonKhan/AnotherFileBrowser Accessed on: 23rd Feb 2021
    // FileDialog for picking a single file
    public string OpenFileBrowser()
    {
        var bp = new BrowserProperties();
        bp.filter = "png files (*.png)|*.png|jpg files (*.jpg)|*.jpg";
        bp.filterIndex = 0;

        string res = "";

        new FileBrowser().OpenFileBrowser(bp, result =>
        {
            res = result;
        });

        return res;
    }
}
