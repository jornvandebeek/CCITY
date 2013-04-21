using UnityEngine;
using System.Collections;

public class Output : Socket{
    protected override void Connect(Socket other){
        base.Connect<InputSocket>(other);
    }

    public void Trigger<DataType>(DataType data){
        InputSocket target = (InputSocket) this.target;
        if(target){
            target.parentFunction.Trigger<DataType>(target, data);
        }
    }

}