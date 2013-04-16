using UnityEngine;
using System.Collections;

public class TriggerFunction : FunctionBehavior {

    public string data;

    // Use this for initialization
    protected override void Start () {
        outputNames = new string[]{"out"};
        name = "Trigger";
        base.Start();
    }

    // Update is called once per frame
    protected override void Update () {

    }

    void OnMouseUp(){
        outputs[0].Trigger<string>(data);
    }
}
