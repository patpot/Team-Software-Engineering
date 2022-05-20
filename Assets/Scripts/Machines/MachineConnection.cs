using Assets.Scripts;
using Assets.Scripts.Items;
using Assets.Scripts.Utils_Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineConnection : MonoBehaviour
{
    public BasicMachine ConnectedMachine;
    public Inventory ConnectedInventory;

    private bool _followCursor = true;
    private LineRenderer _lr;
    private Transform _playerTransform;
    public void Start()
        => _lr = GetComponent<LineRenderer>();

    public void SetPlayerTransform(Transform trans)
        => _playerTransform = trans;

    public void AssignMachine(BasicMachine machine)
        => ConnectedMachine = machine;
    public void FinaliseConnection(Inventory inv)
    {
        _followCursor = false;
        ConnectedInventory = inv;

        // Add a collider to the renderer, bake its mesh to be the line renderer's current points
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        var mesh = new Mesh();
        _lr.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;
        
        // We want a collider for click events, but not to block the player so set this collider to be a trigger
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
    }
    public void SendItemStack(ItemData data, float stackSize)
    {
        // Create our dropped object
        GameObject droppedObject = Instantiate(data.Model);
        droppedObject.name = data.Name;
        droppedObject.transform.position = transform.TransformPoint(_lr.GetPosition(0));

        // Add on our item data
        var itemStack = droppedObject.AddComponent<ItemStack>();
        itemStack.ItemData = data;
        itemStack.ItemCount = stackSize;

        // Add the script that will move this object towards its target inventory as well as the functionality for when it reaches this position
        var mtp = droppedObject.AddComponent<MoveTowardsPosition>();
        mtp.TargetPosition = _lr.GetPosition(1);
        mtp.OnCompleteAction += delegate
        {
            ConnectedInventory.TryDepositItem(data, stackSize);
            Destroy(droppedObject);
        };
    }

    public void RemoveConnection()
    {
        if (ConnectedMachine != null)
            ConnectedMachine.RemoveConnection(this);
        Destroy(gameObject);
    }

    public void Update()
    {
        // Make the line renderer follow the mouse
        if (SpellbookToggle.SpellbookActive && _followCursor)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5))
                _lr.SetPosition(1, hit.point);
        }
        
        // Ensure that we aren't too far from the last placed point
        if (_followCursor)
        {
            var pointPos = _lr.GetPosition(1);
            if (Vector2.Distance(pointPos, _playerTransform.position) > 5f)
                _lr.SetPosition(1, _lr.GetPosition(0));
        }

        if (!_followCursor && (!ConnectedMachine || !ConnectedInventory))
            RemoveConnection();
    }
}
