using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.RenderGraphModule.Util.RenderGraphUtils;

public class SimpleSSRRenderPass : ScriptableRenderPass
{
    private SimpleSSRSettings settings;

    private const int NUMTHREAD_X = 8;
    private const int NUMTHREAD_Y = 8;

    private int threadGroupsX;
    private int threadGroupsY;

    private Material blitSSRMaterial;

    public class PassData
    {
        public TextureHandle ssrColorTexture;
        public TextureHandle gBuffer2Texture;
    }

    public SimpleSSRRenderPass(SimpleSSRSettings simpleSSRSettings)
    {
        settings = simpleSSRSettings;
        blitSSRMaterial = new Material(Shader.Find("Custom/BlitSSR"));

        threadGroupsX = Screen.width / NUMTHREAD_X;
        threadGroupsY = Screen.height / NUMTHREAD_Y;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        RenderTextureDescriptor descriptor = cameraData.cameraTargetDescriptor;
        descriptor.depthStencilFormat = GraphicsFormat.None;
        descriptor.msaaSamples = 1;
        descriptor.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;
        descriptor.enableRandomWrite = true;
        TextureHandle _SSRColorTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, descriptor, "_SSRColorTexture", true);

        using (var builder = renderGraph.AddComputePass<PassData>("Simple SSR", out var passData))
        {
            passData.ssrColorTexture = _SSRColorTexture;
            passData.gBuffer2Texture = resourceData.gBuffer[2];

            builder.UseTexture(passData.ssrColorTexture, AccessFlags.Write);
            builder.UseTexture(passData.gBuffer2Texture);

            builder.SetRenderFunc((PassData data, ComputeGraphContext context) =>
            {
                ComputeShader computeShader = settings.simpleSSRComputeShader;
                int kernelIndex = computeShader.FindKernel("SimpleSSR");

                context.cmd.SetComputeIntParam(computeShader, Shader.PropertyToID("_MaxSteps"), settings.maxSteps);
                context.cmd.SetComputeFloatParam(computeShader, Shader.PropertyToID("_RayOffset"), settings.rayOffset);
                context.cmd.SetComputeFloatParam(computeShader, Shader.PropertyToID("_StepSize"), settings.stepSize);
                context.cmd.SetComputeFloatParam(computeShader, Shader.PropertyToID("_Thickness"), settings.thickness);
                context.cmd.SetComputeTextureParam(computeShader, kernelIndex, "_SSRColorTexture", passData.ssrColorTexture);
                context.cmd.SetComputeTextureParam(computeShader, kernelIndex, "_GBuffer2Texture", passData.gBuffer2Texture);
                context.cmd.DispatchCompute(computeShader, kernelIndex, threadGroupsX, threadGroupsY, 1);
            });

            builder.SetGlobalTextureAfterPass(_SSRColorTexture, Shader.PropertyToID("_SSRColorTexture"));
        }

        TextureHandle blendColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, descriptor, "_BlendSSRColor", true);
        BlitMaterialParameters blitMaterialParameters = new BlitMaterialParameters(resourceData.activeColorTexture, blendColor, blitSSRMaterial, 0);
        renderGraph.AddBlitPass(blitMaterialParameters, "Blend SSR Color");
        resourceData.cameraColor = blendColor;
    }
}
