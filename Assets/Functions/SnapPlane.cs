using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System;
using MiniJSON;
using System.Linq;

public class SnapPlane : MonoBehaviour{
    public Dictionary<string, FunctionBehavior> childFunctions = new Dictionary<string, FunctionBehavior>();

    static GameObject functionBasePrefab;

    // Use this for initialization
    void Start(){
        functionBasePrefab = functionBasePrefab ?? (GameObject)Resources.Load("functionBaseP");
        Import();
    }
 
    // Update is called once per frame
    void Update(){
    }

    void OnMouseOver(){
        if(Input.GetKeyDown("t")){

        }
    }

    public Vector3 SnapPositionFor(Vector3 point, Bounds rendererBounds){
        Bounds meshBounds = new Bounds(Vector3.zero, rendererBounds.size);
        float snapUnit = (renderer.bounds.max.x - renderer.bounds.min.x)/50f;
        float offsetX = (float) Math.Round((point.x - renderer.bounds.min.x)/snapUnit)*snapUnit;
        float offsetY = (float) Math.Round((point.y - renderer.bounds.min.y)/snapUnit)*snapUnit;
        Vector3 snappedPos = new Vector3(renderer.bounds.min.x+ offsetX,
            renderer.bounds.min.y+ offsetY, 0);
        return ClampVector3(snappedPos,
            new Vector3(renderer.bounds.min.x + meshBounds.max.x,renderer.bounds.min.y + meshBounds.max.y,renderer.bounds.min.z- 0.1f),
            new Vector3(renderer.bounds.max.x - meshBounds.max.x,renderer.bounds.max.y - meshBounds.max.y,renderer.bounds.max.z- 0.1f));
    }

    public static Vector3 ClampVector3(Vector3 vec, Vector3 min, Vector3 max)
    {
        return Vector3.Min(max,Vector3.Max(min,vec));
    }

    public void Export(){
        string json = MiniJSON.Json.Serialize(childFunctions.Values.ToArray());
        StreamWriter write = new StreamWriter("functions.json");
        write.Write(json);
        write.Close();
        //Import(json);
    }

    public void Import(){
        StreamReader read = new StreamReader("functions.json");
        string json = read.ReadToEnd();
        read.Close();
        Import(json, this);
    }

    static void Import(string json, SnapPlane parentPlane){
        object functions = MiniJSON.Json.Deserialize(json);
        foreach(IDictionary dict in (IList) functions){
            string typeString = (string)dict["type"];
            //Type type = Type.GetType(typeString);

            Vector3 position = ParseVector3((string)dict["position"],parentPlane.transform.position.z - 0.1f);
            GameObject functionBase = (GameObject)Instantiate(functionBasePrefab, position, functionBasePrefab.transform.rotation);
            FunctionBehavior func = (FunctionBehavior)functionBase.AddComponent(typeString);
            func.Init((string) dict["id"], parentPlane);
            IList socketDicts = (IList)dict["sockets"];
            foreach(IDictionary sockDict in socketDicts){
                int index = Convert.ToInt32(sockDict["index"]);
                func.InitSocket(index, sockDict);
            }
        }
    }

    static Vector3 ParseVector3(string stringrep, float customZ){
        stringrep = stringrep.Substring(1, stringrep.Length - 2);
        string[] parts = Regex.Split(stringrep, ", ");
        return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), customZ);
    }
}
