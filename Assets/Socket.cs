using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Socket : MonoBehaviour, IAsJson {
    static Socket startedDragOn;
    static Socket lastEntered;
    static GameObject connectorLinePrefab;

    public FunctionBehavior parentFunction;
    public Socket target;
    int index;

    LineRenderer line;


    void Start(){
        connectorLinePrefab = connectorLinePrefab ?? (GameObject) Resources.Load("ConnectorLine");
    }

    public void Init(string name, FunctionBehavior parentFunction){
        this.name = name;
        this.parentFunction = parentFunction;
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

    public object[] GetId(){
        return new object[] {parentFunction.GetId(), index};
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
        hash.Add("id", GetId());
        if(target){
            hash.Add("targetId", target.GetId());
        } else {
            hash.Add("targetId", null);
        }
        hash.Add("type", this.GetType().Name);
        return hash;
    }

    public void SetTarget(Socket other){
        target = other;
    }
}
