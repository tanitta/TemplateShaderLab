using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;
using Scriban;

namespace trit.ShaderLabTemplate{
    [ScriptedImporter(1, "shadertmp")]
    public class ShaderLabTemplateImporter : ScriptedImporter
    {
        [SerializeField]
        public List<ShaderVariable> _variables = new List<ShaderVariable>();

        public override void OnImportAsset(AssetImportContext ctx)
        {
            string assetPath = ctx.assetPath;
            string fullPath = Path.GetFullPath(assetPath);
            string shaderSource= File.ReadAllText(fullPath);
            var template = Template.Parse(shaderSource);
            if (template.HasErrors)
            {
                foreach (var message in template.Messages)
                {
                    Debug.LogError($"Template Error: {message}");
                }
                return;
            }
            var page = template.Page;



            var model = CreateModelFromProperties();
            string renderedShaderSource = template.Render(model);
            var shader = ShaderUtil.CreateShaderAsset(ctx, renderedShaderSource, true);

            if (shader != null)
            {
                ctx.AddObjectToAsset("MainAsset", shader);
                ctx.SetMainObject(shader);
            }
            else
            {
                Debug.LogError("Failed to import shaderLab template.");
            }
        }

        private object CreateModelFromProperties()
        {
            var model = new Dictionary<string, object>();
            foreach (var variable in _variables)
            {
                model[variable.name] = variable.GetValue();
            }
            return model;
        }
    }

    [System.Serializable]
    public class ShaderVariable
    {
        public string name;

        public string stringValue;
        public string valueType;

        public object GetValue()
        {
            return stringValue;
        }

        public object GetValueType()
        {
            return valueType;
        }
    }

    public class VariableCollector : Scriban.Syntax.ScriptVisitor
    {
        public HashSet<string> Variables { get; } = new HashSet<string>();

        public override void Visit(Scriban.Syntax.ScriptVariableGlobal node)
        {
            Variables.Add(node.Name);
            base.Visit(node);
        }

        public override void Visit(Scriban.Syntax.ScriptVariableLocal node)
        {
            Variables.Add(node.Name);
            base.Visit(node);
        }
    }
}
