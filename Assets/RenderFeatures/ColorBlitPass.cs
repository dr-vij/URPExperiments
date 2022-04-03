using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class ColorBlitPass : ScriptableRenderPass
{
    private Material m_Material;
    private RTHandle m_CameraColorTarget;
    private float m_Intensity;

    public ColorBlitPass(Material material)
    {
        m_Material = material;
        UpdateIntensity();
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void SetIntensity(float intensity)
    {
        m_Intensity = intensity;
        UpdateIntensity();
    }

    public void SetTarget(RTHandle colorHandle)
    {
        m_CameraColorTarget = colorHandle;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        //Nothing here yet.
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        Debug.Log("Ref size " + m_CameraColorTarget.referenceSize);
        Debug.Log("Scaled size " + m_CameraColorTarget.GetScaledSize(m_CameraColorTarget.referenceSize));
        Debug.Log("---");
        var camera = renderingData.cameraData.camera;
        if (camera.cameraType != CameraType.Game || m_Material == null)
            return;

        var cmd = CommandBufferPool.Get();
        Blitter.BlitTexture(cmd, m_CameraColorTarget, new Vector4(1,1,0,0), m_Material, 0);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
    }

    private void UpdateIntensity()
    {
        if (m_Material != null)
            m_Material.SetFloat("_Intensity", m_Intensity);
    }
}