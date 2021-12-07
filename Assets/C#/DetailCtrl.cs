using UnityEngine;
using UnityEngine.UI;

public class DetailCtrl : MonoBehaviour
{
    GameObject obj;
    public GameObject detailPrefab, contentPanel;
    
    public string detail;
    public RectTransform scrollPanel, entryPanel;
    Vector2 pivotAbove = new Vector2(1,0), pivotBelow = new Vector2(1,1);


    public void Init(string detail, GameObject contentPanel, RectTransform scrollPanel, GameObject detailPrefab){
        this.contentPanel = contentPanel;
        this.scrollPanel = scrollPanel;
        this.detailPrefab = detailPrefab;
        this.detail = detail;
        createDetail();
    }

    public void setDetail(string detail){
        obj.GetComponent<Text>().text = detail; 
        this.detail = detail;
    }

    public void createDetail(){
        obj = Instantiate(detailPrefab, contentPanel.transform);
        obj.transform.Find("Text").gameObject.GetComponent<Text>().text = detail;
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(300, (detail.Length / 30) * 50 + 50);
        disableObject();
    }

    ///set the correct position for the detail panel
    public void enableObject(){
        float diff_y = entryPanel.position.y - scrollPanel.position.y;
        Vector3 pos = entryPanel.transform.localPosition;
        if(diff_y <= 0){
            obj.GetComponent<RectTransform>().pivot = pivotAbove;
            pos.y += 50;
        }
        else
        {
            obj.GetComponent<RectTransform>().pivot = pivotBelow;
            pos.y += -50;
        }
        obj.GetComponent<RectTransform>().localPosition = pos;
        obj.SetActive(true);
    }
    public void disableObject(){
        obj.SetActive(false);
    }
}
