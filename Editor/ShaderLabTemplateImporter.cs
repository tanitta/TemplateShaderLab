using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;
using Scriban;
using Scriban.Syntax;

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

        public void SetTemplateVariables(){
            
            foreach(var v in _variables){
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
        public List<string> GetTemplateVariables()
        {
            string assetPath = this.assetPath;
            string fullPath = Path.GetFullPath(assetPath);
            string shaderSource= File.ReadAllText(fullPath);
            var template = Template.Parse(shaderSource);

            if (template.HasErrors)
            {
                foreach (var message in template.Messages)
                {
                    Debug.LogError($"Template Error: {message}");
                }
                return null;
            }

            var collector = new VariableCollector();
            collector.Visit(template.Page);

            var context = new TemplateContext();
            var builtInObject = context.BuiltinObject;

            var builtInMembers = new HashSet<string>(builtInObject.GetMembers());
            // var builtInMembers = new HashSet<string>(
            //     builtInObject.GetMembers().Select(member => member.Key)
            // );

            var undefinedVariables = new List<string>();

            foreach (var variable in collector.VariablesUsed)
            {
                bool cond = !collector.VariablesAssigned.Contains(variable) && !builtInMembers.Contains(variable);
                if (cond)
                {
                    undefinedVariables.Add(variable);
                }
            }

            return undefinedVariables;
        }
    }

    [System.Serializable]
    public class ShaderVariable
    {
        public string name;

        public string stringValue;

        public object GetValue()
        {
            return stringValue;
        }
    }

    public class VariableCollector : ScriptVisitor
{
    public HashSet<string> VariablesUsed { get; } = new HashSet<string>();
    public HashSet<string> VariablesAssigned { get; } = new HashSet<string>();

    public override void Visit(ScriptVariableGlobal node)
    {
        VariablesUsed.Add(node.Name);
        base.Visit(node);
    }

    // public override void Visit(ScriptVariableLocal node)
    // {
    //     VariablesUsed.Add(node.Name);
    //     base.Visit(node);
    // }

    public override void Visit(ScriptAssignExpression node)
    {
        if (node.Target is ScriptVariable)
        {
            var variable = (ScriptVariable)node.Target;
            VariablesAssigned.Add(variable.Name);
        }
        base.Visit(node);
    }

    // public override void Visit(ScriptFunction node)
    // {
    //     foreach (var parameter in node.Parameters)
    //     {
    //         // parameter は ScriptParameter 型
    //         VariablesAssigned.Add(parameter.Name);
    //     }
    //     base.Visit(node);
    // }
}
}
