using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Events;

public class ScreencaptureManager : Singleton<ScreencaptureManager>
{
    private RenderTexture combinedRenderTexture;
    private Texture2D m_LastCameraTexture;
    [SerializeField] private Camera[] m_Cameras;
    [SerializeField] private RawImage m_PreviewImage;

    public void startCapture(int width, int height, UnityAction<Texture2D> onComplete)
    {
        IEnumerator coroutineCapture()
        {
            combinedRenderTexture = new RenderTexture(width, height, 0);

            foreach (var camera in m_Cameras)
            {
                var singleRenderTexture = new RenderTexture(width, height, 0);
                camera.targetTexture = singleRenderTexture;

                yield return coroutineReadPixels(singleRenderTexture);

                // Copy texture of single camera to combined render texture
                Graphics.Blit(m_LastCameraTexture, combinedRenderTexture);
                
                camera.targetTexture = null;
            }
            
            // Copy the RenderTexture from GPU to CPU
            yield return coroutineReadPixels(combinedRenderTexture);

            // m_PreviewImage.enabled = true;
            // m_PreviewImage.texture = m_LastCameraTexture;
            
            onComplete?.Invoke(m_LastCameraTexture);
        }

        StartCoroutine(coroutineCapture());
    }

    IEnumerator coroutineReadPixels(RenderTexture renderTexture)
    {
        yield return new WaitForEndOfFrame();
        
        // Copy the RenderTexture from GPU to CPU
        RenderTexture.active = renderTexture;
        if (m_LastCameraTexture == null)
            m_LastCameraTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, true);
        m_LastCameraTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        m_LastCameraTexture.Apply();
        RenderTexture.active = null;
    }
}
