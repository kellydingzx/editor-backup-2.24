using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Compression;
using System.IO;
using UnityEngine.Video;

public class FinalDelivery : MonoBehaviour
{
    public StatusController statusController;
    public VideoPlayer videoPlayer;
    public void saveCustom()
    {
        videoPlayer.Stop();
        string zip_path = Directory.GetParent(statusController.getRootProjectPath()).FullName;
        string zip_file_fullpath = Path.Combine(zip_path, statusController.getRootProjectName() + ".zip");
        Debug.Log(zip_file_fullpath);
        ZipFile.CreateFromDirectory(zip_path, zip_file_fullpath, System.IO.Compression.CompressionLevel.Fastest, false);
    }

}
