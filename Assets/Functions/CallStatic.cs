using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class CallStatic : FunctionBehavior {

    public string className;
    public string methodName;

    protected override void Start ()
    {
        inputNames = new string[]{"in"};
        outputNames = new string[]{"out"};
        name = "CallStatic";
        base.Start();
    }

    // Update is called once per frame
    protected override void Update ()
    {

    }

    public override void Trigger<DataType>(InputSocket inp, DataType data){
        Type t = Type.GetType(className);
        MethodInfo method = t.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);

        method.Invoke(null, null);
    }
}
