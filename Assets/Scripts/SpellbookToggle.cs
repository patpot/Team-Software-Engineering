using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpellbookToggle : MonoBehaviour
{
    public UnityEvent onToggle;
    public static bool SpellbookActive;
    
    private MeshRenderer _mRenderer;
    private Spellbook _spellbook;
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
            if (UIManager.ActiveUICount > 0) return;

            _mRenderer.enabled = !_mRenderer.enabled;
            _spellbook.enabled = !_spellbook.enabled;
            SpellbookActive = _spellbook.enabled;
            if (SpellbookActive)
                _spellbook.Fader.StartFade();
            onToggle?.Invoke();
        }
    }
}
