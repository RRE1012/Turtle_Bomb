// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UnityMaskMan/Main" {
	Properties
	{
		_Edge ("Edge Thickness", Float) = 0
		_Edge_Saturation ("Edge Saturation", Range(0.01, 1.0)) = 0.6
		_Edge_Brightness ("Edge Brightness", Range(0.01, 1.0)) = 0.8
				
		_MainTex ("Diffuse", 2D) = "white" {}

		_SkinHighLight ("Skin Highlight Color", Color) = (1,1,1,1)
		_SkinLowLight ("Skin Lowlight Color", Color) = (1,0.86,0.78,1)
		_SkinHighX ("Skin High X", Range(0.01, 1.0)) = 0.31
		_SkinLowX ("Skin Low X", Range(0.01, 1.0)) = 0.59

		_RimHighX ("Rim High X", Range(0.01, 1.0)) = 0.61
		_RimLowX ("Rim Low X", Range(0.01, 1.0)) = 0.72
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
			"Queue"="Geometry"
			"LightMode" = "ForwardBase"
		}

		Pass
		{
			Cull Back
			ZTest LEqual
			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			float4 _LightColor0;

			sampler2D _MainTex;

			half _SkinHighX;
			half _SkinLowX;
			half4 _SkinHighLight;
			half4 _SkinLowLight;
			half _RimHighX;
			half _RimLowX;

			struct v2f
			{
				float4 pos    : SV_POSITION;
				float3 normal : TEXCOORD0;
				float2 uv     : TEXCOORD1;
				float3 eyeDir : TEXCOORD2;
				float3 lightDir : TEXCOORD3;
			};

			v2f vert( appdata_base v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				o.normal = normalize( mul( unity_ObjectToWorld, half4( v.normal, 0 ) ).xyz );
				
				// Eye direction vector
				half4 worldPos =  mul( unity_ObjectToWorld, v.vertex );
				o.eyeDir = normalize( _WorldSpaceCameraPos - worldPos );

				o.lightDir = _WorldSpaceLightPos0.xyz;

				return o;
			}

			float4 frag( v2f i ) : COLOR
			{
				half4 diffSamplerColor = tex2D( _MainTex, i.uv );

				half normalDotEye = dot( i.normal, i.eyeDir );
				half falloffU = clamp( 1 - abs( normalDotEye ), 0.02, 0.98 );
				half skinX = clamp((falloffU-_SkinHighX)/(_SkinLowX-_SkinHighX), 0, 1);
				half4 skinLightColor =  lerp(_SkinHighLight, _SkinLowLight, skinX);
				half3 combinedColor = skinLightColor.rgb * diffSamplerColor.rgb;

				// Rimlight
				half rimlightDot = saturate( 0.5 * ( dot( i.normal, i.lightDir ) + 1.0 ) );
				falloffU = saturate( rimlightDot * falloffU );
				half rimX = clamp((falloffU-_RimHighX)/(_RimLowX-_RimHighX), 0, 1);
				falloffU = lerp(0, 1, rimX);
				half3 lightColor = diffSamplerColor.rgb * 0.5; // * 2.0;
				combinedColor += falloffU * lightColor;

				return half4( combinedColor, diffSamplerColor.a ) * _LightColor0;
			}


			ENDCG
		}

		Pass
		{
			Cull Front
			ZTest Less
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"


			float _Edge;
			float _Edge_Saturation;
			float _Edge_Brightness;
			sampler2D _MainTex;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert( appdata_base v )
			{
				v2f o;
				o.uv = v.texcoord.xy;

				half4 projSpacePos = UnityObjectToClipPos( v.vertex );
				half4 projSpaceNormal = normalize( UnityObjectToClipPos( half4( v.normal, 0 ) ) );
				half4 scaledNormal = _Edge * 0.003 * projSpaceNormal;

				scaledNormal.z += 0.00001;
				o.pos = projSpacePos + scaledNormal;

				return o;
			}

			float4 frag( v2f i ) : COLOR
			{
				half4 _mainColor = tex2D( _MainTex, i.uv );

				half maxValue = max( max( _mainColor.r, _mainColor.g ), _mainColor.b );
				half4 _saturateColor = _mainColor;

				maxValue -= ( 1.0 / 255.0 );
				half3 lerpVals = saturate( ( _saturateColor.rgb - float3( maxValue, maxValue, maxValue ) ) * 255.0 );
				_saturateColor.rgb = lerp( _Edge_Saturation * _saturateColor.rgb, _saturateColor.rgb, lerpVals );
				
				return float4( _Edge_Brightness * _saturateColor.rgb * _mainColor.rgb, _mainColor.a ); 
			}



			ENDCG
		}

	}
	FallBack "Diffuse"
}
