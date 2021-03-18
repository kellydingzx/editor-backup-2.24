using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineController : MonoBehaviour
{
    public Window_Graph window_Graph;
    public void draw(string path, float stemY, float gap, float startX, float endX) {
        string[] x = System.IO.Directory.GetFiles(path);
        float videoLength = 0;
        /*
        foreach (string name in x)
        {
            if (System.IO.Path.GetExtension(name) == ".mp4") {
                videoLength = VideoLength(name);
            }
        }
        */
        string json_path1 = System.IO.Path.Combine(path, "hotspots.json");
        string hotspotjsons1 = System.IO.File.ReadAllText(json_path1);
        HotspotDatas hotspotdatas1 = JsonUtility.FromJson<HotspotDatas>(hotspotjsons1);
        double largest_end = 10;
        foreach (string json in hotspotdatas1.hotspotdatas)
        {
            HotspotData h = JsonUtility.FromJson<HotspotData>(json);
            if (h.end_time > largest_end) largest_end = h.end_time;
        }
        videoLength = (float)largest_end + 20;

        foreach (string name in x)
        {
            if (System.IO.Path.GetExtension(name) == ".json") {
                string hotspotjsons = System.IO.File.ReadAllText(name);
                HotspotDatas hotspotdatas = JsonUtility.FromJson<HotspotDatas>(hotspotjsons);
                foreach (string json in hotspotdatas.hotspotdatas)
                {
                    HotspotData h = JsonUtility.FromJson<HotspotData>(json);
                    float xpos = (float)(startX+((endX-startX)*(h.start_time / videoLength)));
                    window_Graph.CreateCircle(new Vector2(xpos,stemY),h.name + "c");
                    if((h.url_video != null) && (h.url_video != "")) {
                        window_Graph.CreateDotConnection(new Vector2(xpos,stemY),new Vector2(xpos,stemY+gap), h.name);
                        window_Graph.CreateDotConnection(new Vector2(xpos,stemY+gap),new Vector2(endX,stemY+gap), h.name + "l");
                        draw(System.IO.Path.Combine(path,h.name),stemY+gap,(gap/2),xpos,endX);
                    }
                }
            }
        }
    }
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
        public HotspotData(double _start_time, double _end_time, string _name, string _text, string _url_photo, string _url_video,GameObject _hotspot)
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
    public float VideoLength(string path) {
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
}
