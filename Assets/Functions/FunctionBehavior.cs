using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.IO;
using MiniJSON;

class WrongDataTypeException : System.ArgumentException{};

public abstract class FunctionBehavior : MonoBehaviour, IAsJson {
    Socket[] sockets;

    string id;

    virtual string[] getInputNames(){
        return new string[0];
    }
    virtual string[] getOutputNames(){
        return new string[0];
    }

    static GameObject textMeshLPrefab;
    static GameObject textMeshRPrefab;
    static GameObject textMeshMPrefab;
    static GameObject functionBasePrefab;
    public static Dictionary<string, FunctionBehavior> allFunctions = new Dictionary<string, FunctionBehavior>();

    void OnDestroy(){
        allFunctions.Remove(GetId());
    }

    // Use this for initialization
    protected virtual void Start ()
    {
        if(!allFunctions.ContainsKey(GetId())){
            Init();
        }
    }

    // Update is called once per frame
    protected virtual void Update ()
    {

    }

    void Init(){
        textMeshLPrefab = textMeshLPrefab ?? (GameObject) Resources.Load("TextMeshL");
        textMeshRPrefab = textMeshRPrefab ?? (GameObject) Resources.Load("TextMeshR");
        textMeshMPrefab = textMeshMPrefab ?? (GameObject) Resources.Load("TextMeshM");
        functionBasePrefab = functionBasePrefab ?? (GameObject) Resources.Load("functionBaseP");
        sockets = new Socket[GetInputNames().Length + GetOutputNames().Length];
        int count = 0;
        foreach(string name in GetInputNames()){
            GameObject gob = CreateLText(new Vector3(renderer.bounds.min.x, renderer.bounds.max.y, renderer.bounds.max.z) + count* new Vector3(0,-3,0), name);
            InputSocket inp = gob.AddComponent<InputSocket>();
            inp.Init(name, this, count);
            sockets[count] = inp;
            count++;
        }
        int rightCount = 0;
        foreach(string name in GetOutputNames()){
            GameObject gob = CreateRText(new Vector3(renderer.bounds.max.x, renderer.bounds.max.y, renderer.bounds.max.z) + rightCount* new Vector3(0,-3,0), name);
            Output outp = gob.AddComponent<Output>();
            outp.Init(name, this, count);
            sockets[count] = outp;
            count++;
            rightCount++;
        }

        CreateMText(new Vector3(renderer.bounds.center.x, renderer.bounds.max.y, renderer.bounds.max.z), this.name);
        allFunctions.Add(GetId(), this);
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

    public string GetId(){
        if(id == null){
            id = Guid.NewGuid().ToString();
        }
        return id;
    }

    public IDictionary AsJson(){
        Dictionary<string, object> hash = new Dictionary<string, object>();
        hash.Add("name", name);
        hash.Add("id", GetId());
        hash.Add("sockets", sockets);
        hash.Add("position", transform.position);
        hash.Add("type", this.GetType().Name);
        return hash;
    }


    public static void Export(){
        string json = MiniJSON.Json.Serialize(allFunctions.Values.ToArray());
        StreamWriter write = new StreamWriter("functions.json");
        write.Write(json);
        write.Close();
        //Import(json);
    }
    public static void Import(){
        StreamReader read = new StreamReader("functions.json");
        string json = read.ReadToEnd();
        read.Close();
        Import(json);
    }
    static void Import(string json){
        object functions = MiniJSON.Json.Deserialize(json);
        foreach(IDictionary dict in (IList) functions){
            string typeString = (string) dict["type"];
            //Type type = Type.GetType(typeString);

            Vector3 position = ParseVector3((string) dict["position"]);
            GameObject functionBase = (GameObject) Instantiate(functionBasePrefab, position, Quaternion.identity);
            FunctionBehavior func = (FunctionBehavior) functionBase.AddComponent(typeString);
            func.id = (string) dict["id"];
            func.Init();
            Debug.Log(typeString + " " + func.outputNames.Length);
            IList socketDicts = (IList) dict["sockets"];
            foreach(IDictionary sockDict in socketDicts){
                long index = (long) sockDict["index"];
                Debug.Log("i"+index);
                func.sockets[index].InitFromDict(dict);
            }
        }
    }

    static Vector3 ParseVector3(string stringrep){
        stringrep = stringrep.Substring(1,stringrep.Length-2);
        string[] parts = Regex.Split(stringrep, ", ");
        return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
    }

    public InputSocket GetInput(int index){
        return (InputSocket) sockets.Where(s => s is InputSocket).ToArray()[index];
    }
    public Output GetOutput(int index){
        return (Output) sockets.Where(s => s is Output).ToArray()[index];
    }

    public Socket GetSocket(int index){
        return sockets[index];
    }
}
