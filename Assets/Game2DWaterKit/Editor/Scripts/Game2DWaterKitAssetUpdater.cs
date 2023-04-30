using UnityEditor;

[InitializeOnLoad]
public class Game2DWaterKitAssetUpdater
{
    private static string[] urp_shaders_guids = new string[]
    {
        "ba8d3b622257d1c438eec20608c4e43c",
        "e886a16a9b070044382af4ae00cba832",
        "3c987857323fdf5478ae141907bf4ca7",
        "38990091021a5fb4fb256778330bd63c"
    };

    private static string[] lwrp_shaders_guids = new string[]
    {
        "82fb9fa3c2eba5e4da3ab447cbbefcfd",
        "ed71b189d390d3e488c2da97d5d74a5f",
        "0e0b0aefa92ae5c4d81e2b6158a92fd2",
        "81c2f2b3f192a9246844e5b43a907b04"
    };

#if UNITY_2019_1_OR_NEWER
    const string LWRP_SHADERS_PACKAGE_GUID = "bec4a79152db4a84799833513c8eba9f";
    const string URP_SHADERS_PACKAGE_GUID = "84c4cf967e584394795bfc5ebae62b25";

    const string LWRP_SCRIPTING_DEFINE_SYMBOL = "GAME_2D_WATER_KIT_LWRP";
    const string URP_SCRIPTING_DEFINE_SYMBOL = "GAME_2D_WATER_KIT_URP";
#endif

    static Game2DWaterKitAssetUpdater()
    {
        AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
        AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
    }

    private static void OnImportPackageCompleted(string packageName)
    {
        if (packageName != "Game 2D Water Kit" && packageName != "Game2DWaterKit")
            return;

        bool isBuiltinRenderPipeline;

#if UNITY_2019_1_OR_NEWER
        string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        isBuiltinRenderPipeline = !scriptingDefineSymbols.Contains(LWRP_SCRIPTING_DEFINE_SYMBOL) && !scriptingDefineSymbols.Contains(URP_SCRIPTING_DEFINE_SYMBOL);
#else
        isBuiltinRenderPipeline = true;
#endif

        if (isBuiltinRenderPipeline)
        {
            DeleteSRPShadersIfExist(lwrp_shaders_guids);
            DeleteSRPShadersIfExist(urp_shaders_guids);
        }

#if UNITY_2019_1_OR_NEWER
        if (!isBuiltinRenderPipeline)
        {
            bool isURP = scriptingDefineSymbols.Contains(URP_SCRIPTING_DEFINE_SYMBOL);

            DeleteSRPShadersIfExist(isURP ? lwrp_shaders_guids : urp_shaders_guids); // Delete the other SRP shaders if exist

            ImportSRPShadersPackage(isURP ? URP_SHADERS_PACKAGE_GUID : LWRP_SHADERS_PACKAGE_GUID); // Update current SRP shaders
        }
#endif
    }

#if UNITY_2019_1_OR_NEWER
    private static void ImportSRPShadersPackage(string packageGUID)
    {
        AssetDatabase.ImportPackage(AssetDatabase.GUIDToAssetPath(packageGUID), false);
    }
#endif

    private static void DeleteSRPShadersIfExist(string[] shadersGUIDs)
    {
        foreach (var shaderGUID in shadersGUIDs)
        {
            string shaderPath = AssetDatabase.GUIDToAssetPath(shaderGUID);

            if (shaderPath != string.Empty && shaderPath.Contains("Game2DWaterKit"))
                AssetDatabase.DeleteAsset(shaderPath);
        }
    }
}
