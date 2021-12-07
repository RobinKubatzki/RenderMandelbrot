using UnityEngine;	
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

///<summary>contains the inputelement and a Manager of a Parameter.</summary> 
public struct ParameterDependencies{
    public ParameterInputUI inputUI;
    public ParameterManager paramMngr;
    public ParameterDependencies(ParameterInputUI inputUI, ParameterManager paramMngr){
        this.inputUI = inputUI;
        this.paramMngr = paramMngr;
    }
}
///<summary> manages the settings menu, loading and saving of SettingsData, applying Settings of a Controller</summary>
public class SettingsManager : MonoBehaviour
{
    public GameObject defaultEntryPrefab, defaultDetailPrefab,
        defaultInputPrefab;
    public GameObject contentPanel;
    public RectTransform scrollPanel;

    static SettingsData data;
    List<ParameterDependencies> parametersDescription;

    static string dataPath;
    public void Start(){
        if(dataPath == null){
            dataPath = Application.persistentDataPath 
                 + "/MySaveData.dat";
        }
        loadData();
        createUI();
        foreach(ParameterDependencies item in parametersDescription){
            item.inputUI.updateInputItem();
        }
    }
    public void Update(){
        if(Input.GetKeyUp(KeyCode.Escape)){
            SceneManager.LoadScene(0);
        }
    }

    public GameObject createSettingsEntry(ParameterDependencies dependencies){
        return createSettingsEntry(contentPanel, scrollPanel, dependencies.paramMngr, dependencies.inputUI);
    }
    public GameObject createSettingsEntry(GameObject contentPanel, RectTransform scrollPanel, ParameterManager paramMngr, ParameterInputUI inputUI){
        return createSettingsEntry(defaultEntryPrefab, defaultDetailPrefab, contentPanel, scrollPanel, paramMngr, inputUI);
    }
    public GameObject createSettingsEntry(GameObject entryPrefab, GameObject detailPrefab, GameObject contentPanel, RectTransform scrollPanel, ParameterManager paramMngr, ParameterInputUI inputUI){
        GameObject entry = GameObject.Instantiate(entryPrefab, contentPanel.transform);
        entry.transform.Find("Name").Find("Text").gameObject.GetComponent<Text>().text = paramMngr.getName();
        entry.transform.SetAsFirstSibling();
        if(entry == null)
            Debug.Log("Name element not found");
        inputUI.createUI(entry.transform.Find("Input"));
        DetailCtrl detailCtrl = entry.transform.Find("Detail").gameObject.GetComponent<DetailCtrl>();
        if(detailCtrl == null)
            Debug.Log("DetailCtrl not found");
        detailCtrl.Init(paramMngr.getDetail(), contentPanel, scrollPanel, detailPrefab);
        return entry;
    }
    public void createUI(){
        parametersDescription = new List<ParameterDependencies>();
        foreach(ParameterDependencies item in data.getParametersDependencies(defaultInputPrefab)){
            createSettingsEntry(item);
            parametersDescription.Add(item);
        }
    }

    public void saveData(){
        BinaryFormatter bf = new BinaryFormatter(); 
	    FileStream file = File.Create(dataPath); 
	    bf.Serialize(file, data);
	    file.Close();
    }
    public static void loadData(){
        Debug.Log("load data");
        if (File.Exists(dataPath)){
            Debug.Log("load setting, path:" + dataPath);
		    BinaryFormatter bf = new BinaryFormatter();
		    FileStream file = 
                    File.Open(dataPath, FileMode.Open);
		    data = (SettingsData)bf.Deserialize(file);
		    file.Close();
        }
        else{
            data = new SettingsData();
            Debug.Log("no setting file found");
        } 
    }
    public static void applieSettings(Controller ctrl){
        if(data == null){
            loadData();
        }
        foreach(ParameterDependencies item in data.getParametersDependencies(null))
        {
            Debug.Log("set on controller: parameterName = " + item.paramMngr.getName());
            item.paramMngr.applieParameter(ctrl);
        }   
    }
}


///<summary>defines the parameters of settings.
///To add a new Parameter you only need to it as attribute of SettingsData and edit the getParametersDependencies method a bit.</summary> 
[Serializable]
public class SettingsData
{

    //add parameter here like below
    public ParameterItem<float> zoom = new ParameterItem<float>("Zoom", "width of view spectrum of the mandelbrot set", 
        (Controller ctrl, float data)=>
        {
            ctrl.zoom = data;
        },
        4);
    
    public ParameterItem<Vector2_Serialize> viewCenter = new ParameterItem<Vector2_Serialize>("View center", "the initial center of the mandelbrot view spectrum",
        (Controller ctrl, Vector2_Serialize viewCenter)=>
        {
            ctrl.viewCenter = viewCenter.getVal();
        },
        new Vector2_Serialize(0,0));
    public ParameterItem<int> 
        boundExponent = new ParameterItem<int>("Bound exponent", "a result of the mandelbrot function is considered as exeeded when it's length is bigger than 2 to the power of bound exponent."
            +"bound exponent should at least be 1",
            (Controller ctrl, int boundExponent)=>
            {
                ctrl.boundExponent = boundExponent;
            },
            6),
        colorPeriod = new ParameterItem<int>("Color period", "how much iterations of the mandelbrot-function are needed that the color is repeating",
            (Controller ctrl, int colorPeriod)=>
            {
                ctrl.colorPeriod = colorPeriod;
            },
            64);
    ///<summary>get managers of all parameters</summary>
    public List<ParameterDependencies> getParametersDependencies(GameObject inputPrefab){
        List<ParameterDependencies> list = new List<ParameterDependencies>();
        
        //add parameter to list like below
        list.Add(
            new ParameterDependencies(
                (ParameterInputUI) ParameterInputUIDefaults.createFloat(zoom, inputPrefab),
                (ParameterManager) zoom
            ));
        list.Add(
            new ParameterDependencies(
                (ParameterInputUI) ParameterInputUIDefaults.createInt(colorPeriod, inputPrefab),
                (ParameterManager) colorPeriod
            ));
        list.Add(
            new ParameterDependencies(
                (ParameterInputUI) ParameterInputUIDefaults.createInt(boundExponent, inputPrefab),
                (ParameterManager) boundExponent
            ));
        list.Add(
            new ParameterDependencies(
                (ParameterInputUI) ParameterInputUIDefaults.createVector2(viewCenter, inputPrefab),
                (ParameterManager) viewCenter
            ));
        return list;        
    }
}



