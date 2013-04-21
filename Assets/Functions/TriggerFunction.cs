using UnityEngine;
using System.Collections;

public class TriggerFunction : FunctionBehavior {

    public string data;

    // Use this for initialization
    protected override void Start () {
        name = "Trigger";
        base.Start();
    }

    // Update is called once per frame
    protected override void Update () {

    }

    override string[] getInputNames(){
        return new string[0];
    }

    override string[] getOutputNames(){
        return new string[]{"out"};
    }

    void OnMouseUp(){
        GetOutput(0).Trigger<string>(data);
    }
}
