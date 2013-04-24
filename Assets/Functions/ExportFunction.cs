using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class ExportFunction : FunctionBehavior {

    protected override string GetFunctionName(){
        return "Export";
    }
    protected override string[] GetInputNames(){
        return new string[]{"in"};
    }
    protected override string[] GetOutputNames(){
        return new string[]{"out"};
    }

    protected override void Start ()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update ()
    {

    }

    public override void Trigger<DataType>(InputSocket inp, DataType data){
        parentPlane.Export();
        GetOutput(0).Trigger<DataType>(data);
    }
}
