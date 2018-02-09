Shader "custom/DiffuseOutline"{

	Properties{

		_OutlineColor("OutlineColor", Color) = (1, 1, 1, 1)
		_OutlineWidth("OutlineWidth", float) = 0.1
		_Color("Main Color", Color) = (1 ,1 ,1 ,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_DetailMap("Detail (RGB)", 2D) = "gray" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_DetailBumpMap("Normalmap(Detail)", 2D) = "bump" {}
		_DetailScale("DetailScale", Range(0.01, 1)) = 0.4
		_Stencil("Stencil", float) = 2
		[Enum(Off, 0, On, 1)]
		_ZWrite("Zwrite", float) = 0

	}

		SubShader{

		Tags{ "RenderType" = "Opaque" "Queue" = "Transparent" }
		LOD 250

			Stencil{
				Ref [_Stencil]
				Comp always
				Pass replace
				Fail replace
				Zfail keep
			}

		CGPROGRAM

#pragma surface surf Lambert 
#include "UnityCG.cginc"

		sampler2D _MainTex;
	sampler2D _DetailMap;
	sampler2D _BumpMap;
	sampler2D _DetailBumpMap;
	fixed4 _Color;
	half _DetailScale;



	struct Input {
		float2 uv_MainTex;
		float2 uv_DetailMap;
		float2 uv_DetailBumpMap;
	};

	void surf(Input IN, inout SurfaceOutput o)
	{
		// デティールマップ（カラー）をテクスチャカラーと合成する 
		fixed4  c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		c.rgb *= tex2D(_DetailMap,IN.uv_DetailMap).rgb * 2;

		// ２つのノーマルマップから法線ベクトルをフェッチして合成する 
		fixed4 Normal1 = tex2D(_BumpMap, IN.uv_MainTex);
		fixed4 Normal2 = tex2D(_DetailBumpMap ,IN.uv_DetailBumpMap) * float4 (_DetailScale,_DetailScale,0,0);

		//アウトプットノーマルに法線ベクトルをセットする 
		o.Normal = UnpackNormal(Normal1 + Normal2);

		// アウトプットカラーをセットする 
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG

		Stencil{
		Ref [_Stencil]
		Comp notequal
	}

		

		CGPROGRAM

		#pragma surface surf Lambert vertex:vert noambient 

		float4 _MainColor;
	float4 _OutlineColor;
	float _OutlineWidth;

	struct Input {
		float4 vertexColor : COLOR;
	};

	void vert(inout appdata_full v, out Input o) {
		float distance = -UnityObjectToViewPos(v.vertex).z;
		v.vertex.xyz += v.normal * distance * _OutlineWidth;
		o.vertexColor = v.color;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = _OutlineColor.rgb;
		o.Emission = _OutlineColor.rgb;
	}
	ENDCG
	}
		Fallback "Diffuse"
}