using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System;
using MiniJSON;
using System.Linq;

public class SnapPlane : MonoBehaviour{
    public Dictionary<int, FunctionBehavior> childFunctions = new Dictionary<int, FunctionBehavior>();

    static GameObject functionBasePrefab;

    GameObject inputText;
    List<GameObject> functionChoices = new List<GameObject>();

    void Start(){
        functionBasePrefab = functionBasePrefab ?? (GameObject)Resources.Load("functionBaseP");
        //inputText = (inputText ?? Text.CreateLText(transform.position, "Type function name", transform));
        //Debug.Log(inputText); // guess what this prints
        Import();
    }
 
    // Update is called once per frame
    void Update(){
        if(inputText != null){
            if(Input.GetKeyDown(KeyCode.Return)){
                SpawnFunctionOnCameraTarget(inputText.GetComponent<TextMesh>().text);
                Destroy(inputText);
                foreach(GameObject text in functionChoices){
                    Destroy(text);
                }
                return;
            }
            TextMesh tm = inputText.GetComponent<TextMesh>();
            tm.text += Input.inputString;
            foreach(GameObject text in functionChoices){
                Destroy(text);
            }
            SpawnFunctionChoices(tm.text);
        }
    }
    void SpawnFunctionChoices(string filter){
        IEnumerable<Type> funcTypes = typeof(FunctionBehavior).Assembly.GetTypes().Where(
            type => type.IsSubclassOf(typeof(FunctionBehavior)) &&
                    Regex.IsMatch(type.Name.ToLower(), @".*"+filter.ToLower()+@".*"));
        int count = 1;
        foreach(Type funcType in funcTypes){
            functionChoices.Add(
                Text.CreateLTextClickable(CameraPositionOnPlane() + count*new Vector3(0f,-2f,0f), funcType.Name, Camera.main.transform, SpawnFunctionOnCameraTarget)
            );
            count++;
        }
    }

    Vector3 CameraPositionOnPlane(){
        return new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z-0.1f);
    }

    void SpawnFunctionOnCameraTarget(string functionName){
        SpawnFunction(functionName, 0, CameraPositionOnPlane(), null);
    }

    void SpawnFunction(string functionName, int id, Vector3 position, IList sockets){
        Type type = Type.GetType(functionName);
        if(type == null || !type.IsSubclassOf(typeof(FunctionBehavior))){
            Debug.Log("Tried to spawn illegal type"+ functionName);
            return;
        }
        GameObject functionBase = (GameObject)Instantiate(functionBasePrefab, position, functionBasePrefab.transform.rotation);
        FunctionBehavior func = (FunctionBehavior)functionBase.AddComponent(functionName);
        func.Init(id, this, sockets);
        functionBase.transform.parent = transform;
    }

    void OnMouseOver(){
        if(Input.GetKeyDown(KeyCode.T)){
            Vector3 position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, 30f));
            if(inputText == null){
                inputText = Text.CreateLText(position, "", Camera.main.transform);
            }
            SpawnFunctionChoices("");
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
            Vector3 position = ParseVector3((string)dict["position"],parentPlane.transform.position.z - 0.1f);
            parentPlane.SpawnFunction(typeString, Convert.ToInt32(dict["id"]), position, (IList)dict["sockets"]);
        }
    }

    static Vector3 ParseVector3(string stringrep, float customZ){
        stringrep = stringrep.Substring(1, stringrep.Length - 2);
        string[] parts = Regex.Split(stringrep, ", ");
        return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), customZ);
    }
}
