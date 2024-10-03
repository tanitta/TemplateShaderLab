using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Scriban;
using UnityEditor.AssetImporters;
using Scriban.Syntax;

namespace trit.ShaderLabTemplate
{
    [CustomEditor(typeof(ShaderLabTemplateImporter))]
    public class ScribanShaderImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var importer = target as ShaderLabTemplateImporter;

            if(GUILayout.Button("Detect Global Variables",GUILayout.Width(240))){
                var variables = importer.GetTemplateVariables();
                foreach(var v in variables){
                    Debug.Log(v);
                }
                SyncVariables(importer, variables);
                EditorUtility.SetDirty(importer);
            };
            base.OnInspectorGUI();
        }

        private void SyncVariables(ShaderLabTemplateImporter importer, List<string> variableNames)
        {
            var existingVariables = importer._variables;

            foreach (var variableName in variableNames)
            {
                if (!existingVariables.Exists(v => v.name == variableName))
                {
                    existingVariables.Add(new ShaderVariable() { name = variableName });
                }
            }

            // Remove
            // existingVariables.RemoveAll(v => !variableNames.Contains(v.name));
        }
    }

}
