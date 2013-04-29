using UnityEngine;
using System.Collections;

public delegate void ClickDelegate(string text);

public class Text : MonoBehaviour{
    ClickDelegate clickDelegate;
    static GameObject textMeshLPrefab;
    static GameObject textMeshRPrefab;
    static GameObject textMeshMPrefab;

    public static GameObject CreateLText(Vector3 position, string text, Transform parentTransform){
        textMeshLPrefab = textMeshLPrefab ?? (GameObject)Resources.Load("TextMeshL");
        return CreateText(position, text, textMeshLPrefab, parentTransform);
    }
    public static GameObject CreateLTextClickable(Vector3 position, string text, Transform parentTransform, ClickDelegate clickD){
        textMeshLPrefab = textMeshLPrefab ?? (GameObject)Resources.Load("TextMeshL");
        GameObject gob = CreateText(position, text, textMeshLPrefab, parentTransform);
        Text t = gob.AddComponent<Text>();
        t.clickDelegate = clickD;
        return gob;
    }

    public static GameObject CreateRText(Vector3 position, string text, Transform parentTransform){
        textMeshRPrefab = textMeshRPrefab ?? (GameObject)Resources.Load("TextMeshR");
        return CreateText(position, text, textMeshRPrefab, parentTransform);
    }

    public static GameObject CreateMText(Vector3 position, string text, Transform parentTransform){
        textMeshMPrefab = textMeshMPrefab ?? (GameObject)Resources.Load("TextMeshM");
        return CreateText(position, text, textMeshMPrefab, parentTransform);
    }

    static GameObject CreateText(Vector3 position, string text, GameObject prefab, Transform parentTransform){
        GameObject gob = (GameObject) GameObject.Instantiate(prefab, position, Quaternion.identity);
        gob.transform.parent = parentTransform;
        TextMesh mesh = gob.GetComponent<TextMesh>();
        mesh.text = text;
        BoxCollider bb = gob.GetComponent<BoxCollider>();
        bb.center = gob.renderer.bounds.center - gob.transform.position;
        bb.size = gob.renderer.bounds.size + new Vector3(0, 0, 1);
        return gob;
    }

    void OnMouseDown(){
        clickDelegate(GetComponent<TextMesh>().text);
    }

}
