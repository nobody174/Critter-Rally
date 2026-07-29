using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CritterRally.EditorTools
{
    /// <summary>
    /// One-time editor utility: creates a URP asset + renderer and assigns
    /// it as the project's active render pipeline (default and all quality
    /// levels), replacing the deprecated Built-in pipeline. Run once, before
    /// any Phase 2 art/materials exist — see ROADMAP.md 2026-07-29 decision.
    /// </summary>
    public static class URPSetup
    {
        private const string RendererAssetPath = "Assets/Settings/CritterRally_Renderer.asset";
        private const string PipelineAssetPath = "Assets/Settings/CritterRally_URP.asset";

        [MenuItem("CritterRally/Setup URP (one-time)")]
        public static void SetupUrp()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Settings"))
                AssetDatabase.CreateFolder("Assets", "Settings");

            var renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
            AssetDatabase.CreateAsset(renderer, RendererAssetPath);

            var pipelineAsset = UniversalRenderPipelineAsset.Create(renderer);
            AssetDatabase.CreateAsset(pipelineAsset, PipelineAssetPath);

            GraphicsSettings.defaultRenderPipeline = pipelineAsset;

            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, applyExpensiveChanges: false);
                QualitySettings.renderPipeline = pipelineAsset;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[URPSetup] URP asset created at {PipelineAssetPath} and assigned as the active render pipeline (default + all {QualitySettings.names.Length} quality levels).");
        }
    }
}
