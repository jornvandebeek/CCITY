using UnityEngine;
using System.Collections;

public class InputSocket : Socket{
    protected override void Connect(Socket other){
        base.Connect<Output>(other);
    }
}