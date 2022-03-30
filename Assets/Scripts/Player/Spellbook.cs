using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VFX;

public class Spellbook : MonoBehaviour
{
    public Material GlowingFade;
    public GameObject TreeBreakVFX;
    public List<Texture2D> PointCachePositions;
    public List<string> PointCachePositionNames;

    private Dictionary<string, Texture2D> _pointCacheNameToPosition = new Dictionary<string, Texture2D>();
    private BasicMachine _selectedMachine;
    private void Start()
    {
        // Unity can't serialize dictionaries so we have to do this mess
        for (int i = 0; i < PointCachePositionNames.Count; i++)
            _pointCacheNameToPosition[PointCachePositionNames[i]] = PointCachePositions[i];
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (UIManager.UIActive) return; // If we're rendering any UI we don't want to accidentally break a tree down

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
            }
            else if (_selectedMachine != null)
            {
                // We clicked and didn't hit anything, let's just cancel our last machine selection
                _selectedMachine = null;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            _selectedMachine = null;
        }
    }

    private bool _checkForTreeCollision(RaycastHit hit)
    {
        Texture2D chosenPosition;
        foreach (var pcp in _pointCacheNameToPosition)
        {
            if (hit.transform.name.Contains(pcp.Key))
            {
                chosenPosition = pcp.Value;

                // Clone this tree object, set correct position and scale
                var go = Instantiate(hit.transform.gameObject);
                go.transform.position = hit.transform.position;
                go.transform.localScale = hit.transform.localScale;
                // Change the clones material to be our glowing fade out material
                go.GetComponent<MeshRenderer>().material = GlowingFade;
                // Add on script that controls the material fading
                MagicFade mf = go.AddComponent<MagicFade>();
                mf.ItemName = "Wood Log";
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
            // We hit a machine, if we had already selected a machine last click then we connect them
            if (_selectedMachine != null)
            {
                Debug.Log("Machine output registered");
                _selectedMachine.OutputInventories.Add(machine.InputInventory);
                _selectedMachine = null;
            }
            else
            {
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
            // We hit an inventory, if we had already selected a machine last click then we connect them
            if (_selectedMachine != null)
            {
                Debug.Log("Output registered");
                _selectedMachine.OutputInventories.Add(inventory);
                _selectedMachine = null;
            }
            return true;
        }

        return false;
    }

    private bool _checkForItemCollision(RaycastHit hit)
    {
        // Any item use cases are very specific, so we hardcode :c
        if (hit.transform.parent.name == "Wood Log")
        {
            hit.transform.parent.name = "Fading Log";
            hit.transform.gameObject.GetComponent<MeshRenderer>().material = GlowingFade;
            // Add on script that controls the material fading
            MagicFade mf = hit.transform.gameObject.AddComponent<MagicFade>();
            mf.ItemName = "Earth Mana";
            // Add in our visual effect that drops particles to the floor
            var treeVFX = Instantiate(TreeBreakVFX);
            treeVFX.transform.position = hit.transform.position;
            treeVFX.transform.rotation = hit.transform.rotation;
            treeVFX.transform.localScale = hit.transform.localScale;
            treeVFX.name = "Fake Fading Log";

            treeVFX.GetComponent<VisualEffect>().SetTexture("PointCachePosition", _pointCacheNameToPosition["Wood Log"]);
        }
        return false;
    }
}
