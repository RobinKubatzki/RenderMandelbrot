using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

///<summary>this class is intended to handel a Parameter and implements an interface <c cref="ParameterManager">ParameterManager</c>
///</summary>
[Serializable]
public class ParameterItem<T> : ParameterManager
{
    private T data;
    private string name, detail;

    private UnityAction<Controller, T> setOnController;
    public ParameterItem(string name, string detail, UnityAction<Controller, T> setOnController){
        this.name = name;
        this.detail = detail;
        this.setOnController = setOnController;
    }
    public ParameterItem(string name, string detail, UnityAction<Controller, T> setOnController, T data) : this(name, detail, setOnController){
        this.data = data;
    }
    public string getDetail(){
        return detail;
    }

    public string getName(){
        return name;
    }
    public void setData(T data){
        this.data = data;
    }
    public T getData(){
        return data;
    }
    public void applieParameter(Controller ctrl){
        setOnController(ctrl, data);
    }
}
[Serializable]
public class Vector2_Serialize{
    public float x=0, y=0;
    public Vector2_Serialize(float x, float y){
        this.x = x;
        this.y = y;
    }
    public Vector2 getVal(){
        return new Vector2(x,y);
    }
    public void setVal(Vector2 val){
        x = val.x;
        y = val.y;
    }
}
///<summary>provides a manager to get the name and detail of a Parameter.
/// Also provides a way to applie the Parameter on a Controller</summary>
public interface ParameterManager{
    string getName();
    string getDetail();
    void applieParameter(Controller ctrl);
}