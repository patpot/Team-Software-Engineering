using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellbookToggle : MonoBehaviour
{
    private MeshRenderer _mRenderer;
    private Spellbook _spellbook;

    public static bool SpellbookActive;
    // Start is called before the first frame update
    void Start()
    {
        _mRenderer = GetComponent<MeshRenderer>();
        _spellbook = GetComponent<Spellbook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !CameraSwitcher.BuildMode)
        {
            if (UIManager.UIActive) return;

            _mRenderer.enabled = !_mRenderer.enabled;
            _spellbook.enabled = !_spellbook.enabled;
            SpellbookActive = _spellbook.enabled;
            if (SpellbookActive)
                _spellbook.Fader.StartFade();
        }
    }
}
