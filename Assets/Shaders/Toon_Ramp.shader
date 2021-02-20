Shader "Raven/Toon_Ramp"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ToonRamp("Toon Ramp", 2D) = "White"{}
        _SpecularColor("Specular Color" , Color) = (1,1,1,1)
        _SpeculacIntense("Specular Intense", range(0,1)) = 1
        _SpecShininess("Specular Shininess", range(1,100)) = 1
        _Step("Step", range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Toon 

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _ToonRamp;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
        fixed4 _SpecularColor;
        fixed _Step;
        fixed _SpecShininess;
        fixed _SpeculacIntense;

        half4 LightingToon(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
        {
        //diffuse light
            fixed _lightDir = dot(lightDir, s.Normal) * atten;
            fixed4 _lightClip = tex2D(_ToonRamp, fixed2(_lightDir,0.5));


        //specular
            float3 reflVector = normalize(2 * s.Normal * _lightDir - lightDir);
            fixed spec = pow(max(0, dot(reflVector,viewDir)), _SpecShininess);
            fixed4 specClip = smoothstep(0, _Step, spec * atten) * (_SpecularColor * _SpeculacIntense);

        //combinning
            fixed4 color;
            color.rgb = s.Albedo * _LightColor0 * _lightClip + specClip; //s.Albedo * ambient * _LightColor0;
            color.a= s.Alpha;

            return color;
		}

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
