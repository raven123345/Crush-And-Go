Shader "Raven/Toon_V2F"
{ 
    Properties 
    { 
        [HDR]
        _MainTint("Main Tint", Color) = (1,1,1,1) 
        _MainTex("Diffuse", 2D) = "White" {} 
        [HideInInspector]
        _NormalTex("Normal", 2D) = "bump"{}
		_Ramp("Ramp Texture", 2D) = "White" {}
        _RampOffset("Ramp Offsset", float) = 1
    } 
    SubShader 
    { 
       //  
        Tags{ 
            "RenderType"="Opaque" 
            "Queue"="Geometry"  
            "LightMode"="ForwardBase" 
            } 
        Pass 
        { 
             
            Cull Back 
            ZTest LEqual 
 
            CGPROGRAM 
            #pragma multi_compile_fwdbase //за самозасенчването 
            #pragma target 3.0 
            #pragma vertex vert 
            #pragma fragment frag 
            #include "UnityCG.cginc" 
            #include "UnityLightingCommon.cginc" 
            #include "AutoLight.cginc" 
 
            sampler2D _MainTex; 
			sampler2D _Ramp;
            sampler2D _NormalTex;
 
            half4 _MainTex_ST;
            half4 _Ramp_ST;
            half4 _NormalTex_ST;
 
            fixed4 _MainTint; 
            fixed _RampOffset;
 
            struct InputVertex //appdata_t 
            { 
                float4 vertex : POSITION; 
                float4 texcoords : TEXCOORD0;
                float3 normal : NORMAL; 
                float4 tangent : TANGENT;
                float texcoordBlend : TEXCOORD1;
                fixed4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            }; 
            struct OutputVertex  //v2f
            { 
                float4 pos : SV_POSITION; 
                float2 texcoord : TEXCOORD0;
                float3 normalDir : TEXCOORD1; 
                float3 tangentDir : TEXCOORD2; 
                float3 binormalDir : TEXCOORD3; 
                float3 worldPos : TEXCOORD4; 
                float3 worldNormal : TEXCOOR5; 
                float4 _ShadowCoord : TEXCOOR6; // за самозасенчването 
                fixed4 color : COLOR; 
                fixed blend : TEXCOORD7;
                UNITY_FOG_COORDS(8)
                float4 projPos : TEXCOORD9;
                float2 texcoord2 : TEXCOORD10;
                UNITY_VERTEX_OUTPUT_STEREO  
            }; 
 
            OutputVertex vert(InputVertex i) 
            { 
                OutputVertex OUT; 
 
                OUT.normalDir = normalize(half3(mul(half4(i.normal, 0.0), unity_WorldToObject).xyz)); 
                OUT.tangentDir = normalize(half3(mul(unity_ObjectToWorld, half4(half3(i.tangent.xyz),0.0)).xyz)); 
                OUT.binormalDir = normalize(cross(OUT.normalDir, OUT.tangentDir) * i.tangent.w); 
 
                OUT.worldNormal = mul(unity_ObjectToWorld, i.normal); 
                OUT.worldPos = mul(unity_ObjectToWorld, i.vertex);
                OUT.pos = UnityObjectToClipPos(i.vertex); 
                OUT.color = i.color * _MainTint;
                OUT.texcoord = TRANSFORM_TEX(i.texcoords.xy, _MainTex);
                OUT.texcoord2 = TRANSFORM_TEX(i.texcoords.zw, _MainTex);
                OUT.blend = i.texcoordBlend;
                TRANSFER_VERTEX_TO_FRAGMENT( OUT ); 
                return OUT; 
            } 
 
            fixed4 frag(OutputVertex i) : COLOR 
            { 
                half3x3 localToWorldCoords = half3x3( 
                i.tangentDir, 
                i.binormalDir, 
                i.normalDir 
                ); 
            fixed3 normalTex = UnpackNormal(tex2D(_NormalTex, half4(i.texcoord,1,1) * _NormalTex_ST.xy + _NormalTex_ST.zw));
                half3 normalDir = normalize(mul(normalTex, localToWorldCoords));
 
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz); 
 
  				fixed3 fresnel = saturate(dot(normalDir, viewDir));
				fixed3 frtex = tex2D(_Ramp, fixed4(fresnel.x * _RampOffset, 0.5,0.5, 0.5));
               // fixed3 diff = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw) * _MainTint; 
                fixed4 colA = tex2D(_MainTex, i.texcoord);
                fixed4 colB = tex2D(_MainTex, i.texcoord2);

                fixed4 colCombined = lerp(fixed4(colA.rgb, 0), fixed4(colB.rgb, 1), i.blend);
                fixed alpha = lerp(0, 1, i.blend);
  
                fixed3 c = 2.0f * i.color * colCombined * frtex;
 
                return fixed4(c, colCombined.a);
//                return attenuation; 
            } 
 
            ENDCG 
        } 
    } 
    FallBack "Diffuse" 
}  