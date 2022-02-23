using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StateRunner : MonoBehaviour
{
    public IStateMgr StateMgr;

    public void Update()
    {
        // Sadly we can't actually use rotf-server time semantics
        StateMgr.OnTick(new Time());
    }
}
