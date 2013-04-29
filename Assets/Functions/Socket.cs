using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MiniJSON;

public class TargetId : IAsJson {
    public int parentId;
    public int index;

    public TargetId(int parentId, int index){
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
        return new TargetId(Convert.ToInt32(dict["parentId"]), Convert.ToInt32(dict["index"]));
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
        line = ((GameObject) Instantiate(connectorLinePrefab, new Vector3(0,0,0), Quaternion.identity)).GetComponent<LineRenderer>();
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

        if(other && other is TargetType && other.parentFunction != parentFunction){
            target = (TargetType) other;
            other.target = this;
        } else {
            target = null;
            if(target){
                target.target = null;
            }
        }
        if(target){
            target.UpdateLine();
        }
        UpdateLine();
    }

    void Disconnect(){
        if(target != null && target.target != null){
            target.target = null;
            target.UpdateLine();
        }
        target = null;
    }

    public void UpdateLine(){
        if(target){
            if(!target.line.enabled){
                line.enabled = true;
                line.SetPosition(0, transform.position + new Vector3(0,0,-0.1f));
                line.SetPosition(1, target.transform.position + new Vector3(0,0,-0.1f));
            } else {
                line.enabled = false;
                target.UpdateLine();
            }
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
        if(dict["targetId"] != null){
            targetId = TargetId.NewFromDict((IDictionary) dict["targetId"]);
        }
    }

    public void TryInitTarget(){
        Socket target = parentFunction.GetSocketInParentPlane(targetId.parentId, targetId.index);
        if(target != null){
            Connect(target);
            targetId = null;
        }
    }

    public void SetTarget(Socket other){
        target = other;
    }
}
