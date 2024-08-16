using UnityEngine;
using System.Collections;
using UnityEditor;

public class ReplaceWithPrefab : MonoBehaviour
{
    [MenuItem("Tools/Replace Selected with Prefab")]
    static void ReplaceSelectedWithPrefab()
    {
        // 프로젝트 폴더에서 원하는 프리팹을 로드합니다.
        GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Src/Prefeps/SlotPrefab.prefab", typeof(GameObject));
        if (prefab == null)
        {
            Debug.LogError("Prefab not found at specified path. Please check the path.");
            return;
        }
        // Hierarchy에서 선택된 모든 오브젝트를 가져옵니다.
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject selected in selectedObjects)
        {
            // 선택된 오브젝트를 Prefab으로 대체합니다.
            GameObject newPrefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            newPrefabInstance.transform.SetParent(selected.transform.parent);
            newPrefabInstance.transform.position = selected.transform.position;
            newPrefabInstance.transform.rotation = selected.transform.rotation;
            newPrefabInstance.transform.localScale = selected.transform.localScale;

            // 기존 오브젝트 삭제
            DestroyImmediate(selected);
        }
    }
}