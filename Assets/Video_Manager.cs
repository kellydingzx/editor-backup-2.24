using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
#if UNITY_STANDALONE_WIN
using AnotherFileBrowser.Windows;
#endif



public class Video_Manager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string url;

    public void getVideo()
    {
        url = OpenFileBrowser("mp4");
        if (url != null)
        {
            videoPlayer.url = url;
        }
        videoPlayer.Play();

    }

    // reference: https://github.com/SrejonKhan/AnotherFileBrowser Accessed on: 23rd Feb 2021
    /// FileDialog for picking a single file
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
