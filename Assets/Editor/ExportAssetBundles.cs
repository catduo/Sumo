using UnityEngine;
using UnityEditor;
public class ExportAssetBundles {
	[MenuItem("Assets/Build Android Bundle")]
	static void ExportResourceAndroid () {
		// Bring up save panel
		string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
		if (path.Length != 0) {
			// Build the resource file from the active selection.
			Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
			BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, 
			                               BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, targetPlatform: BuildTarget.Android);
			Selection.objects = selection;
		}
	}
	[MenuItem("Assets/Build iOS Bundle")]
	static void ExportResourceIOS () {
		// Bring up save panel
		string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
		if (path.Length != 0) {
			// Build the resource file from the active selection.
			Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
			BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, 
			                               BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, targetPlatform: BuildTarget.iPhone);
			Selection.objects = selection;
		}
	}
	[MenuItem("Assets/Build Others Bundle")]
	static void ExportResourceOthers () {
		// Bring up save panel
		string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
		if (path.Length != 0) {
			// Build the resource file from the active selection.
			Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
			BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, 
			                               BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, targetPlatform: BuildTarget.StandaloneWindows);
			Selection.objects = selection;
		}
	}
}