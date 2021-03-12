using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_Graph : MonoBehaviour
{
    public int xmin = 100;
    public int xmax = 1950;
    public int ymin = 50;
    public int ymax = 250;
    public RectTransform graphContainer;
    public Controller controller;
    public void Awake() {
        graphContainer = transform.Find("Graph_Container").GetComponent<RectTransform>();
    }
    public GameObject CreateCircle(Vector2 anchoredPosition, string name) {
        GameObject hi = new GameObject(name,typeof(Image));

        Sprite circle = Resources.Load<Sprite>("Circle");
        hi.GetComponent<Image>().sprite = circle;

        hi.AddComponent<Button>();
        hi.GetComponent<Button>().onClick.AddListener(delegate{openWindow1(hi.name);});

        hi.transform.SetParent(graphContainer);
        RectTransform rectTransform = hi.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);
        return hi;
    }
    public void openWindow1(string name) {
        string name2 = name.Remove(name.Length - 1, 1);  
        controller.openWindow(name2);
    }
    public void CreateDotConnection(Vector2 pos1 ,Vector2 pos2,string name) {
        GameObject hi = new GameObject(name,typeof(Image));
        hi.transform.SetParent(graphContainer, false);
        hi.GetComponent<Image>().color = new Color(1,1,1,.5f);
        RectTransform rectTransform = hi.GetComponent<RectTransform>();
        Vector2 dir = (pos2 - pos1).normalized;
        float dis = Vector2.Distance(pos1, pos2);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);
        rectTransform.sizeDelta = new Vector2(dis, 3f);
        rectTransform.anchoredPosition = pos1 + dir * dis * .5f;
        float angle = (dir.y < 0 ? -Mathf.Acos(dir.x) : Mathf.Acos(dir.x)) * Mathf.Rad2Deg;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }
    public void BranchLine(Vector2 pos1 ,Vector2 pos2) {
        Vector2 pos3 = new Vector2(pos1.x + (pos2.x - pos1.x) * .05f,pos2.y);
        CreateDotConnection(pos1, pos3, "l1");
        CreateDotConnection(pos3, pos2, "l2");
    }
    public float MainBranch() {
        CreateCircle(new Vector2(xmin, (ymax+ymin)/2),"c1");
        CreateCircle(new Vector2(xmax, (ymax+ymin)/2),"c2");
        CreateDotConnection(new Vector2(xmin, (ymax+ymin)/2),new Vector2(xmax, (ymax+ymin)/2),"l1");
        return ((ymax+ymin)/2);
    }
    public void NewBranch(float stemY, float xpos, Vector2 to_add) {
        Vector2 stem = new Vector2(xpos, stemY);
        CreateCircle(to_add, "c1");
        BranchLine(stem,to_add);
    }
    public void NewLeaf(float stemY, float xpos) {
        Vector2 stem = new Vector2(xpos, stemY);
        CreateCircle(new Vector2(xpos+100,stemY+100), "c1");
        CreateDotConnection(new Vector2(xpos, stemY),new Vector2(xpos+100,stemY+100),"l1");
    }
}

