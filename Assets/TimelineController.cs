﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

//This file draws the timeline on the timeline panel.
public class TimelineController : MonoBehaviour
{
    public Window_Graph window_Graph;

    private Hashtable timeline_dictionary;
    private string rootpath;

    private int recur_depth;
    void Start()
    {
        recur_depth = 0;
        timeline_dictionary = new Hashtable();
    }

    public void draw(string path, float stemY, float gap, float startX, float endX) // This function draws the timeline recursively.
    {
        Debug.Log(path);
        float videoLength = 0;
        if (recur_depth == 0) rootpath = path;

        string assumed_videopath = Path.Combine(path, "MainVideo.mp4");
        Debug.Log(assumed_videopath);
        if (File.Exists(@assumed_videopath) && recur_depth < 3)
        {
            recur_depth++;
            string json_path = System.IO.Path.Combine(path, "hotspots.json");
            Debug.Log(json_path);
            string hotspotjsons = System.IO.File.ReadAllText(@json_path);
            HotspotDatas hotspotdatas = JsonUtility.FromJson<HotspotDatas>(hotspotjsons);
            double largest_end = 10;
            foreach (string json in hotspotdatas.hotspotdatas)
            {
                HotspotData h = JsonUtility.FromJson<HotspotData>(json);
                if (h.end_time > largest_end) largest_end = h.end_time;
            }
            videoLength = (float)largest_end + 20;

            foreach (string json in hotspotdatas.hotspotdatas)
            {
                HotspotData h = JsonUtility.FromJson<HotspotData>(json);
                float xpos = (float)(startX + ((endX - startX) * (h.start_time / videoLength)));
                GameObject tree_node = window_Graph.CreateCircle(new Vector2(xpos, stemY), h.name + "c");
                string tree_node_id = tree_node.GetInstanceID().ToString();
                string relative_path = "";
                if(recur_depth > 1)
                {
                    relative_path = path.Substring(rootpath.Length);
                }
                GotoHelper helper = new GotoHelper(relative_path, h.worldPosition, h.start_time);
                timeline_dictionary.Add(tree_node_id, helper);
                Debug.Log(h.url_video);
                if ((h.url_video != null) && (h.url_video != ""))
                {
                    float gap2 = (float)(gap * (1 - (h.start_time / videoLength)));
                    window_Graph.CreateDotConnection(new Vector2(xpos, stemY), new Vector2(xpos, stemY + gap2), h.name);
                    window_Graph.CreateDotConnection(new Vector2(xpos, stemY + gap2), new Vector2(endX, stemY + gap2), h.name + "l");
                    draw(@Path.Combine(path, h.name), stemY + gap2, (gap / 2), xpos, endX);
                }
            }

        }
    }

    //This function set the root path where the draw function starts the drawing.
    //This function is used by the package manager, where the user starts the new project.
    public void setRootPath(string root_path) 
    {
        rootpath = root_path;
    }


    //This function is used when the timeline is redrawed by the save button.
    //The recursion flag is set to 0.
    public void resetRecurDepth()
    {
        recur_depth = 0;
    }

    //This function is used when the timeline is redrawed by the save button.
    public string getRoot()
    {
        return rootpath;
    }

    //The follwing functions are used for when click on the node on the timeline.
    //Each of them gets the information accordingly for the openwindow1().
    public string getPathByID(string node_id)
    {
        return getHelper(node_id).pathBranch;
    }

    public double getStartTimeByID(string node_id)
    {
        return getHelper(node_id).start_time;
    }
    public Vector3 getWorldPositionByID(string node_id)
    {
        return getHelper(node_id).worldposition;
    }
    private GotoHelper getHelper(string node_id)
    {
        return (GotoHelper)timeline_dictionary[node_id];
    }

    //Structure used to store the relative information needed for the openwindow1() function.
    public class GotoHelper{ 
        public string pathBranch;
        public Vector3 worldposition;
        public double start_time;

        public GotoHelper(string branch_path, Vector3 location, double starts_time)
        {
            this.pathBranch = branch_path;
            this.worldposition = location;
            this.start_time = starts_time;
        }
    }

    //Helper structure for reading the json files.
    public class HotspotDatas
    {
        public string[] hotspotdatas;
    }
    public class HotspotData
    {
        public double start_time;
        public double end_time;
        public string name;
        public string text;
        public string url_photo;
        public string url_video;
        public Vector3 worldPosition;
        public Quaternion rot;
        public HotspotData(double _start_time, double _end_time, string _name, string _text, string _url_photo, string _url_video, GameObject _hotspot)
        {
            start_time = _start_time;
            end_time = _end_time;
            name = _name;
            text = _text;
            url_photo = _url_photo;
            url_video = _url_video;
            worldPosition = _hotspot.transform.position;
            rot = _hotspot.transform.rotation;
        }
    }

    /*
    public float VideoLength(string path)
    {
        double vidlength;
        GameObject h = new GameObject("vidlength");
        var videoPlayer = h.AddComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.url = path;
        vidlength = videoPlayer.length;
        Debug.Log(vidlength);
        Destroy(h);
        return (float)vidlength;
    }
    */
    
}