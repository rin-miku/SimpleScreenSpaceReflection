using System;
using UnityEngine;

[Serializable]
public class SimpleSSRSettings
{
    public int maxSteps = 512;
    public float rayOffset = 0.2f;
    public float stepSize = 0.01f;
    public float thickness = 0.001f;
    public ComputeShader simpleSSRComputeShader;
}
