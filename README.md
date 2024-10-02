# ShaderLab Template Importer

## Overview

The **ShaderLabTemplateImporter** is a custom Unity asset importer that processes shader templates with the `.shadertmp` extension. It leverages the [Scriban](https://github.com/scriban/scriban) templating engine to render dynamic shader code based on user-defined variables. This allows for more flexible and maintainable shader development within the Unity engine.

## Features

- **Custom Shader Importing**: Automatically imports `.shadertmp` files and converts them into usable shader assets.
- **Template Rendering**: Uses Scriban to parse and render shader templates with dynamic content.
- **Variable Management(WIP)**: Supports the use of custom variables within shader templates to control shader properties and behavior.

## Installation
1. **Install NuGetForUnity**: Install [GlitchEnzo/NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) to your project.
1. **Add UPM Package**: Add `https://github.com/tanitta/ShaderLabTemplate.git` to Package Manager or add `"net.tanitta.shader_lab_template": "https://github.com/tanitta/ShaderLabTemplate.git#main"` to Packages/manifest.json.

## Usage

### 1. Create a Shader Template

- Write your shader code using Scriban's templating syntax.
- Save the file with a `.shadertmp` extension.

**Example (`MyShader.shadertmp`):**

```hlsl
{{ $textures = 4}}
Shader "Custom/MyDynamicShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        {{for $i in (0..$textures)}}
            _Tex{{ $i }}("Texture", 2D) = "white" {}
        {{end}}
    }
    SubShader
    {
        Tags { "RenderType"="{{ renderType }}" }
        LOD {{ lod }}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            {{for $i in (0..$textures)}}
                sampler2D _Tex{{ $i }};
            {{end}}

            ...

            ENDCG
        }
    }
}
```

### 2. Import the Shader Template

- When you save the `.shadertmp` file, Unity will automatically use the `ShaderLabTemplateImporter` to process it.
- Select the shader template in the Unity Project window to view its Inspector.

### 3. Define Template Variables

- In the Inspector, you'll see a list of variables extracted from your template.
- Assign values to these variables as needed.

**Variables:**

- `renderType`: e.g., `Opaque`
- `lod`: e.g., `200`
- `shaderCode`: e.g., custom shader code snippets

### 4. Use the Generated Shader

- The importer will generate a shader asset from your template and variables.
- You can now use this shader in your materials like any .shader asset.

## Scriban Templating
Read [scriban/doc at master · scriban/scriban](https://github.com/scriban/scriban/tree/master/doc).
<!-- 
- **Variables**: Use `{{ variableName }}` to include variables in your template.
- **Control Flow**:
  - **If Statements**: `{{ if condition }}...{{ endif }}`
  - **Loops**: `{{ for item in collection }}...{{ endfor }}`
- **Functions**: Leverage built-in functions or define custom ones as needed.
-->

<!-- 
**Example Usage in Shader Template:**

```hlsl
float _Brightness = {{ brightness }};
{{ if useColorTint }}
float4 _ColorTint = {{ colorTint }};
{{ endif }}
```

<!-- 
## Example Workflow

1. **Create a Shader Template (`ExampleShader.shadertmp`):**

   ```hlsl
   Shader "Custom/ExampleShader"
   {
       Properties
       {
           _MainTex ("Texture", 2D) = "white" {}
       }
       SubShader
       {
           Tags { "RenderType"="{{ renderType }}" }
           LOD {{ lod }}

           Pass
           {
               CGPROGRAM
               #pragma vertex vert
               #pragma fragment frag

               sampler2D _MainTex;
               float _Brightness = {{ brightness }};
               {% if useColorTint %}
               float4 _ColorTint = {{ colorTint }};
               {% endif %}

               // Shader code...

               ENDCG
           }
       }
   }
   ```

2. **Import the Template:**

   - Unity automatically processes the `.shadertmp` file with the custom importer.

3. **Set Variables in the Inspector:**

   - **renderType**: `"Opaque"`
   - **lod**: `300`
   - **brightness**: `1.5`
   - **useColorTint**: `true`
   - **colorTint**: `float4(1, 0, 0, 1)`

4. **Use the Generated Shader:**

   - The shader is now available as an asset and can be applied to materials.
-->
