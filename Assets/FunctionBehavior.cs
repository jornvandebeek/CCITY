using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class WrongDataTypeException : System.ArgumentException{};
interface IAsJson{
    IDictionary AsJson();
}

public abstract class FunctionBehavior : MonoBehaviour, IAsJson {
    public InputSocket[] inputs;
    public Output[] outputs;
    public Guid id;
    protected string[] inputNames = new string[0];
    protected string[] outputNames  = new string[0];

    static GameObject textMeshLPrefab;
    static GameObject textMeshRPrefab;
    static GameObject textMeshMPrefab;
    static List<FunctionBehavior> allFunctions = new List<FunctionBehavior>();

    void Awake(){
        allFunctions.Add(this);
    }

    void OnDestroy(){
        allFunctions.Remove(this);
    }

    // Use this for initialization
    protected virtual void Start ()
    {
        id = Guid.NewGuid();
        textMeshLPrefab = textMeshLPrefab ?? (GameObject) Resources.Load("TextMeshL");
        textMeshRPrefab = textMeshRPrefab ?? (GameObject) Resources.Load("TextMeshR");
        textMeshMPrefab = textMeshMPrefab ?? (GameObject) Resources.Load("TextMeshM");
        inputs = new InputSocket[inputNames.Length];
        int count = 0;
        foreach(string name in inputNames){
            GameObject gob = CreateLText(new Vector3(renderer.bounds.min.x, renderer.bounds.max.y, renderer.bounds.max.z) + count* new Vector3(0,-3,0), name);
            InputSocket inp = gob.AddComponent<InputSocket>();
            inp.Init(name, this);
            inputs[count] = inp;
            count += 1;
        }

        outputs = new Output[outputNames.Length];
        count = 0;
        foreach(string name in outputNames){
            GameObject gob = CreateRText(new Vector3(renderer.bounds.max.x, renderer.bounds.max.y, renderer.bounds.max.z) + count* new Vector3(0,-3,0), name);
            Output outp = gob.AddComponent<Output>();
            outp.Init(name, this);
            outputs[count] = outp;
            count += 1;
        }

        CreateMText(new Vector3(renderer.bounds.center.x, renderer.bounds.max.y, renderer.bounds.max.z), this.name);
    }

    GameObject CreateLText(Vector3 position, string text){
        return CreateText(position, text, textMeshLPrefab);
    }

    GameObject CreateRText(Vector3 position, string text){
        return CreateText(position, text, textMeshRPrefab);
    }

    GameObject CreateMText(Vector3 position, string text){
        return CreateText(position, text, textMeshMPrefab);
    }

    GameObject CreateText(Vector3 position, string text, GameObject prefab){
        GameObject gob = (GameObject) Instantiate(prefab, position, Quaternion.identity);
        gob.transform.parent = transform;
        TextMesh mesh = gob.GetComponent<TextMesh>();
        mesh.text = text;
        BoxCollider bb = gob.GetComponent<BoxCollider>();
        bb.center = gob.renderer.bounds.center - gob.transform.position;
        bb.size = gob.renderer.bounds.size + new Vector3(0,0,1);
        return gob;
    }

    //protected abstract void Trigger(InputSocket inp);
    public virtual void Trigger<DataType>(InputSocket inp, DataType data){}
 
    // Update is called once per frame
    protected virtual void Update ()
    {
 
    }

    public IDictionary AsJson(){
        Dictionary<string, object> hash = new Dictionary<string, object>();
        hash.Add("name", name);
        hash.Add("id", id);
        hash.Add("inputs", inputs);
        hash.Add("outputs", outputs);
        hash.Add("type", this.GetType().Name);
        return hash;
    }

    public static void Export(){
        Debug.Log(MiniJSON.Json.Serialize(allFunctions));
    }
}
