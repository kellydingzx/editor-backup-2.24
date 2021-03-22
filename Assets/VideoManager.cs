using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;
#if UNITY_STANDALONE_WIN
using AnotherFileBrowser.Windows;
#endif

//Contains functions related to the video player that would be commonly used.
public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject packageManager;
    private string url;

    void Start()
    {
        videoPlayer.waitForFirstFrame = true;
    }

    public long getFrame() { return videoPlayer.frame; }

    public void loadVideo(string new_url){
        if (File.Exists(new_url)) {
            videoPlayer.enabled = true;
            videoPlayer.url = new_url;
            videoPlayer.Prepare();
            videoPlayer.Play();
        }
    }

    public void removeVideo()
    {
        videoPlayer.Stop();
        videoPlayer.enabled = false;
    }

    public double videoLength()
    {
        return videoPlayer.length;
    }

    public bool videoLoaded()
    {
        return videoPlayer.isPrepared;
    }

    public void loadVideoAtFrame(long aim_frame, string video_url)
    {
        loadVideo(video_url);
        videoPlayer.frame = aim_frame;
    }

    public void loadVideoAtTime(double aim_time, string video_url)
    {
        loadVideo(video_url);
        videoPlayer.time = aim_time;
    }

    public void goToPosinTime(double aim_time)
    {
        Debug.Log(videoPlayer.frameCount);
        videoPlayer.time = aim_time;
    }

    public void getVideo() 
    {
        url = OpenFileBrowser("mp4");
        string new_url = packageManager.GetComponent<PackageManager>().addMainVideo(url);
        loadVideo(new_url);
    }

    // reference: https://github.com/SrejonKhan/AnotherFileBrowser Accessed on: 23rd Feb 2021
    // FileDialog for picking a single file
    public string OpenFileBrowser(string typee)
    {
#if UNITY_STANDALONE_WIN
        var bp = new BrowserProperties();
        bp.filter = typee + " files (*." + typee + ")|*." + typee;
        bp.filterIndex = 0;

        string res = "";

        new FileBrowser().OpenFileBrowser(bp, result =>
        {
            res = result;
        });

        return res;
#endif
    }

}
