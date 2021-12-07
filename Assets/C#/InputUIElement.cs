using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

///<summary>provides methods to manage a input element of the UI</summary> 
public interface InputUIElement<T>
{
    T getData();
    void setData(T data);
    void createUI(Transform parent);
    void setOnEndEdit(UnityAction callback);
}
///<summary>contains methods to create default versions of a certain type</summary>
public static class InputUIElementDefaults
{
    public class FloatInput : InputUIElement<float>{
        GameObject prefab;
        InputField input;

        FloatInput(GameObject prefab){
            this.prefab = prefab;
        }
        public void createUI(Transform parent)
        {
            GameObject newObj = GameObject.Instantiate(prefab, parent);    
            input = newObj.GetComponent<InputField>();
        }

        public float getData()
        {
            return float.Parse(input.text);
        }

        public void setData(float data)
        {
            if(input == null){
                Debug.Log("InputElement not set");
            }
            input.text = "" + data;
        }
        public void setOnEndEdit(UnityAction callback){
            input.onEndEdit.AddListener((string s ) => callback());
        }
        public static InputUIElement<float> createObj(GameObject prefab){
            return new FloatInput(prefab);
        }
    }
    public class IntInput : InputUIElement<int>
    {
        GameObject prefab;
        InputField input;
        public IntInput(GameObject prefab){
            this.prefab = prefab;
        }

        public void createUI(Transform parent)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            input = obj.GetComponent<InputField>();
        }

        public int getData()
        {
            return int.Parse(input.text);
        }

        public void setData(int data)
        {
            input.text = "" + data;
        }
        public void setOnEndEdit(UnityAction callback){
            input.onEndEdit.AddListener((string s) => callback());
        }
        public static InputUIElement<int> createObj(GameObject prefab){
            return new IntInput(prefab);
        }
    }
    public class Vector2Input : InputUIElement<Vector2_Serialize>
    {
        InputUIElement<float> input1, input2;
        Vector2Input(GameObject prefab)
        {
            input1 = FloatInput.createObj(prefab);
            input2 = FloatInput.createObj(prefab);
        }

        public void createUI(Transform parent)
        {
            input1.createUI(parent);
            input2.createUI(parent);
        }

        public Vector2_Serialize getData()
        {
            return new Vector2_Serialize(input1.getData(), input2.getData());
        }

        public void setData(Vector2_Serialize data)
        {
            input1.setData(data.x);
            input2.setData(data.y);
        }
        public void setOnEndEdit(UnityAction callback){
            input1.setOnEndEdit(callback);
            input2.setOnEndEdit(callback);
        }
        public static InputUIElement<Vector2_Serialize> createObj(GameObject prefab){
            return new Vector2Input(prefab);
        }
    }
    
}

