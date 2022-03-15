using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementFollowCursor : MonoBehaviour
{
    public void Update()
        => transform.position = Input.mousePosition;
}