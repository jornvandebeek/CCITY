using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MiniJSON;
using System.Linq;

class WrongDataTypeException : System.ArgumentException{
};

public abstract class FunctionBehavior : MonoBehaviour, IAsJson{
    Socket[] sockets;
    SnapPlane parentPlane;
    string id;

    protected virtual string GetFunctionName(){
        return "FunctionBehavior";
    }
    protected virtual string[] GetInputNames(){
        return new string[0];
    }
    protected virtual string[] GetOutputNames(){
        return new string[0];
    }

    static GameObject textMeshLPrefab;
    static GameObject textMeshRPrefab;
    static GameObject textMeshMPrefab;
 
    void OnDestroy(){
        parentPlane.childFunctions.Remove(GetId());
    }

    // Use this for initialization
    protected virtual void Start(){
        if(!parentPlane.childFunctions.ContainsKey(GetId())){
            Init();
        }
    }

    // Update is called once per frame
    protected virtual void Update(){

    }
    public void Init(string id, SnapPlane parentPlane){
        this.id = id;
        this.parentPlane = parentPlane;
        Init();
    }
    void Init(){
        this.name = GetFunctionName();
        textMeshLPrefab = textMeshLPrefab ?? (GameObject)Resources.Load("TextMeshL");
        textMeshRPrefab = textMeshRPrefab ?? (GameObject)Resources.Load("TextMeshR");
        textMeshMPrefab = textMeshMPrefab ?? (GameObject)Resources.Load("TextMeshM");
        string[] inputNames = GetInputNames();
        string[] outputNames = GetOutputNames();
        sockets = new Socket[inputNames.Length + outputNames.Length];
        int count = 0;
        foreach(string name in inputNames){
            GameObject gob = CreateLText(new Vector3(renderer.bounds.min.x, renderer.bounds.max.y, renderer.bounds.max.z) + count * new Vector3(0, -3, 0), name);
            InputSocket inp = gob.AddComponent<InputSocket>();
            inp.Init(name, this, count);
            sockets[count] = inp;
            count++;
        }
        int rightCount = 0;
        foreach(string name in outputNames){
            GameObject gob = CreateRText(new Vector3(renderer.bounds.max.x, renderer.bounds.max.y, renderer.bounds.max.z) + rightCount * new Vector3(0, -3, 0), name);
            Output outp = gob.AddComponent<Output>();
            outp.Init(name, this, count);
            sockets[count] = outp;
            count++;
            rightCount++;
        }

        CreateMText(new Vector3(renderer.bounds.center.x, renderer.bounds.max.y, renderer.bounds.max.z), this.name);
        parentPlane.childFunctions.Add(GetId(), this);
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
        GameObject gob = (GameObject)Instantiate(prefab, position, Quaternion.identity);
        gob.transform.parent = transform;
        TextMesh mesh = gob.GetComponent<TextMesh>();
        mesh.text = text;
        BoxCollider bb = gob.GetComponent<BoxCollider>();
        bb.center = gob.renderer.bounds.center - gob.transform.position;
        bb.size = gob.renderer.bounds.size + new Vector3(0, 0, 1);
        return gob;
    }

    //protected abstract void Trigger(InputSocket inp);
    public virtual void Trigger<DataType>(InputSocket inp, DataType data){
    }

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

    public InputSocket GetInput(int index){
        return (InputSocket)sockets.Where(s => s is InputSocket).ToArray()[index];
    }
    public Output GetOutput(int index){
        return (Output)sockets.Where(s => s is Output).ToArray()[index];
    }
    public Socket GetSocket(int index){
        return sockets[index];
    }

    public void InitSocket(int index, IDictionary dict){
        sockets[index].InitFromDict(dict);
    }

    public Socket GetSocketInParentPlane(string id, int index){
        if(parentPlane.childFunctions.ContainsKey(id)){
            return parentPlane.childFunctions[id].GetSocket(index);
        } else{
            return null;
        }
    }
}
