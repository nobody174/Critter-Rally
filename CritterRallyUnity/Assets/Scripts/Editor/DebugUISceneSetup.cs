using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CritterRally.UI;

namespace CritterRally.EditorTools
{
    /// <summary>
    /// One-time editor utility: adds a bare-bones Canvas UI (DebugFlowUI) to
    /// the existing Main.unity scene, wired to the GameFlow object created by
    /// GameFlowSceneSetup. No layout/art polish — see DebugFlowUI's doc
    /// comment. Run GameFlowSceneSetup first if Main.unity doesn't exist yet.
    /// </summary>
    public static class DebugUISceneSetup
    {
        private const string ScenePath = "Assets/Scenes/Main.unity";

        [MenuItem("CritterRally/Setup Debug UI (Week 3.5)")]
        public static void SetupDebugUI()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath);

            var gameFlowObject = GameObject.Find("GameFlow");
            if (gameFlowObject == null)
            {
                Debug.LogError("[DebugUISceneSetup] No GameFlow object found — run 'CritterRally > Setup Main Scene (Week 2)' first.");
                return;
            }
            var gameFlow = gameFlowObject.GetComponent<GameFlow>();

            // Re-running this tool is expected as the debug UI evolves —
            // remove any previous Canvas/DebugFlowUI so re-runs don't stack
            // duplicate UIs on top of each other.
            var existingDebugUI = Object.FindFirstObjectByType<DebugFlowUI>();
            if (existingDebugUI != null)
                Object.DestroyImmediate(existingDebugUI.gameObject);
            var existingCanvas = GameObject.Find("Canvas");
            if (existingCanvas != null)
                Object.DestroyImmediate(existingCanvas);

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }

            var canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);

            var mainMenuPanel = BuildMainMenuPanel(canvasObject.transform, out var startRaceButton);
            var critterSelectPanel = BuildCritterSelectPanel(canvasObject.transform, out var critterButtonContainer, out var critterButtonPrefab);
            var equipmentSelectPanel = BuildEquipmentSelectPanel(canvasObject.transform, out var equipmentStatsText, out var gadgetButtonContainer, out var gadgetButtonPrefab, out var equipmentContinueButton);
            var biomeSelectPanel = BuildBiomeSelectPanel(canvasObject.transform, out var biomeInfoText, out var raceForestButton);
            var raceResultPanel = BuildRaceResultPanel(canvasObject.transform, out var raceResultText, out var raceAgainButton);

            var debugUIObject = new GameObject("DebugFlowUI");
            var debugUI = debugUIObject.AddComponent<DebugFlowUI>();
            var serializedUI = new SerializedObject(debugUI);
            serializedUI.FindProperty("gameFlow").objectReferenceValue = gameFlow;
            serializedUI.FindProperty("mainMenuPanel").objectReferenceValue = mainMenuPanel;
            serializedUI.FindProperty("critterSelectPanel").objectReferenceValue = critterSelectPanel;
            serializedUI.FindProperty("biomeSelectPanel").objectReferenceValue = biomeSelectPanel;
            serializedUI.FindProperty("raceResultPanel").objectReferenceValue = raceResultPanel;
            serializedUI.FindProperty("startRaceButton").objectReferenceValue = startRaceButton;
            serializedUI.FindProperty("critterButtonContainer").objectReferenceValue = critterButtonContainer;
            serializedUI.FindProperty("critterButtonPrefab").objectReferenceValue = critterButtonPrefab;
            serializedUI.FindProperty("equipmentSelectPanel").objectReferenceValue = equipmentSelectPanel;
            serializedUI.FindProperty("equipmentSelectStatsText").objectReferenceValue = equipmentStatsText;
            serializedUI.FindProperty("gadgetButtonContainer").objectReferenceValue = gadgetButtonContainer;
            serializedUI.FindProperty("gadgetButtonPrefab").objectReferenceValue = gadgetButtonPrefab;
            serializedUI.FindProperty("equipmentContinueButton").objectReferenceValue = equipmentContinueButton;
            serializedUI.FindProperty("biomeSelectInfoText").objectReferenceValue = biomeInfoText;
            serializedUI.FindProperty("raceForestButton").objectReferenceValue = raceForestButton;
            serializedUI.FindProperty("raceResultText").objectReferenceValue = raceResultText;
            serializedUI.FindProperty("raceAgainButton").objectReferenceValue = raceAgainButton;
            serializedUI.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            Debug.Log("[DebugUISceneSetup] Debug UI added to Main.unity and wired to GameFlow.");
        }

        private static GameObject BuildMainMenuPanel(Transform parent, out Button startRaceButton)
        {
            var panel = CreatePanel("MainMenuPanel", parent);
            CreateLabel("TitleText", panel.transform, "Critter Rally", 64, new Vector2(0, 300), new Vector2(800, 100));
            startRaceButton = CreateButton("StartRaceButton", panel.transform, "Start Race", new Vector2(0, 0), new Vector2(400, 100));
            return panel;
        }

        private static GameObject BuildCritterSelectPanel(Transform parent, out Transform buttonContainer, out Button buttonPrefab)
        {
            var panel = CreatePanel("CritterSelectPanel", parent);
            CreateLabel("HeaderText", panel.transform, "Select Your Critter", 48, new Vector2(0, 700), new Vector2(800, 80));

            var containerObject = new GameObject("CritterButtonContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
            containerObject.transform.SetParent(panel.transform, false);
            var containerRect = containerObject.GetComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(600, 800);
            containerRect.anchoredPosition = new Vector2(0, 0);
            var layout = containerObject.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 20;
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;
            buttonContainer = containerObject.transform;

            var prefabButton = CreateButton("CritterButtonTemplate", panel.transform, "Critter", Vector2.zero, new Vector2(600, 150));
            prefabButton.gameObject.SetActive(false);
            buttonPrefab = prefabButton;

            return panel;
        }

        private static GameObject BuildEquipmentSelectPanel(Transform parent, out Text statsText, out Transform gadgetButtonContainer, out Button gadgetButtonPrefab, out Button continueButton)
        {
            var panel = CreatePanel("EquipmentSelectPanel", parent);
            CreateLabel("HeaderText", panel.transform, "Choose Gadgets (max 2)", 48, new Vector2(0, 800), new Vector2(800, 80));
            statsText = CreateLabel("StatsText", panel.transform, "", 32, new Vector2(0, 680), new Vector2(900, 100));

            var containerObject = new GameObject("GadgetButtonContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
            containerObject.transform.SetParent(panel.transform, false);
            var containerRect = containerObject.GetComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(700, 900);
            containerRect.anchoredPosition = new Vector2(0, 150);
            var layout = containerObject.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 15;
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;
            gadgetButtonContainer = containerObject.transform;

            var prefabButton = CreateButton("GadgetButtonTemplate", panel.transform, "Gadget", Vector2.zero, new Vector2(700, 120));
            prefabButton.gameObject.SetActive(false);
            gadgetButtonPrefab = prefabButton;

            continueButton = CreateButton("EquipmentContinueButton", panel.transform, "Continue", new Vector2(0, -750), new Vector2(400, 100));

            return panel;
        }

        private static GameObject BuildBiomeSelectPanel(Transform parent, out Text infoText, out Button raceForestButton)
        {
            var panel = CreatePanel("BiomeSelectPanel", parent);
            CreateLabel("HeaderText", panel.transform, "Select Biome", 48, new Vector2(0, 700), new Vector2(800, 80));
            infoText = CreateLabel("SelectedCritterInfoText", panel.transform, "", 36, new Vector2(0, 500), new Vector2(800, 100));
            raceForestButton = CreateButton("RaceForestButton", panel.transform, "Race: Forest", new Vector2(0, 0), new Vector2(400, 100));
            return panel;
        }

        private static GameObject BuildRaceResultPanel(Transform parent, out Text resultText, out Button raceAgainButton)
        {
            var panel = CreatePanel("RaceResultPanel", parent);
            resultText = CreateLabel("ResultText", panel.transform, "", 36, new Vector2(0, 200), new Vector2(800, 600));
            raceAgainButton = CreateButton("BackToMenuButton", panel.transform, "Back to Menu", new Vector2(0, -400), new Vector2(400, 100));
            return panel;
        }

        private static GameObject CreatePanel(string name, Transform parent)
        {
            var panel = new GameObject(name, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return panel;
        }

        private static Text CreateLabel(string name, Transform parent, string text, int fontSize, Vector2 anchoredPosition, Vector2 size)
        {
            var labelObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(parent, false);
            var rect = labelObject.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var label = labelObject.GetComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = fontSize;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.black;
            return label;
        }

        private static Button CreateButton(string name, Transform parent, string label, Vector2 anchoredPosition, Vector2 size)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.85f, 0.85f, 0.85f);

            var button = buttonObject.GetComponent<Button>();

            CreateLabel($"{name}Text", buttonObject.transform, label, 32, Vector2.zero, size);

            return button;
        }
    }
}
