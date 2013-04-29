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
    protected SnapPlane parentPlane;
    int id = 0;

    protected virtual string GetFunctionName(){
        return "FunctionBehavior";
    }
    protected virtual string[] GetInputNames(){
        return new string[0];
    }
    protected virtual string[] GetOutputNames(){
        return new string[0];
    }

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
    public void Init(int id, SnapPlane parentPlane, IList sockets){
        this.id = id;
        this.parentPlane = parentPlane;
        Init();

        if(sockets != null){
            foreach(IDictionary sockDict in sockets){
                InitSocket(sockDict);
            }
        }
    }
    void Init(){
        this.name = GetFunctionName();
        string[] inputNames = GetInputNames();
        string[] outputNames = GetOutputNames();
        sockets = new Socket[inputNames.Length + outputNames.Length];
        int count = 0;
        foreach(string name in inputNames){
            GameObject gob = Text.CreateLText(new Vector3(renderer.bounds.min.x, renderer.bounds.max.y, renderer.bounds.max.z) + count * new Vector3(0, -3, 0), name, transform);
            InputSocket inp = gob.AddComponent<InputSocket>();
            inp.Init(name, this, count);
            sockets[count] = inp;
            count++;
        }
        int rightCount = 0;
        foreach(string name in outputNames){
            GameObject gob = Text.CreateRText(new Vector3(renderer.bounds.max.x, renderer.bounds.max.y, renderer.bounds.max.z) + rightCount * new Vector3(0, -3, 0), name, transform);
            Output outp = gob.AddComponent<Output>();
            outp.Init(name, this, count);
            sockets[count] = outp;
            count++;
            rightCount++;
        }

        Text.CreateMText(new Vector3(renderer.bounds.center.x, renderer.bounds.max.y, renderer.bounds.max.z), this.name, transform);
        parentPlane.childFunctions.Add(GetId(), this);
    }

    //protected abstract void Trigger(InputSocket inp);
    public virtual void Trigger<DataType>(InputSocket inp, DataType data){
    }

    public int GetId(){
        if(id == 0){
            id = GetInstanceID();
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

    void InitSocket(IDictionary dict){
        int index = Convert.ToInt32(dict["index"]);
        sockets[index].InitFromDict(dict);
    }

    public Socket GetSocketInParentPlane(int id, int index){
        if(parentPlane.childFunctions.ContainsKey(id)){
            return parentPlane.childFunctions[id].GetSocket(index);
        } else{
            return null;
        }
    }

    public void OnMouseDrag(){
        float dist =  parentPlane.transform.position.z - Camera.main.transform.position.z;
        Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
        Vector3 point = Camera.main.ScreenToWorldPoint(screenPoint);
        transform.position = parentPlane.SnapPositionFor(point, renderer.bounds);
        foreach(Socket sock in sockets){
            sock.UpdateLine();
        }
    }
}

