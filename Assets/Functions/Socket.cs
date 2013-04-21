using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MiniJSON;

public class TargetId : IAsJson {
    public string parentId;
    public int index;

    public TargetId(string parentId, int index){
        this.parentId = parentId;
        this.index = index;
    }
    public IDictionary AsJson(){
        Dictionary<string, object> hash = new Dictionary<string, object>();
        hash.Add("parentId", parentId);
        hash.Add("index", index);
        return hash;
    }
    public static TargetId NewFromDict(IDictionary dict){
        return new TargetId((string) dict["parentId"], int.Parse((string) dict["index"]));
    }
}

public abstract class Socket : MonoBehaviour, IAsJson {
    static Socket startedDragOn;
    static Socket lastEntered;
    static GameObject connectorLinePrefab;

    public FunctionBehavior parentFunction;
    public Socket target;

    TargetId targetId;
    int index;

    LineRenderer line;


    void Start(){
        connectorLinePrefab = connectorLinePrefab ?? (GameObject) Resources.Load("ConnectorLine");
    }
    void Update(){
        if(targetId != null){
            TryInitTarget();
        }
    }

    public void Init(string name, FunctionBehavior parentFunction, int index){
        this.name = name;
        this.parentFunction = parentFunction;
        this.index = index;
    }

    public TargetId GetAsTargetId(){
        return new TargetId(parentFunction.GetId(), index);
    }

    void OnMouseDown() {
        startedDragOn = this;
    }
    void OnMouseUp(){
        startedDragOn.Connect(lastEntered);
        startedDragOn = null;
    }
    void OnMouseExit(){
        lastEntered = null;
    }

    void OnMouseEnter(){
        lastEntered = this;
    }

    protected abstract void Connect(Socket other);

    protected virtual void Connect<TargetType>(Socket other) where TargetType : Socket{
        if(other && other is TargetType){
            target = (TargetType) other;
            addLineTo(target);
            other.target = this;
        } else {
            if(target){
                target.target = null;
                target.addLineTo(null);
            }
            target = null;
            addLineTo(null);
        }
    }

    protected void addLineTo(Socket other){
        line = line ?? ((GameObject) Instantiate(connectorLinePrefab, new Vector3(0,0,0), Quaternion.identity)).GetComponent<LineRenderer>();
        if(other && other.parentFunction != parentFunction && other.target != this){
            line.enabled = true;
            line.SetPosition(0, transform.position + new Vector3(0,0,-0.1f));
            line.SetPosition(1, other.transform.position + new Vector3(0,0,-0.1f));
        } else {
            line.enabled = false;
        }
    }

    public IDictionary AsJson(){
        Dictionary<string, object> hash = new Dictionary<string, object>();
        hash.Add("name", name);
        hash.Add("index", index);
        if(target){
            hash.Add("targetId", target.GetAsTargetId());
        } else {
            hash.Add("targetId", null);
        }
        hash.Add("type", this.GetType().Name);
        return hash;
    }

    public void InitFromDict(IDictionary dict){
        name = (string) dict["name"];
        targetId = TargetId.NewFromDict((IDictionary) dict["targetId"]);
    }

    public void TryInitTarget(){
        if(FunctionBehavior.allFunctions.ContainsKey(targetId.parentId)){
            FunctionBehavior targetFunc = FunctionBehavior.allFunctions[targetId.parentId];
            target = targetFunc.GetSocket(targetId.index);
            targetId = null;
        }
    }

    public void SetTarget(Socket other){
        target = other;
    }
}
