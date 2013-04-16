using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

public class SHA512Function : FunctionBehavior
{
    SHA512 shaM;
    protected override void Start ()
    {
        inputNames = new string[]{"data"};
        outputNames = new string[]{"hash"};
        name = "SHA512";
        shaM = new SHA512Managed ();
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

        outputs[0].Trigger<string>(result);
    }

}
