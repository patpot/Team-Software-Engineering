using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VFX;

public class Spellbook : MonoBehaviour
{
    public Material GlowingFade;
    public GameObject TreeBreakVFX;
    public GameObject MachineConnectionPrefab;
    public List<Texture2D> PointCachePositions;
    public List<string> PointCachePositionNames;
    public MachineConnectionFader Fader;

    private Dictionary<string, Texture2D> _pointCacheNameToPosition = new Dictionary<string, Texture2D>();
    private BasicMachine _selectedMachine;
    private MachineConnection _connection;
    private void Start()
    {
        // Unity can't serialize dictionaries so we have to do this mess
        for (int i = 0; i < PointCachePositionNames.Count; i++)
            _pointCacheNameToPosition[PointCachePositionNames[i]] = PointCachePositions[i];
    }
    void Update()
    {
        if (UIManager.ActiveUICount > 0) return; // If we're rendering any UI we don't want to accidentally break a tree down

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5))
            {
                // Spellbook has 3 functions, check if our raycast activates any of them

                // If we haven't got a machine selected then we check if we hit a tree
                if (_selectedMachine == null)
                    if (_checkForTreeCollision(hit)) return;

                // Check if we hit another machine/inventory
                if (_checkForMachineCollision(hit)) return;
                if (_checkForInventoryCollision(hit)) return;
                if (_checkForItemCollision(hit)) return;
                if (_checkForConnectionCollision(hit)) return;
            }
            else if (_selectedMachine != null)
            {
                // We clicked and didn't hit anything, let's just cancel our last machine selection and remove current pending machine connection
                _selectedMachine = null;
                Destroy(_connection.gameObject);
                _connection = null;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            // We right clicked and didn't hit anything, let's just cancel our last machine selection and remove current pending machine connection
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5))
                if (_checkForConnectionCollision(hit)) return;

            _selectedMachine = null;
            if (_connection != null)
                Destroy(_connection.gameObject);
            _connection = null;
        }
    }

    private bool _checkForTreeCollision(RaycastHit hit)
    {
        Texture2D chosenPosition;
        foreach (var pcp in _pointCacheNameToPosition)
        {
            if (hit.transform.GetComponent<ItemStack>() != null) return false; // This is an item not a tree

            if (hit.transform.name.Contains(pcp.Key))
            {
                chosenPosition = pcp.Value;

                // Clone this tree object, set correct position and scale
                var go = Instantiate(hit.transform.gameObject);
                go.transform.position = hit.transform.position;
                go.transform.localScale = hit.transform.localScale;
                // Change the clones material to be our glowing fade out material
                go.GetComponent<MeshRenderer>().material = GlowingFade;
                go.name = "Fake Glowing Tree";
                // Add on script that controls the material fading
                MagicFade mf = go.AddComponent<MagicFade>();
                mf.ItemName = "Wood Log";
                mf.ItemCount = 1f;
                // Destroy original
                Destroy(hit.transform.gameObject);

                // Add in our visual effect that drops particles to the floor
                var treeVFX = Instantiate(TreeBreakVFX);
                treeVFX.transform.position = go.transform.position;
                treeVFX.transform.rotation = go.transform.rotation;
                treeVFX.transform.localScale = go.transform.localScale;
                treeVFX.name = "Fake Fading Tree";

                treeVFX.GetComponent<VisualEffect>().SetTexture("PointCachePosition", chosenPosition);
                return true;
            }
        }
        return false;
    }
    private bool _checkForMachineCollision(RaycastHit hit)
    {
        BasicMachine machine = hit.transform.gameObject.GetComponentInParent<BasicMachine>();
        if (machine != null)
        {
            if (_selectedMachine != null)
            {
                // We hit a machine and had already selected a machine last click, try connect them
                // TODO: Maybe make an in-world error popup?
                foreach (var connection in _selectedMachine.MachineConnections)
                    if (connection.ConnectedInventory == machine.InternalInputInventory) return true; // Don't assign the same inventory twice
                if (machine == _selectedMachine) return true; // Don't assign ourselves

                Debug.Log("Machine output registered");

                // Finalise our machine connection
                _connection.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                _connection.AssignMachine(_selectedMachine);
                _connection.FinaliseConnection(machine.InternalInputInventory);
                // Assign the connection to our machine
                _selectedMachine.MachineConnections.Add(_connection);

                _selectedMachine = null;
                _connection = null;

                Fader.StartFade();
            }
            else
            {
                // This is the first machine we've selected, create our machine connection object with this as a start point
                _connection = Instantiate(MachineConnectionPrefab).GetComponent<MachineConnection>();
                _connection.GetComponent<LineRenderer>().SetPosition(0, hit.point);
                _connection.SetPlayerTransform(transform.parent.transform);
                Fader.ResetAlpha();

                Debug.Log("Machine registered");
                _selectedMachine = machine;
            }
            return true;
        }

        return false;
    }

    private bool _checkForInventoryCollision(RaycastHit hit)
    {
        Inventory inventory = hit.transform.gameObject.GetComponentInParent<Inventory>();
        if (inventory != null)
        {
            if (_selectedMachine != null)
            {
                // We hit an inventory, if we had already selected a machine last click then we connect them
                // TODO: Maybe make an in-world error popup?
                foreach (var connection in _selectedMachine.MachineConnections)
                    if (connection.ConnectedInventory == inventory) return true; // Don't assign the same inventory twice
                if (inventory == _selectedMachine.InternalInputInventory) return true; // Don't assign to our own inventory

                // Finalise our machine connection
                _connection.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                _connection.AssignMachine(_selectedMachine);
                _connection.FinaliseConnection(inventory);
                // Assign the connection to our machine
                _selectedMachine.MachineConnections.Add(_connection);

                _selectedMachine = null;
                _connection = null;
                Fader.StartFade();
            }
            return true;
        }

        return false;
    }

    private bool _checkForItemCollision(RaycastHit hit)
    {
        // Any item use cases are very specific, so we hardcode :c
        ItemStack itemStack = hit.transform.gameObject.GetComponent<ItemStack>();
        if (itemStack != null)
        {
            if (itemStack.Name == "Wood Log" && hit.transform.name != "Fading Log")
            {
                itemStack.Collectible = false;
                hit.transform.name = "Fading Log";
                hit.transform.gameObject.GetComponent<MeshRenderer>().material = GlowingFade;
                //hit.transform.gameObject.AddComponent<DieAfterTime>()
                // Add on script that controls the material fading
                MagicFade mf = hit.transform.gameObject.AddComponent<MagicFade>();
                mf.ItemName = "Earth Mana";
                mf.ItemCount = itemStack.ItemCount;
                // Add in our visual effect that drops particles to the floor
                var treeVFX = Instantiate(TreeBreakVFX);
                treeVFX.transform.position = hit.transform.position;
                treeVFX.transform.rotation = hit.transform.rotation;
                treeVFX.transform.localScale = hit.transform.localScale;
                treeVFX.name = "Fake Fading Log";

                treeVFX.GetComponent<VisualEffect>().SetTexture("PointCachePosition", _pointCacheNameToPosition["Wood Log"]);
            }
            else if (itemStack.Collectible)
            {
                // We have no special interactions with this item stack, pick it back up
                PlayerInventory.Instance.TryDepositItem(itemStack.ItemData, itemStack.ItemCount);
                Destroy(hit.transform.gameObject);
            }
        }
        return false;
    }

    private bool _checkForConnectionCollision(RaycastHit hit)
    {
        MachineConnection connection = hit.transform.gameObject.GetComponent<MachineConnection>();
        if (connection != null)
        {
            connection.RemoveConnection();
            return true;
        }
        return false;
    }
}
