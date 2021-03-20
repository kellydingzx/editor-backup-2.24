using System.IO.Compression;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Controller : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    //Controllers
    public StatusController statusController;
    public Warning warningController;
    public InputPanelControl inputPanelController;

    //Buttons
    public GameObject hotspot;
    
    public Window_Graph window_Graph;
    public TimelineController timelineController;
    
    public string current_id;

    public bool ready_to_load;

    private Hashtable all_hotspots;
    private bool hotspots_loaded;
   
    void Start()
    {
        Debug.Log(statusController.getPath());
    }

    void Update(){

        if (videoPlayer.isPrepared && (videoPlayer.length != 0))
        {
            addingAndViewingHotspots();
        }
        
        if(videoPlayer.length != 0 && ready_to_load && !hotspots_loaded)
        {
            load();
        }

        if (hotspots_loaded){ checkHotspotValidity();}
    }

    public void please_load() { ready_to_load = true; Debug.Log("please load"); }

    void addingAndViewingHotspots()
    {
        //Add Hotspot on right click
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,20));
            double start_time = videoPlayer.time;
            GameObject a = Instantiate(hotspot, worldPosition, transform.rotation);
            a.name = a.GetInstanceID().ToString();
            all_hotspots.Add(a.name, new Hotspot(a, start_time,videoPlayer.length));
            a.SetActive(true);
            //window_Graph.CreateDotConnection(new Vector2((float)((videoPlayer.time/videoPlayer.length)*1850+100), 150),new Vector2((float)((videoPlayer.time/videoPlayer.length)*1850+100)+100,150+100),a.GetInstanceID().ToString());
            //window_Graph.CreateCircle(new Vector2((float)((videoPlayer.time/videoPlayer.length)*1850+100)+100,150+100),a.GetInstanceID().ToString()+"c");
            window_Graph.ClearGraph();
            window_Graph.MainBranch("main");
            timelineController.draw(statusController.getPath(), 150, 100, 100, 1950);
        }

        //View hotspot on left click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit)){
                if(hit.transform.gameObject.tag == "Trigger")
                { 
                    current_id = hit.transform.gameObject.GetInstanceID().ToString();
                    openWindow(current_id);
                }
            }
        }
    }

    public void checkHotspotValidity() //check the validity of hotspot in terms of time
    {
        double current_time = videoPlayer.time;
        foreach (DictionaryEntry entry in all_hotspots)
        {
            Hotspot h = (Hotspot)entry.Value;
            if (h.getStart() >= current_time || h.getEnd() <= current_time)
            {
                h.getHotspot().SetActive(false);
            }
            else
            {
                h.getHotspot().SetActive(true);
            }
        }
    }

    public void recordBranch(string id)
    {
        Hotspot current_hotspot = (Hotspot)all_hotspots[id];
        current_hotspot.SetBranch();
        all_hotspots[id] = current_hotspot;
    }

    public void openWindow(string id) {      
        Hotspot current_hotspot = (Hotspot)all_hotspots[id];
        inputPanelController.loadHotspot(id, current_hotspot.getName(), current_hotspot.getText(), current_hotspot.getUrl_photo());
        goToHotspot(current_hotspot);
        videoPlayer.Pause();
    }

    public void goToHotspot(Hotspot hs)
    {
        double hs_start_time = hs.getStart();
        videoPlayer.Prepare();
        Debug.Log(videoPlayer.frameCount);
        Debug.Log(videoPlayer.frameRate);
        long location_frame = Convert.ToInt64(hs_start_time / (videoPlayer.frameCount / videoPlayer.frameRate)* videoPlayer.frameCount)+5;
        videoPlayer.frame = location_frame;
        Debug.Log(location_frame);
        Camera.main.transform.LookAt(hs.getHotspot().transform);
    }

    public Transform getTransformByID(string hotspot_id)
    {
        return ((Hotspot)all_hotspots[hotspot_id]).getHotspot().transform;
    }

    public void saveJson()
    {
        save(all_hotspots, statusController.getPath());
        warningController.displayErrorMessage("Saved.");
    }


    public void SetEndtime(string id)
    {
        Hotspot a = (Hotspot)all_hotspots[id];
        a.SetEndTime(videoPlayer.time);
    }

    public void update_hotspot(string id, string name, string text, string url_photo)
    {
        Hotspot current_hotspot = (Hotspot)all_hotspots[id];
        Debug.Log(id);
        current_hotspot.SetMoreInfo(name, text, url_photo);
        all_hotspots[id] = current_hotspot;
    }

    public void delete_hotspot(string id)
    {
        Hotspot h = (Hotspot)all_hotspots[id];
        GameObject a = h.getHotspot();
        all_hotspots.Remove(id);
        Destroy(a);
        GameObject b = GameObject.Find("/Canvas/Window_Graph/Graph_Container/" + id);
        Destroy(b);
        GameObject c = GameObject.Find("/Canvas/Window_Graph/Graph_Container/" + id + "c");
        Destroy(c);
        videoPlayer.Play();
    }

    public void removeAllHotspots()
    {
        hotspots_loaded = false;
        ready_to_load = false;
        List<String> needs_removed = new List<string>();
        foreach (DictionaryEntry entry in all_hotspots)
        {
            Hotspot h = (Hotspot)all_hotspots[entry.Key.ToString()];
            GameObject a = h.getHotspot();
            Destroy(a);
        }
        all_hotspots.Clear();
    }

    void OnApplicationQuit()
    {
        //if(statusController.path_ready()){ saveJson();}
    }

    public class Hotspot
    {
        GameObject hotspot;
        double start_time;
        double end_time;
        string name;
        string text;
        string url_photo;
        string url_video;

        public Hotspot(GameObject a, double start, double end)
        {
            this.hotspot = a;
            this.start_time = start;
            this.end_time = end;
        }

         public Hotspot(GameObject a, double start, double end, string name, string text, string url_photo, string url_video)
        {
            this.hotspot = a;
            this.start_time = start;
            this.end_time = end;
            this.name = name;
            this.text = text;
            this.url_photo = url_photo;
        }
        public double getStart()
        {
            return start_time;
        }
        public double getEnd()
        {
            return end_time;
        }
        public string getName()
        {
            return name;
        }
        public string getText()
        {
            return text;
        }
        public string getUrl_photo()
        {
            return url_photo;
        }
        public string getUrl_video()
        {
            return url_video;
        }
        public GameObject getHotspot()
        {
            return hotspot;
        }
        public void SetEndTime(double endtime)
        {
            this.end_time = endtime;
        }

        public void SetMoreInfo(string name, string text, string url_photo)
        {
            if(!name.Equals("Hotspot Name")) { this.name = name;}
            if(!text.Equals("Description")) { this.text = text; }
            if (!url_photo.Equals("Photo path here.") && url_photo != null){ 
                this.url_photo = url_photo;
            }
        }

        public void SetBranch()
        {
            this.url_video = this.name;
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
    public void save(Hashtable a, string json_folder)
    {
        Debug.Log("Saving");
        List<string> jsonlist = new List<string>();
        foreach (DictionaryEntry entry in a)
        {
            Hotspot h1 = (Hotspot)entry.Value;
            HotspotData h2 = new HotspotData(h1.getStart(),h1.getEnd(),h1.getName(),h1.getText(),h1.getUrl_photo(),h1.getUrl_video(),h1.getHotspot());
            string h3 = JsonUtility.ToJson(h2);
            jsonlist.Add(h3);
        }
        string[] jsons = new string[jsonlist.Count];
        jsons = jsonlist.ToArray();
        HotspotDatas hotspotdatas = new HotspotDatas() { hotspotdatas = jsons };
        string json = JsonUtility.ToJson(hotspotdatas);
        string json_path = System.IO.Path.Combine(json_folder, "hotspots.json");
        File.WriteAllText(json_path, json);
    }

    public void load()
    {
        all_hotspots = new Hashtable();
        string json_path = System.IO.Path.Combine(statusController.getPath(), "hotspots.json");
        Debug.Log(json_path);
        if (File.Exists(json_path))
        {
            string hotspotjsons = File.ReadAllText(json_path);
            HotspotDatas hotspotdatas = JsonUtility.FromJson<HotspotDatas>(hotspotjsons);
            foreach (string json in hotspotdatas.hotspotdatas)
            {
                HotspotData h1 = JsonUtility.FromJson<HotspotData>(json);
                GameObject a = Instantiate(hotspot, h1.worldPosition, h1.rot);
                a.name = a.GetInstanceID().ToString();
                all_hotspots.Add(a.GetInstanceID().ToString(), new Hotspot(a, h1.start_time, h1.end_time, h1.name, h1.text, h1.url_photo, h1.url_video));
                //window_Graph.CreateDotConnection(new Vector2((float)((h1.start_time / videoPlayer.length) * 1850 + 100), 150), new Vector2((float)((h1.start_time / videoPlayer.length) * 1850 + 100) + 100, 250), a.GetInstanceID().ToString());
                //window_Graph.CreateCircle(new Vector2((float)((h1.start_time / videoPlayer.length) * 1850 + 100) + 100, 150 + 100), a.GetInstanceID().ToString() + "c");
            }
            hotspots_loaded = true;
        }
    }
   
    /*
    public void loadCustom(string path) {
        ZipFile.ExtractToDirectory(path, Path.GetDirectoryName(path));
    }
   
   */
}