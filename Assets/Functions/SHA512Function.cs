using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

public class SHA512Function : FunctionBehavior
{
    SHA512 shaM;

    protected override string GetFunctionName(){
        return "SHA512";
    }
    protected override string[] GetInputNames(){
        return new string[]{"data"};
    }
    protected override string[] GetOutputNames(){
        return new string[]{"hash"};
    }

    protected override void Start ()
    {
        shaM = new SHA512Managed();
        base.Start();
    }

    // Update is called once per frame
    protected override void Update ()
    {

    }

    public override void Trigger<DataType>(InputSocket inp, DataType data){
        if(!(data is string)){
            throw new WrongDataTypeException();
        }
        string result = Convert.ToBase64String(shaM.ComputeHash(Encoding.UTF8.GetBytes((string) (object)data)));

        GetOutput(0).Trigger<string>(result);
    }
}
