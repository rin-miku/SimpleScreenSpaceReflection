using UnityEngine.Rendering.Universal;

public class SimpleSSRRendererFeature : ScriptableRendererFeature
{
    public SimpleSSRSettings settings;

    private SimpleSSRRenderPass simpleSSRRenderPass;

    public override void Create()
    {
        simpleSSRRenderPass = new SimpleSSRRenderPass(settings);
        simpleSSRRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(simpleSSRRenderPass);
    }
}
