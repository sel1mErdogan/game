using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfirmationPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action onConfirm;

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
        gameObject.SetActive(false); // Başlangıçta kapalı olsun
    }

    public void Show(string question, Action onConfirmAction)
    {
        gameObject.SetActive(true);
        questionText.text = question;
        onConfirm = onConfirmAction;
    }

    private void OnConfirm()
    {
        onConfirm?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnCancel()
    {
        onConfirm = null; // Görevi iptal et
        gameObject.SetActive(false);
    }
}