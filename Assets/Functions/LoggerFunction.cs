using UnityEngine;
using System.Collections;

public class LoggerFunction : FunctionBehavior {

	// Use this for initialization
	protected override void Start () {
        inputNames = new string[]{"string"};
        name = "Logger";
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
	
	}

    public override void Trigger<DataType>(InputSocket inp, DataType data){
        Debug.Log(data.ToString());
    }
}
