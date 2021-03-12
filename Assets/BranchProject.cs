using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class BranchProject : MonoBehaviour
{
    public InputPanelControl inputpanelcontrol;

    public Controller controller;
    public StatusController statusController;
    public PackageManager packageManager;
    public VideoManager videoManager;
    public Warning warningController;

    public GameObject backButton;

    //Private states before branch
    private int floor_count;
    private long prev_video_frame;
    private Transform prev_tramsform;

    void Start()
    {
        backButton.SetActive(false);
    }

    public void create_branch()
    {
        string hotspot_name = inputpanelcontrol.getName();
        //string relative_path = Path.Combine("Branches", hotspot_name);
        string project_Path = Path.Combine(statusController.getPath(), hotspot_name);
        Directory.CreateDirectory(project_Path);
        packageManager.create_SubElems(project_Path);
        //Create the baseline for the new branch
    }

    public void goToBranch()
    {
        string hotspot_name = inputpanelcontrol.getName();
        floor_count++;
        prev_video_frame = videoManager.getFrame();
        prev_tramsform = controller.getTransformByID(inputpanelcontrol.getCurrentID());
        statusController.branch_out(hotspot_name);
        string current_path = statusController.getPath();
        string mainVideo_path = Path.Combine(current_path, "MainVideo.mp4");
        if (File.Exists(mainVideo_path)){
            videoManager.loadVideo(mainVideo_path);
            controller.please_load();
        }else
        {
            warningController.displayErrorMessage("Please add a Main Video.");
        }
        controller.saveJson();
        videoManager.removeVideo();
        controller.removeAllHotspots();
        inputpanelcontrol.closeWindow();
        backButton.SetActive(true);
    }

    public void backPreviousBranch()
    {
        controller.saveJson();
        controller.removeAllHotspots();
        floor_count--;
        statusController.branch_back();
        backtoPrevVideo(statusController.mainVideoPath(), prev_video_frame, prev_tramsform);
        if (floor_count == 0) { backButton.SetActive(false); }
    }

    private void backtoPrevVideo(string url_video, long prev_frame, Transform prev_hotspot)
    {
        videoManager.loadVideoAtFrame(prev_frame, url_video);
        Camera.main.transform.LookAt(prev_hotspot);
        controller.please_load();
    }
}