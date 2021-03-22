using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

//This file is used to make the editor to go to a sub-branch of the project when it's needed.
public class BranchProject : MonoBehaviour
{
    public InputPanelControl inputpanelcontrol;

    public Controller controller;
    public StatusController statusController;
    public PackageManager packageManager;
    public VideoManager videoManager;
    public Warning warningController;
    public TimelineController timelineController;

    public GameObject backButton;

    //Private states before branch
    private int floor_count;
    private long prev_video_frame;
    private Transform prev_tramsform;

    void Start()
    {
        backButton.SetActive(false);
    }

    public void create_branch() // Used by the create branch button on the input panel.
    {
        string current_id = inputpanelcontrol.getCurrentID();
        string hotspot_name = inputpanelcontrol.getName();
        string project_Path = Path.Combine(statusController.getPath(), hotspot_name);
        Directory.CreateDirectory(project_Path);
        packageManager.create_SubElems(project_Path);
        controller.recordBranch(current_id, hotspot_name);
        warningController.displayErrorMessage("Branch named " + hotspot_name + " is created.");
    }

    public void goToBranch() // Used by the Go To Branch button on the input panel.
    {
        string hotspot_name = inputpanelcontrol.getName();
        floor_count++;
        //Store status data before branch
        prev_video_frame = videoManager.getFrame();
        prev_tramsform = controller.getTransformByID(inputpanelcontrol.getCurrentID());
        inputpanelcontrol.saveHotspotInfo();
        controller.saveJsonWithoutNotice();
        videoManager.removeVideo();
        controller.removeAllHotspots();
        inputpanelcontrol.closeWindow();
        //Branch and load data
        statusController.branch_out(hotspot_name);
        string current_path = statusController.getPath();
        string mainVideo_path = Path.Combine(current_path, "MainVideo.mp4");
        Debug.Log(mainVideo_path);
        if (File.Exists(mainVideo_path)){
            videoManager.loadVideo(mainVideo_path);
            controller.please_load();
        }
        else
        {
            warningController.displayErrorMessage("Please add a Main Video.");
        }
        backButton.SetActive(true);   
    }

    //Used when clicking on the node on the timeline.
    public void branchOutbyNode(string relative_path, Vector3 location, double start_time)
    {
        
        string intendedPath = timelineController.getRoot() + relative_path;
        Debug.Log(intendedPath);
        if(!intendedPath.Equals(statusController.getPath()))
        {
            controller.saveJsonWithoutNotice();
            videoManager.removeVideo();
            controller.removeAllHotspots();
            statusController.branch_into(relative_path);
            Debug.Log(relative_path);
            string mainVideo_path = Path.Combine(statusController.getPath(), "MainVideo.mp4");
            videoManager.loadVideo(mainVideo_path);
            controller.please_load();
        }
        Camera.main.transform.LookAt(location);
        videoManager.goToPosinTime(start_time);
    }

    //Used by the back button when goes from the branch to the main video.
    public void backPreviousBranch()
    {
        controller.saveJsonWithoutNotice();
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