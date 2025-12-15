using UnityEditor;
using System.IO;

public class BuildAssetBundles
{
    private static string _outputPathAndroid = "AssetBundles/Android";

    [MenuItem("Tools/AssetBundles/Build for Android")]
    public static void BuildAllAssetBundlesForAndroid()
    {
        SetNameBundles();

        if (!Directory.Exists(_outputPathAndroid))
            Directory.CreateDirectory(_outputPathAndroid);

        BuildPipeline.BuildAssetBundles(
            _outputPathAndroid,
            BuildAssetBundleOptions.None,
            BuildTarget.Android
        );

        DeleteUnnecessaryFiles();
    }

    private static void SetNameBundles()
    {
        string basePath = "Assets/Scenarios";
        string[] scenarioFolders = Directory.GetDirectories(basePath);

        foreach (string scenarioFolder in scenarioFolders)
        {
            string scenarioName = Path.GetFileName(scenarioFolder);

            string[] assetPaths = AssetDatabase.FindAssets("t:Object", new[] { scenarioFolder });
            foreach (string guid in assetPaths)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string assetName = Path.GetFileNameWithoutExtension(assetPath);

                string resourceType = GetResourceType(assetName);

                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                if (importer != null)
                    importer.assetBundleName = $"{scenarioName}.{resourceType}";
            }
        }
        AssetDatabase.SaveAssets();
    }

    private static string GetResourceType(string assetName)
    {
        if (assetName.StartsWith("for_collect_image"))
             return "for_collect";
        else if (assetName.StartsWith("image") || assetName.StartsWith("info"))
            return "main_resources";
        else if(assetName.StartsWith("extra_image"))
            return "extra_images";
        else
            return "only_gameplay";
    }

    private static void DeleteUnnecessaryFiles()
    {
        string androidFile = Path.Combine(_outputPathAndroid, "Android");
        if (File.Exists(androidFile))
            File.Delete(androidFile);

        string[] files = Directory.GetFiles(_outputPathAndroid, "*.manifest");
        foreach (string file in files)
            File.Delete(file);
        AssetDatabase.Refresh();
    }
}