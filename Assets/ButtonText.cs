using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonText : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] LevelBuilder Builder;
    [SerializeField] private bool build;
    [SerializeField] TextMeshProUGUI buttonText;

    [Header("Build Button")]
    [SerializeField] string buttonText1;
    [SerializeField] string buttonText2;

    [Header("Speed Button")]
    [SerializeField] string buttonText3;
    [SerializeField] string buttonText4;

    private void Update()
    {
        if (Builder == null) return;

        if (build)
        {
            if (Builder.builderRunning) buttonText.text = buttonText2;
            else buttonText.text = buttonText1;
        }
        else
        {
            if (Builder.DEBUGGING) buttonText.text = buttonText3;
            else buttonText.text = buttonText4;
        }
    }
}
