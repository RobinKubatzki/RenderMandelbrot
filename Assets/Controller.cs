using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public float MIN_INTENSITY_OUTER_UNIT_CIRCLE = 0.01f;
    public Image overlay;
    public Text stateDisplay;
    public ComputeShader calcShad, initShad;
    public Vector2 viewCenter = new Vector2(0,0);
    //boundExponent: probably best to take a integer
    public float zoom = 2, boundExponent = 4; 
    public int colorPeriod = 10, iterationCnt = 0;
    public int targetFrameRate = 60, pixelSampleSize = 4;


    private RenderTexture mandelBrot, position, positionLength, mandelBrot_prevResult, mandelBrot_Result;
    private int kernel_calc, kernel_init, width, height;
    private uint threadGrpSize_x, threadGrpSize_y;
    private int dispatch_width, dispatch_heigth;//shaders should have same ThreadGrpSize, so one of these is enought.
    private float currentIterationVal;
    private bool debug_TexSettings = false, debug_iterationParams = false;

    float iterationVal(int cnt){
        float itLeft = Mathf.Cos( ((float)cnt-0.5f)*2.0f*Mathf.PI / (float)colorPeriod ) + 1;
        itLeft *= 0.5f;
        itLeft = MIN_INTENSITY_OUTER_UNIT_CIRCLE + (1-MIN_INTENSITY_OUTER_UNIT_CIRCLE)*itLeft;
        return (Mathf.Log(itLeft+1.0f)) / Mathf.Log(2.0f);
    }

    void createTextures(){
        mandelBrot = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        mandelBrot.enableRandomWrite = true;
        mandelBrot.filterMode = FilterMode.Bilinear;
        mandelBrot.Create();
        mandelBrot_prevResult = new RenderTexture(width, height, 0, RenderTextureFormat.RGFloat);
        mandelBrot_prevResult.enableRandomWrite = true;
        mandelBrot_prevResult.Create();
        mandelBrot_Result = new RenderTexture(width, height, 0, RenderTextureFormat.RGFloat);
        mandelBrot_Result.enableRandomWrite = true;
        mandelBrot_Result.Create();
        position = new RenderTexture(width, height, 0, RenderTextureFormat.RGFloat);
        position.enableRandomWrite = true;
        position.Create();
        positionLength = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat);
        positionLength.enableRandomWrite = true;
        positionLength.Create();
        
    }
    void initShaders(){
        kernel_calc = calcShad.FindKernel("CSMain");
        kernel_init = initShad.FindKernel("Init");
        calcShad.GetKernelThreadGroupSizes(kernel_calc,out threadGrpSize_x,out threadGrpSize_y, out _);
        dispatch_width = width/(int)threadGrpSize_x;
        dispatch_heigth = height/(int)threadGrpSize_y;
    }
    void setTex_InitShad(){
        initShad.SetTexture(kernel_init, "Position", position);
        initShad.SetTexture(kernel_init, "PositionLength", positionLength);
        initShad.SetTexture(kernel_init, "prevResult", mandelBrot_prevResult);
        initShad.SetTexture(kernel_init, "Color", mandelBrot);
    }
    void setParam_InitShad(){
        initShad.SetInt("width_inPixel", width);
        initShad.SetInt("height_inPixel", height);
        initShad.SetVector("mid_absolut", viewCenter);
        initShad.SetFloat("width_absolut", zoom);        
    }
    void refreshMandelBrot(){
        setParam_InitShad();
        dispatch_init();
        iterationCnt = 0;
    }
    
    void setTex_calcShad(){
        calcShad.SetTexture(kernel_calc, "Result", mandelBrot_Result);
        calcShad.SetTexture(kernel_calc, "prevResult", mandelBrot_prevResult);
        calcShad.SetTexture(kernel_calc, "Color", mandelBrot);
        calcShad.SetTexture(kernel_calc, "Position", position);
        calcShad.SetTexture(kernel_calc, "PositionLength", positionLength);
    }
    void setParam_calcShad(){
        calcShad.SetFloat("bound", Mathf.Pow(2, boundExponent));
        calcShad.SetFloat("bound_exponent", boundExponent);
        calcShad.SetInt("width", width);
    }
    void initData(){
        width = Screen.width*pixelSampleSize;
        height = Screen.height*pixelSampleSize;
        currentIterationVal = iterationVal(iterationCnt);
        Debug.Log("init with ( width:"+width+" , height:"+height+" )");
        createTextures();
        initShaders();
        setTex_InitShad();
        setParam_InitShad();
        setTex_calcShad();
        setParam_calcShad();
        dispatch_init();
    }
    void dispatch_init(){
        initShad.Dispatch(kernel_init, dispatch_width, dispatch_heigth, 1);
        Graphics.CopyTexture(position, mandelBrot_prevResult);
    }

    void dispatch_calc(){
        calcShad.SetFloat("iterationVal", currentIterationVal);
        string iterationDebugMsg = "";
        if(debug_iterationParams){
            iterationDebugMsg = iterationCnt%colorPeriod + "->("; 
            iterationDebugMsg += currentIterationVal + ", ";
        }
        currentIterationVal = iterationVal(++iterationCnt);
        if(iterationCnt == int.MaxValue)
            iterationCnt = int.MaxValue % colorPeriod;
        calcShad.SetFloat("iterationVal_next", currentIterationVal);
        if(debug_iterationParams){
            Debug.Log( iterationDebugMsg + currentIterationVal + ")");
        }
        calcShad.Dispatch(kernel_calc, dispatch_width, dispatch_heigth, 1);
        Graphics.CopyTexture(mandelBrot_Result, mandelBrot_prevResult); 
    }

    void Start()
    {
        initData();
        overlay.material.SetTexture("_MainTex", mandelBrot);
        if(debug_TexSettings){
            Debug.Log("filtermode:" + mandelBrot.filterMode);
            Debug.Log("antialiasing:" + mandelBrot.antiAliasing);
            Debug.Log("anisoLevel:" + mandelBrot.anisoLevel);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Vector3 mousePos = Input.mousePosition;
            float ScreenPixels_x = Screen.width;
            float ScreenPixels_y = Screen.height;
            float pixelsSize_absolute = zoom/ScreenPixels_x;
            viewCenter.x +=  (mousePos.x - 0.5f*ScreenPixels_x) * pixelsSize_absolute;
            viewCenter.y +=  (mousePos.y - 0.5f*ScreenPixels_y) * pixelsSize_absolute;
            zoom *= 0.5f;
            refreshMandelBrot();
            dispatch_init();
        }


        dispatch_calc();
        stateDisplay.text = ""+iterationCnt;
        
    }
}
