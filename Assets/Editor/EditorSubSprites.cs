using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExportSubSprites : Editor
{

    [MenuItem("Assets/Export Sub-Sprites")]
    public static void DoExportSubSprites()
    {
        var folder = EditorUtility.OpenFolderPanel("Export subsprites into what folder?", "", "");
        foreach (var obj in Selection.objects)
        {
            var sprite = obj as Sprite;
            if (sprite == null) continue;
            var extracted = ExtractAndName(sprite);
            SaveSubSprite(extracted, folder);
        }

    }

    [MenuItem("Assets/Export Sub-Sprites", true)]
    private static bool CanExportSubSprites()
    {
        return Selection.activeObject is Sprite;
    }

    // Since a sprite may exist anywhere on a tex2d, this will crop out the sprite's claimed region and return a new, cropped, tex2d.
    private static Texture2D ExtractAndName(Sprite sprite)
    {
        var output = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        var r = sprite.textureRect;
        var pixels = sprite.texture.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
        output.SetPixels(pixels);
        output.Apply();
        output.name = sprite.texture.name + " " + sprite.name;
        return output;
    }

    private static void SaveSubSprite(Texture2D tex, string saveToDirectory)
    {
        if (!System.IO.Directory.Exists(saveToDirectory)) System.IO.Directory.CreateDirectory(saveToDirectory);
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(saveToDirectory, tex.name + ".png"), tex.EncodeToPNG());
    }
}