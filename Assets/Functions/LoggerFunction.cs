using UnityEngine;
using System.Collections;

public class LoggerFunction : FunctionBehavior {

    protected override string GetFunctionName(){
        return "Logger";
    }
    protected override string[] GetInputNames(){
        return new string[]{"string"};
    }
    protected override string[] GetOutputNames(){
        return new string[0];
    }

    // Use this for initialization
	protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
	
	}

    public override void Trigger<DataType>(InputSocket inp, DataType data){
        Debug.Log(data.ToString());
    }
}
