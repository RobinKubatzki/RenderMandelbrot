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

    void setPivotAbove(RectTransform rect){
        rect.pivot = pivotAbove;
    }
    void setPivotBelow(RectTransform rect){
        rect.pivot = pivotBelow;
    }
    public void enableObject(){
        float diff_y = entryPanel.position.y - scrollPanel.position.y;
        //Debug.Log("localPosition :" + entryPanel.localPosition.ToString() + ", " + obj.GetComponent<Transform>().localPosition.ToString());
        //Debug.Log("entryPanel width:" + entryPanel.rect.width);
        //setPivotAbove(entryPanel);
        //setPivotAbove(obj.GetComponent<RectTransform>());
        Vector3 pos = entryPanel.transform.localPosition;
        //pos.x += entryPanel.rect.width/2;
        Debug.Log("diff_y:" + diff_y);
        if(diff_y <= 0){
            setPivotAbove(obj.GetComponent<RectTransform>());
            pos.y += 50;
        }
        else
        {
            setPivotBelow(obj.GetComponent<RectTransform>());
            pos.y += -50;
        }
        obj.GetComponent<RectTransform>().localPosition = pos;
        //Debug.Log("entryPanel width:" + entryPanel.rect.width + ", position set to:" + pos.ToString());
        
        obj.SetActive(true);
    }
    public void disableObject(){
        obj.SetActive(false);
    }
}
