using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private Slider healthSlider = default;

    //[SerializeField] private TextMeshProUGUI staminaText = default;
    [SerializeField] private Slider staminaSlider = default;

    private void OnEnable()
    {
        FirstPersonController.OnDamage += UpdateHealth;
        FirstPersonController.OnHeal += UpdateHealth;
        FirstPersonController.OnStaminaChange += UpdateStamina;
    }
    private void OnDisable()
    {
        FirstPersonController.OnDamage -= UpdateHealth;
        FirstPersonController.OnHeal -= UpdateHealth;
        FirstPersonController.OnStaminaChange -= UpdateStamina;
    }

    private void Start()
    {
        healthSlider.maxValue = 100;
        staminaSlider.maxValue = 100;
        UpdateHealth(100);
        UpdateStamina(100);
    }

    private void UpdateHealth(float currentHealth)
    {
        //healthText.text = currentHealth.ToString("00");
        healthSlider.value = currentHealth;
    }
    private void UpdateStamina(float currentStamina)
    {
        //staminaText.text = currentStamina.ToString("00");
        staminaSlider.value = currentStamina;
    }
}
