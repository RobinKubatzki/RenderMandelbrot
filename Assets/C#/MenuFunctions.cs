using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{


    public void startGame(){
        SceneManager.LoadScene(1);
    }
    public void settingsMenu(){
        SceneManager.LoadScene(2);
    }
    public void helpMenu(){
        SceneManager.LoadScene(3);
    }
    static Vector2 pivotAbove = new Vector2(1,0), pivotBelow = new Vector2(1,1);

    void setAncorAbove(RectTransform rect){
        rect.pivot = pivotAbove;
    }
    void setAncorBelow(RectTransform rect){
        rect.pivot = pivotBelow;
    }
    public void enableObject(GameObject obj, Transform contentPanel, Transform entryPanel){
        if(entryPanel.position.y - contentPanel.position.y >= 0){
            setAncorAbove(obj.GetComponent<RectTransform>());
        }
        else
        {
            setAncorBelow(obj.GetComponent<RectTransform>());
        }
        obj.SetActive(true);
    }
    public void disableObject(GameObject obj){
        obj.SetActive(false);
    }
}