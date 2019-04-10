using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ImprintManager : MonoBehaviour
{
    //Quality & behaviour settings
    public int RenderSize_X = 256;
    public int RenderSize_Y = 256;
    public bool Refill = false;
    public float RefillSpeed = 0.1f;    
    public bool Elastic = false;

    //Ouputs & input
    public ComputeShader ComputeShader;
    public Camera OrthographicCamera;
    public MeshRenderer GroundRenderer;
    public MeshRenderer OptionalRenderer;

    //Internal component for refill effect
    private RenderTexture RenderTextureCamera;
    private RenderTexture OutputRenderTexture;
    private float Refilled = 0.0f;
    private const float MinColorInc = (1.0f / 256f) + 0.001f;

    /// <summary>
    /// First frame
    /// </summary>
    void Start()
    {
        //Verifications
        if (this.OrthographicCamera == null)
        {
            Debug.Log(this.name + " need a Camera !");
        }

        //Work with Depth directly
        if (this.Refill == false)
        {
            //Set up camera output
            this.RenderTextureCamera = new RenderTexture(this.RenderSize_X, this.RenderSize_Y, 16, RenderTextureFormat.Depth);
            this.RenderTextureCamera.filterMode = FilterMode.Trilinear;
            this.RenderTextureCamera.Create();
            this.OrthographicCamera.targetTexture = RenderTextureCamera;

            //Keep memory of imprint or immediatly go back into position ?
            if (this.Elastic == false)
            {
                this.OrthographicCamera.clearFlags = CameraClearFlags.Nothing;
            }
            else
            {
                this.OrthographicCamera.clearFlags = CameraClearFlags.Depth;
            }

            //Tie directly to the renderer(s)
            if (this.GroundRenderer == null)
            {
                Debug.Log(this.name + " need a MeshRenderer !");
            }
            else if (this.GroundRenderer.material.shader.name != "Custom/ImprintableGround")
            {
                Debug.Log(this.GroundRenderer.name + " need to use the 'Custom/ImprintableGround' shader !");
            }
            else
            {
                this.GroundRenderer.material.SetTexture("_HeightTex", RenderTextureCamera);
            }
            if (this.OptionalRenderer != null)
            {
                this.OptionalRenderer.material.mainTexture = RenderTextureCamera;
            }

            //Nothing more to do
            this.enabled = false;
        }
        //Work in RGB mode to allow image effects
        else
        {
            //Set up camera output
            this.RenderTextureCamera = new RenderTexture(this.RenderSize_X, this.RenderSize_Y, 16, RenderTextureFormat.Depth);
            this.RenderTextureCamera.filterMode = FilterMode.Trilinear;
            this.RenderTextureCamera.Create();
            this.OrthographicCamera.targetTexture = RenderTextureCamera;
            
            //Texture filled by the ComputeShader and displayed
            this.OutputRenderTexture = new RenderTexture(this.RenderSize_X, this.RenderSize_Y, 0);
            this.OutputRenderTexture.enableRandomWrite = true;
            this.OutputRenderTexture.Create();
            
            //Give the modified texture instead of directly the one from the camera     
            if (this.GroundRenderer == null)
            {
                Debug.Log(this.name + " need a MeshRenderer !");
            }
            else if (this.GroundRenderer.material.shader.name != "Custom/ImprintableGround")
            {
                Debug.Log(this.GroundRenderer.name + " need to use the 'Custom/ImprintableGround' shader !");
            }
            else
            {
                //this.GroundRenderer.material.SetTexture("_HeightTex", this.OutputTexture);
                this.GroundRenderer.material.SetTexture("_HeightTex", this.OutputRenderTexture);
            }
            if (this.OptionalRenderer != null)
            {
                //this.OptionalRenderer.material.mainTexture = this.OutputTexture;
                this.OptionalRenderer.material.mainTexture = this.OutputRenderTexture;
            }
        }
    }

    /// <summary>
    /// GameObject removed
    /// </summary>
    void OnDestroy()
    {
        this.RenderTextureCamera.Release();
    }
            
    /// <summary>
    /// Once per frame
    /// </summary>
    void Update()
    {
        //Color internally saved as RGBA32Bits -> 4xInt
        //Don't work well with small changes -> "big" change once every other frame
        this.Refilled += this.RefillSpeed * Time.deltaTime;        
        if (this.Refilled >= ImprintManager.MinColorInc)
        {
            this.Refilled -= ImprintManager.MinColorInc;
            this.ComputeShader.SetFloat("Reduce", ImprintManager.MinColorInc);
        }
        else
        {
            this.ComputeShader.SetFloat("Reduce", 0);
        }

        //Apply fresh heights
        this.ComputeShader.SetTexture(0, "New", this.RenderTextureCamera);
        this.ComputeShader.SetTexture(0, "Memory", this.OutputRenderTexture);
        this.ComputeShader.Dispatch(0, this.RenderSize_X / 8, this.RenderSize_Y / 8, 1);        
    }
}  
