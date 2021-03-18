using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;

public class InputPanelControl : MonoBehaviour
{
    //Controllers 
    public StatusController statusController;
    public PackageManager packageManager;
    public Controller controller;
    public VideoPlayer videoPlayer;

    //Panel 
    public GameObject inputPanel;

    //Displays
    public GameObject id_display;
    public GameObject photoUrlDisplay;
    public RawImage image;

    //Input fields
    public GameObject nameinputField;
    public GameObject textInputField;

    void Start()
    {
        inputPanel.SetActive(false);
    }

    public string getCurrentID() { return id_display.GetComponent<Text>().text;}

    public string getName() { return nameinputField.GetComponent<InputField>().text; }

    public void setEndTime() //Used by endtime button
    {
        string id = id_display.GetComponent<Text>().text; 
        controller.SetEndtime(id);
        closeWindow();
    }

    public void deleteHotspot() //Used by bin button
    {
        string id = id_display.GetComponent<Text>().text;
        controller.delete_hotspot(id);
        closeWindow();
    }

    public void closeWindow() //Used by x button
    {
        inputPanel.SetActive(false);
        videoPlayer.Play();
    }

    public void saveHotspotInfo() //Used by save button
    {
        string id = id_display.GetComponent<Text>().text;
        string name = nameinputField.GetComponent<InputField>().text;
        string text = textInputField.GetComponent<InputField>().text;
        string url_photo = photoUrlDisplay.GetComponent<Text>().text;

        if (url_photo != "")
        {

            if (!url_photo.Equals("Photo path here.") && Path.IsPathRooted(@url_photo))
            {
                url_photo = packageManager.copyFile(url_photo, name, "Pictures");
            }
            else
            { //Rename the photo
                string concrete_photo_path = Path.Combine(statusController.getPath(), url_photo);
                string filename_withoutext = Path.GetFileNameWithoutExtension(concrete_photo_path);
                if (filename_withoutext != name)
                {
                    string photo_exten = Path.GetExtension(concrete_photo_path);
                    string new_relative_path = Path.Combine("Pictures", name + photo_exten);
                    string new_filepath = Path.Combine(statusController.getPath(), new_relative_path);
                    File.Move(concrete_photo_path, new_filepath);
                    url_photo = new_relative_path;
                }
            }
        }

        controller.update_hotspot(id, name, text, url_photo);

        nameinputField.GetComponent<InputField>().text = "";
        textInputField.GetComponent<InputField>().text = "";
        photoUrlDisplay.GetComponent<Text>().text = "Photo path here.";

        closeWindow();
    }

    public void loadHotspot(string id, string hot_name, string hot_text, string hot_url_photo)
    {
        id_display.GetComponent<Text>().text = id;
        nameinputField.GetComponent<InputField>().text = hot_name;
        textInputField.GetComponent<InputField>().text = hot_text;
        photoUrlDisplay.GetComponent<Text>().text = hot_url_photo;
        //display Photo
        if (hot_url_photo != "" && hot_url_photo != null)
        {   
            string new_photo_url = System.IO.Path.Combine(statusController.getPath(), hot_url_photo);
            loadVideoOntoPanel(new_photo_url);
        }
        inputPanel.SetActive(true);
    }

    private void loadVideoOntoPanel(string videoURL)
    {
        WWW www = new WWW("file:///" + videoURL);
        image.texture = www.texture;
    }
}
