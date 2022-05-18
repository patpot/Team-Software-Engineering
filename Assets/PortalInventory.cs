using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInventory : Inventory
{
    public Material PortalShader;
    public Portal Portal;

    [ColorUsage(true, true)]
    public Color DefaultPortalColour;

    [ColorUsage(true, true)]
    public Color FinalPortalColour;

    private bool _ranOnce;
    // Start is called before the first frame update
    void Awake()
    {
        InventoryName = "Portal";

        // I changed the internal ID to just "Color" and the whole thing went pink so I just took the given internal ID
        PortalShader.SetColor("Color_01a739123df84429ab2d0d8cae72b343", DefaultPortalColour);
    }

    private void Update()
    {
        if (!_ranOnce && GetItemCount("Earth Crystal") >= 100)
        {
            _ranOnce = true;
            PortalShader.SetColor("Color_01a739123df84429ab2d0d8cae72b343", FinalPortalColour);
            Portal.Activate();
        }
    }
}
