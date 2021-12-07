using UnityEngine;

public class ParameterInputUI<T> : ParameterInputUI
{
    private ParameterItem<T> param;
    private InputUIElement<T> inputUIElement;
    public ParameterInputUI(ParameterItem<T> parameterItem, InputUIElement<T> inputUIElement){
        this.param = parameterItem;
        this.inputUIElement = inputUIElement;
    }

    public void createUI(Transform parent)
    {
        inputUIElement.createUI(parent);
        inputUIElement.setOnEndEdit(this.updateData);
    }

    public void updateData()
    {
        Debug.Log("" + inputUIElement.getData());
        param.setData(inputUIElement.getData());
    }

    public void updateInputItem()
    {
        Debug.Log("" + param.getData().ToString());
        inputUIElement.setData(param.getData());
    }
}
public interface ParameterInputUI{
    void updateInputItem();
    void updateData();
    void createUI(Transform parent); 
}
public static class ParameterInputUIDefaults{
    public static ParameterInputUI<float> createFloat(ParameterItem<float> param, GameObject prefab){
        if(prefab == null)
            return null;
        return new ParameterInputUI<float>(param, InputUIElementDefaults.FloatInput.createObj(prefab));
    }
    public static ParameterInputUI<int> createInt(ParameterItem<int> param, GameObject prefab){
        if(prefab == null)
            return null;
        return new ParameterInputUI<int>(param, InputUIElementDefaults.IntInput.createObj(prefab));
    }
    public static ParameterInputUI<Vector2_Serialize> createVector2(ParameterItem<Vector2_Serialize> param, GameObject prefab){
        if(prefab == null)
            return null;
        return new ParameterInputUI<Vector2_Serialize>(param, InputUIElementDefaults.Vector2Input.createObj(prefab));
    }
}
