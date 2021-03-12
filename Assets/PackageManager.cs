﻿using System.Collections;
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
    //Panels used
    public GameObject packageControlPanel;
    public GameObject packageNameInputPanel;
    public GameObject packageNameInputField;

    //Controllers
    public StatusController statusController;
    public VideoManager videoManager;
    public Warning warningController;
    public Controller controller;
    
    private string root_folder;

    void Start(){
        packageNameInputPanel.SetActive(false);
        packageControlPanel.SetActive(false);
    }

    public void start_newProject(){
         root_folder = browse_folder();
         packageNameInputPanel.SetActive(true); 
         packageControlPanel.SetActive(false);
    }

    public void save_name()
    {
        string project_name = packageNameInputField.GetComponent<InputField>().text;
        packageNameInputPanel.SetActive(false);
        string project_path = create_folder(project_name);
        statusController.setNameAndPath(project_name, project_path);
    }

    private string create_folder(string project_name)
    {
        string project_path = System.IO.Path.Combine(root_folder, project_name);
        System.IO.Directory.CreateDirectory(project_path);
        create_SubElems(project_path);
        return project_path;
    }

    public void start_existProject(){
        string project_path = browse_folder();
        string project_name = Path.GetFileName(project_path);
        statusController.setNameAndPath(project_name, project_path);
        Debug.Log(project_name);
        string main_video_path = System.IO.Path.Combine(project_path,"MainVideo.mp4");
        if(File.Exists(@main_video_path))
        {
            videoManager.loadVideo(main_video_path);
            controller.please_load();
        }else{
            warningController.displayErrorMessage("Please add a main video.");
        } 
    }

    public void create_SubElems(string folder_url){
        //string videos_folder_path = System.IO.Path.Combine(folder_url, "Branches");
        //if(!Directory.Exists(@videos_folder_path)){
        //    System.IO.Directory.CreateDirectory(videos_folder_path);
        //}
        string pics_folder_path = System.IO.Path.Combine(folder_url, "Pictures");
        if(!Directory.Exists(@pics_folder_path)){
            System.IO.Directory.CreateDirectory(pics_folder_path);
        }
        string json_path = System.IO.Path.Combine(folder_url, "hotspots.json");
        if(!File.Exists(@json_path)){           
            File.Create(json_path).Dispose();
            controller.save(new Hashtable(), folder_url);
        }
    }

    public void open_package_control_panel(){
        packageControlPanel.SetActive(true);
    }

    public string addMainVideo(string url){
        string new_path = System.IO.Path.Combine(statusController.getPath(), "MainVideo.mp4");
        System.IO.File.Copy(url, new_path, true);
        packageControlPanel.SetActive(false);
        return new_path;
    }

    public string copyFile(string original_url, string new_name, string folder_name){
        string file_folder_path = System.IO.Path.Combine(statusController.getPath(), folder_name);
        string new_file_name = new_name+System.IO.Path.GetExtension(original_url);
        string new_file_url = System.IO.Path.Combine(file_folder_path, new_file_name);
        System.IO.File.Copy(original_url, new_file_url, true);
        Debug.Log(System.IO.Path.Combine(folder_name, new_file_name));
        return System.IO.Path.Combine(folder_name, new_file_name);
    }

    public string browse_folder()
    {
        var bp = new BrowserProperties();
        bp.filter = "txt files (*.txt)|*.txt|All Files (*.*)|*.*";
        bp.filterIndex = 0;

        string selected_path = "";
        new FileBrowser().OpenFolderBrowser(bp, path =>
        {
            selected_path = path;
        });

        return selected_path;
    }

}
