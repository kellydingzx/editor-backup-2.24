using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPanel : MonoBehaviour
{

    public Image HelpImage;
    [SerializeField] private Sprite HelpImage_1;
    [SerializeField] private Sprite HelpImage_2;
    [SerializeField] private Sprite HelpImage_3;

    private int img = 1;
    void Start(){
        HelpImage.sprite = HelpImage_1;
    }

    public void NextImage()
    {
        if (img == 1) { 
            HelpImage.sprite = HelpImage_2;
            img = 2;
        } 
        else if (img == 2) { 
            HelpImage.sprite = HelpImage_3;
            img = 3;
        }
        else if (img == 3) { 
            HelpImage.sprite = HelpImage_1;
            img = 1;
        } 
    }
}
