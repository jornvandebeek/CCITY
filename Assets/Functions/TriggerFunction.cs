using UnityEngine;
using System.Collections;

public class TriggerFunction : FunctionBehavior {

    public string data = "";

    protected override string GetFunctionName(){
        return "Trigger";
    }
    protected override string[] GetInputNames(){
        return new string[0];
    }
    protected override string[] GetOutputNames(){
        return new string[]{"out"};
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update () {

    }

    void OnMouseUp(){
        GetOutput(0).Trigger<string>(data);
    }
}
