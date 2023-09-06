using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.MUIP;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour {
    [Header("Buttons")]
    [SerializeField] Button regenerateButton;
    [SerializeField] Button simulationButton;
    [SerializeField] Button informationButton;
    [SerializeField] Button biomePrefab;
    [SerializeField] ButtonManager biomeWindowConfirm;

    [Header("Windows")]
    [SerializeField] ModalWindowManager informationWindow;
    [SerializeField] ModalWindowManager biomeWindow;

    [Header("Input fields")]
    [SerializeField] TMP_InputField GRA_Input;
    [SerializeField] TMP_InputField TS_Input;
    [SerializeField] TMP_InputField LR_Input;
    [SerializeField] TMP_InputField CA_Input;
    [SerializeField] TMP_InputField R_Input;

    [Header("Other")]
    [SerializeField] ScrollRect biomeScrollView;
    [SerializeField] Slider numOfStepsSlider;
    [SerializeField] Slider biomeWeightSlider;

    [Header("Scripts")]
    [SerializeField] ProcGenConfigSO procGenConfig;
    [SerializeField] ProcGenManager procGenManager;

    private Color buttonColour;
    Vector3 regButtonScale;
    Vector3 simButtonScale;
    Vector3 infoButtonScale;

    byte[] imageBytes;

    private void Start() {

        PopulateBiomeScrollView();

        regButtonScale = regenerateButton.transform.localScale;
        simButtonScale = simulationButton.transform.localScale;
        infoButtonScale = informationButton.transform.localScale;
        
        handleHovers(regenerateButton, regButtonScale);
        handleHovers(simulationButton, simButtonScale);
        handleHovers(informationButton, infoButtonScale);

        // Add a listener to the regenerate button's onClick event
        regenerateButton.onClick.AddListener(OnRegenerateButtonClicked);

        // Add a listener to the simulation button's onClick event
        simulationButton.onClick.AddListener(OnSimulationButtonClicked);

        // Add a listener to the information button's onClick event
        informationButton.onClick.AddListener(OnInformationButtonClicked);

        // Add a listener to the numOfStepsSlider's onValueChanged event
        numOfStepsSlider.onValueChanged.AddListener(OnNumOfStepsSliderValueChanged);
    }
 
    private void handleHovers(Button targetbutton, Vector3 buttonScale){

        buttonColour = targetbutton.GetComponentInChildren<TextMeshProUGUI>().color;

        EventTrigger eventTrigger = targetbutton.gameObject.AddComponent<EventTrigger>();

        // PointerEnter event
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => { StartCoroutine(ScaleWithFade(targetbutton, buttonScale * 1.05f, Color.white)); });
        eventTrigger.triggers.Add(pointerEnter);

        // PointerExit event
        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => { StartCoroutine(ScaleWithFade(targetbutton, buttonScale, buttonColour)); });
        eventTrigger.triggers.Add(pointerExit);
    }

    private IEnumerator ScaleWithFade(Button targetbutton, Vector3 targetScale, Color targetColour) {

        targetbutton.GetComponentInChildren<TextMeshProUGUI>().color = targetColour;

        float elapsedTime = 0f;
        Vector3 startScale = targetbutton.transform.localScale;

        while (elapsedTime < 0.3f) {
            targetbutton.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / 0.3f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetbutton.transform.localScale = targetScale; 
    }

    private void PopulateBiomeScrollView() {

        // destroy any existing items
        for (int i = biomeScrollView.content.childCount - 1; i >= 0; i--) {
            Destroy(biomeScrollView.content.GetChild(i).gameObject);
        }

        // repopulate list
        for (int index = 0; index < procGenConfig.Biomes.Count; index++) {
            BiomeConfig biomeData = procGenConfig.Biomes[index];

            if(biomeData.Biome.Name == "No Life") continue;

            Button biomeItem = Instantiate(biomePrefab, biomeScrollView.content);
            Button biomeButton = biomeItem.GetComponentInChildren<Button>();
            Image biomeImage = biomeButton.GetComponentInChildren<TextMeshProUGUI>().GetComponentInChildren<Image>();
            biomeButton.GetComponentInChildren<TextMeshProUGUI>().text = biomeData.Biome.Name;
            biomeImage.color = procGenManager.hues[index];

            biomeButton.onClick.AddListener(() => OnBiomeButtonClicked(index, biomeData));
        }
    }

    private void OnRegenerateButtonClicked() {

        procGenManager.RegenerateWorld();
        
    }

    private void OnSimulationButtonClicked() {
        procGenManager.isSimulationRunning = true;
    }

    private void OnInformationButtonClicked() {
        informationWindow.Open();
    }

    private void OnBiomeButtonClicked(int biomeIndex, BiomeConfig targetBiome) {
        biomeWindow.Open();

        biomeWindow.titleText = targetBiome.Biome.Name;

        biomeWeightSlider.value = targetBiome.Weighting;

        // init inputs
        GRA_Input.text = targetBiome.Biome.GrowthRateAdvantage.ToString();
        TS_Input.text = targetBiome.Biome.TemperatureSensitivity.ToString();
        LR_Input.text = targetBiome.Biome.LightRequirements.ToString();
        CA_Input.text = targetBiome.Biome.CompetitiveAbilities.ToString();
        R_Input.text = targetBiome.Biome.Resilience.ToString();

        biomeWindowConfirm.onClick.AddListener(() => ChangeBiomeAttributes(targetBiome));
    }

    private void ChangeBiomeAttributes(BiomeConfig targetBiome) {
        targetBiome.Biome.GrowthRateAdvantage = (float.TryParse(GRA_Input.text, out float GRA_float) ? GRA_float : targetBiome.Biome.GrowthRateAdvantage);
        targetBiome.Biome.TemperatureSensitivity = (float.TryParse(TS_Input.text, out float TS_float) ? TS_float : targetBiome.Biome.TemperatureSensitivity);
        targetBiome.Biome.LightRequirements = (float.TryParse(LR_Input.text, out float LR_float) ? LR_float : targetBiome.Biome.LightRequirements);
        targetBiome.Biome.CompetitiveAbilities = (float.TryParse(CA_Input.text, out float CA_float) ? CA_float : targetBiome.Biome.CompetitiveAbilities);
        targetBiome.Biome.Resilience = (float.TryParse(R_Input.text, out float R_float) ? R_float : targetBiome.Biome.Resilience);

        targetBiome.Weighting = biomeWeightSlider.value;
        // Add a listener to the numOfStepsSlider's onValueChanged event
    }

    private void OnNumOfStepsSliderValueChanged(float value) {
        procGenManager.numOfSteps = (int)value;
    }
}
