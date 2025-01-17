using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AtlasPacker : EditorWindow
{
    int blockSize = 16; // Block size in pixels.
    int atlasSizeInBlocks = 16;
    int atlasSize;

    Object[] rawTextures = new Object[256];
    List<Texture2D> sortedTextures = new List<Texture2D>();
    Texture2D atlas;

    [MenuItem ("Minecraft Clone/AtlasPacker")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AtlasPacker));
    }

    private void OnGUI()
    {
        atlasSize = blockSize * atlasSizeInBlocks;

        GUILayout.Label("Minecraft Clone Texture Atlas Packer", EditorStyles.boldLabel);

        blockSize = EditorGUILayout.IntField("Block Size", blockSize);
        atlasSizeInBlocks = EditorGUILayout.IntField("Atlas Size (in blocks) ", atlasSizeInBlocks);

        GUILayout.Label(atlas);

        if (GUILayout.Button("Load Textures"))
        {
            LoadTextures();
            PackAtlas();

            Debug.Log("Atlas Packer: Textures loaded.");
        }

        if (GUILayout.Button("Clear Textures"))
        {
            atlas = new Texture2D(atlasSize, atlasSize);
            Debug.Log("Atlas Packer: Textures cleared.");
        }

        if (GUILayout.Button("Save Atlas"))
        {
            byte[] bytes = atlas.EncodeToPNG();

            try
            {
                File.WriteAllBytes(Application.dataPath + "/Textures/Packed_Atlas.png", bytes);

            }catch
            {
                Debug.Log("Atlas Packer: Couldn't save atlas to file.");
            }
        }

    }


    void LoadTextures()
    {
        sortedTextures.Clear();
        rawTextures = Resources.LoadAll("AtlasPacker", typeof(Texture2D));

        int index = 0;
        foreach( Object tex in rawTextures)
        {
            Texture2D t = (Texture2D)tex;
            if (t.width == blockSize && t.height == blockSize)
                sortedTextures.Add(t);
            else
                Debug.Log("Asset Packer: " + tex.name + "incorrect size. Texture not loaded.");

            index++;
        }

        Debug.Log("Atlas Packer:" + sortedTextures.Count + "successully loaded.");
    }

    void PackAtlas()
    {
        atlas = new Texture2D(atlasSize, atlasSize);
        Color[] pixels = new Color[atlasSize * atlasSize];

        for (int x =0; x < atlasSize; x++)
        {
            for (int y = 0; y < atlasSize; y++)
            {
                //Ger the curent block that we're looking at.
                int currentBlockX = x / blockSize;
                int currentBlockY = y / blockSize;

                int index = currentBlockY * atlasSizeInBlocks + currentBlockX;

                //Get the pixel in the current block
                int currentPixelX = x - (currentBlockX - blockSize);
                int currentPixelY = y - (currentBlockY - blockSize);

                if (index < sortedTextures.Count)
                    pixels[(atlasSize - y -1) * atlasSize + x] = sortedTextures[index].GetPixel(x, blockSize -y -1);
                else
                    pixels[(atlasSize - y - 1) * atlasSize + x] = new Color(0f, 0f, 0f, 0f);
            }
        }

        atlas.SetPixels(pixels);
        atlas.Apply();
    }
}

