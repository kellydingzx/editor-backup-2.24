using System.Collections;
using System.Runtime;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Controller : MonoBehaviour
{
    //Panels
    public VideoPlayer videoPlayer;
    public GameObject window;
    //Input fields
    public GameObject nameinputField;
    public GameObject textInputField;
    //Buttons
    public GameObject hotspot;
    public GameObject back_button;
    //displays
    public GameObject id_display;
    public GameObject start_time;
    public GameObject photoDisplay;
    public GameObject videoDisplay;

    //Private variables 
    private Hashtable all_hotspots;
    private string current_id;

   
    void Start()
    {
        all_hotspots = load();
        window.SetActive(false);
        back_button.SetActive(false);
    }

    // Update is called once per frame
    void Update()
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
        }

        //View hotspot on left click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 100.0f)){
                if(hit.transform.gameObject.tag == "Trigger")
                {
                    window.SetActive(true);
                    current_id = hit.transform.gameObject.GetInstanceID().ToString();
                    id_display.GetComponent<Text>().text = current_id;
                    Hotspot h = (Hotspot)all_hotspots[current_id];
                    start_time.GetComponent<Text>().text = h.getStart().ToString();
                    videoPlayer.Pause();
                }
            }
        }

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

    public void saveJson()
    {
        save(all_hotspots);
    }

    public void closeWindow()
    {
        window.SetActive(false);
        videoPlayer.Play();
    }

    public void SetEndtime()
    {
        Hotspot a = (Hotspot)all_hotspots[id_display.GetComponent<Text>().text];
        a.SetEndTime(videoPlayer.time);
    }

    public void delete_hotspot()
    {
        string id = id_display.GetComponent<Text>().text;
        Hotspot h = (Hotspot)all_hotspots[id];
        GameObject a = h.getHotspot();
        all_hotspots.Remove(id);
        Destroy(a);
        window.SetActive(false);
        videoPlayer.Play();
    }
    public void ClickSave()
    {
        string name = nameinputField.GetComponent<InputField>().text;
        string text = textInputField.GetComponent<InputField>().text;
        string url_photo = photoDisplay.GetComponent<Text>().text;
        string url_video = videoDisplay.GetComponent<Text>().text;

        Hotspot h = (Hotspot)all_hotspots[current_id];
        h.SetMoreInfo(name, text, url_photo, url_video);
        print(h.getName());
        all_hotspots[current_id] = h;
        print(((Hotspot)all_hotspots[current_id]).getName());

        Debug.Log(name);
        Debug.Log(text);
        Debug.Log(url_photo);
        Debug.Log(url_video);

        nameinputField.GetComponent<InputField>().text = "";
        textInputField.GetComponent<InputField>().text = "";
        photoDisplay.GetComponent<Text>().text = "";
        videoDisplay.GetComponent<Text>().text = "";

        closeWindow();
    }
    class Hotspot
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

        public void SetMoreInfo(string name, string text, string url_photo, string url_video)
        {
            if(!name.Equals("Hotspot Name")) { this.name = name;}
            if(!text.Equals("Description")) { this.text = text; }
            if (!url_photo.Equals("Photo path here")) { this.url_photo = url_photo;}
            if (!url_video.Equals("Video path here.")) { this.url_video = url_video; }
                
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
    public void save(Hashtable a)
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
        //Debug.Log(json);
        File.WriteAllText(Application.dataPath + "/hotspots.json", json);
    }
    public Hashtable load()
    {
        Hashtable r = new Hashtable();
        string hotspotjsons = File.ReadAllText(Application.dataPath + "/hotspots.json");
        HotspotDatas hotspotdatas = JsonUtility.FromJson<HotspotDatas>(hotspotjsons);
        foreach (string json in hotspotdatas.hotspotdatas)
        {
            HotspotData h1 = JsonUtility.FromJson<HotspotData>(json);
            GameObject a = Instantiate(hotspot, h1.worldPosition, h1.rot);
            r.Add(a.GetInstanceID().ToString(), new Hotspot(a, h1.start_time, h1.end_time));
        }
        return r;
    }

    
}