using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_STANDALONE_WIN
using AnotherFileBrowser.Windows;
#endif

public class PackageManager : MonoBehaviour
{
    //Panels 
    public GameObject packageNameDisplayPanel;
    public GameObject packageNameText;
    public GameObject packagePathText;
    public GameObject packageNameInputPanel;
    public GameObject packageNameInputField;

    //Public Status Variable
    public bool path_ready;
    public bool is_new;

    //Controllers
    public GameObject videoManager;

    private string root_folder;
    private string project_name;
    private string package_path;

    void Start(){
        path_ready = false;
        root_folder = "";
        packageNameInputPanel.SetActive(false);
    }

    public void start_newProject(){
         is_new = true;
         browse_folder();
         packageNameInputPanel.SetActive(true); 
    }


    public void browse_folder(){
        var bp = new BrowserProperties();
        bp.filter = "txt files (*.txt)|*.txt|All Files (*.*)|*.*";
        bp.filterIndex = 0;

        new FileBrowser().OpenFolderBrowser(bp, path =>
        {
            root_folder = path;
            Debug.Log(path);
        });
    }

    public void save_name(){
        project_name = packageNameInputField.GetComponent<InputField>().text;
        packageNameText.GetComponent<Text>().text = project_name;
        Debug.Log(project_name);
        packageNameInputPanel.SetActive(false);
        create_folder();
    }

    public void create_folder(){
        package_path = System.IO.Path.Combine(root_folder, project_name);
        packagePathText.GetComponent<Text>().text = package_path;
        Debug.Log(package_path);
        System.IO.Directory.CreateDirectory(package_path);
        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(package_path, "Videos"));
        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(package_path, "Pictures"));
        string json_path = System.IO.Path.Combine(package_path, "hotspots.json");
        File.Create(json_path);
        path_ready = true;
    }

    public void addMainVideo(){
        if(is_new){
            videoManager.GetComponent<Video_Manager>().getVideo();
            string video_path = videoManager.GetComponent<Video_Manager>().url;
            System.IO.File.Copy(video_path, System.IO.Path.Combine(package_path, "MainVideo.mp4"), true);
        }
    }

}
