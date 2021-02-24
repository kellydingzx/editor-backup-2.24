using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
#if UNITY_STANDALONE_WIN
using AnotherFileBrowser.Windows;
#endif



public class Video_Manager : MonoBehaviour
{
    public GameObject chose_button;
    public VideoPlayer videoPlayer;
    private string url;

    public void getVideo()
    {
        url = OpenFileBrowser("mp4");
        if (url != null)
        {
            videoPlayer.url = url;
        }
        videoPlayer.Play();

    }
    // Start is called before the first frame update
    void Start()
    {
        if (url != null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
    }

    // reference: https://github.com/SrejonKhan/AnotherFileBrowser Accessed on: 23rd Feb 2021
    // <summary>
    /// FileDialog for picking a single file
    /// </summary>
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
