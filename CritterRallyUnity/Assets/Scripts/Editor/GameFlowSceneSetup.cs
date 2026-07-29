using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using CritterRally.Critters;
using CritterRally.UI;

namespace CritterRally.EditorTools
{
    /// <summary>
    /// One-time editor utility: builds the Week 2 placeholder Main scene
    /// with a GameFlow object wired to a CritterSpeciesLookup asset. Cubes
    /// stand in for critters per the Phase 1 "no 3D models yet" rule.
    /// </summary>
    public static class GameFlowSceneSetup
    {
        private const string ScenePath = "Assets/Scenes/Main.unity";
        private const string LookupAssetPath = "Assets/ScriptableObjects/Critters/CritterSpeciesLookup.asset";
        private const string FoxAssetPath = "Assets/ScriptableObjects/Critters/Fox.asset";
        private const string FrogAssetPath = "Assets/ScriptableObjects/Critters/Frog.asset";

        [MenuItem("CritterRally/Setup Main Scene (Week 2)")]
        public static void SetupScene()
        {
            var fox = CreateOrLoadSpecies(FoxAssetPath, "Fox",
                55, 2.4f, 40, 1.6f, 25, 1.0f, 20, 0.8f, 45, 1.8f);
            var frog = CreateOrLoadSpecies(FrogAssetPath, "Frog",
                25, 1.0f, 50, 2.2f, 20, 0.8f, 55, 2.4f, 30, 1.2f);

            var lookup = AssetDatabase.LoadAssetAtPath<CritterSpeciesLookup>(LookupAssetPath);
            if (lookup == null)
            {
                lookup = ScriptableObject.CreateInstance<CritterSpeciesLookup>();
                AssetDatabase.CreateAsset(lookup, LookupAssetPath);
            }
            lookup.allSpecies.Clear();
            lookup.allSpecies.Add(fox);
            lookup.allSpecies.Add(frog);
            EditorUtility.SetDirty(lookup);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var flowObject = new GameObject("GameFlow");
            var gameFlow = flowObject.AddComponent<GameFlow>();
            var serializedFlow = new SerializedObject(gameFlow);
            serializedFlow.FindProperty("speciesLookup").objectReferenceValue = lookup;
            serializedFlow.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            Debug.Log($"[GameFlowSceneSetup] Main scene created at {ScenePath} with GameFlow wired to CritterSpeciesLookup ({lookup.allSpecies.Count} species).");
        }

        private static CritterSpecies CreateOrLoadSpecies(
            string path, string name,
            float baseSprint, float growthSprint,
            float baseJump, float growthJump,
            float baseDig, float growthDig,
            float baseSwim, float growthSwim,
            float baseBalance, float growthBalance)
        {
            var species = AssetDatabase.LoadAssetAtPath<CritterSpecies>(path);
            if (species == null)
            {
                species = ScriptableObject.CreateInstance<CritterSpecies>();
                AssetDatabase.CreateAsset(species, path);
            }

            species.speciesName = name;
            species.baseSprint = baseSprint; species.growthSprint = growthSprint;
            species.baseJump = baseJump; species.growthJump = growthJump;
            species.baseDig = baseDig; species.growthDig = growthDig;
            species.baseSwim = baseSwim; species.growthSwim = growthSwim;
            species.baseBalance = baseBalance; species.growthBalance = growthBalance;

            EditorUtility.SetDirty(species);
            return species;
        }
    }
}
