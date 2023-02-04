#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
 
public class SR_RenderCamera : MonoBehaviour {
 
    public string fileName = "0";
    
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            CamCapture();  
        }
    }
 
    [ContextMenu("Save")]
    public void CamCapture()
    {
        Camera Cam = GetComponent<Camera>();
 
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;
 
        Cam.Render();
 
        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;
 
        var Bytes = Image.EncodeToPNG();
        DestroyImmediate(Image);
 
        File.WriteAllBytes(Application.dataPath + "/Backgrounds/" + fileName + ".png", Bytes);

        AssetDatabase.Refresh();
    }
   
}
#endif