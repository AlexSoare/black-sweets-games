#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class TextureGenerator : MonoBehaviour
{
    // Path
    public string path;

    // Size of the texture
    public int textureSize = 256;

    // Rules for generating the texture
    public float frequency = 2.0f;
    public float amplitude = 1.0f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;
    public float offset = 0.0f;

    // Colors for the texture
    public Color color1 = Color.white;
    public Color color2 = Color.black;

    public UnityEngine.UI.Image image;

    [ContextMenu("Generate")]
    public void Generate()
    {
        // Create a new texture
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGB24, false);

        // Generate the texture
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                // Calculate the noise value for this pixel
                float noiseValue = Mathf.PerlinNoise((float)x / textureSize * frequency + offset, (float)y / textureSize * frequency + offset);

                // Apply the noise value and colors to the texture
                Color color = Color.Lerp(color1, color2, noiseValue);
                texture.SetPixel(x, y, color);
            }
        }

        // Apply the changes to the texture
        texture.Apply();

        // Save the texture as a PNG file
        byte[] bytes = texture.EncodeToPNG();
        string fileName = "Texture_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        System.IO.File.WriteAllBytes(Application.dataPath + path + fileName, bytes);

        AssetDatabase.Refresh();

        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        image.sprite = sprite;
    }
}
#endif