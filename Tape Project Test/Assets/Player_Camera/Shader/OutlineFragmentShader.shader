﻿Shader "Unlit/OutlineFragmentShader" {

	Properties{
		_OutlineColor("アウトラインカラー", Color) = (0, 0, 0, 0)
		_OutlineWidth("アウトラインの太さ", float) = 0.1
		_AmbientColor("環境光カラー", Color) = (0.2, 0.2, 0.2, 1)
		_DiffuseColor("拡散反射光カラー", Color) = (1, 1, 1, 1)
		_SpecularColor("鏡面反射光カラー", Color) = (1, 1, 1, 1)
		_SpecularShininess("鏡面反射光指数", float) = 20.0
		_MainTex("ベーステクスチャ", 2D) = "white" {}
	}

		SubShader{
		Tags{ "Queue" = "Geometry" }

		//1パス目、本体描画
		Pass
		{
		Tags{ "LightMode" = "ForwardBase" }
		Stencil{
			Ref 2
			Comp always
			Pass replace
			Fail replace
			Zfail keep
		}


		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag			
		#include "UnityCG.cginc"
		#include "UnityLightingCommon.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 color : COLOR0;
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		//環境光カラー
		uniform float4 _AmbientColor;
		//拡散反射光カラー
		uniform float4 _DiffuseColor;
		//鏡面反射カラー
		uniform float4 _SpecularColor;
		//鏡面反射指数
		uniform float _SpecularShininess;
		//テクスチャ
		uniform sampler2D _MainTex;

		//頂点シェーダー
		v2f vert(appdata v)
		{
			v2f o;
			//法線ベクトルをワールド座標系に変換
			float3 N = UnityObjectToWorldNormal(v.normal);
			//ライト方向のベクトルを求める
			float3 L = normalize(WorldSpaceLightDir(v.vertex));
			//ワールド座標系のライト方向のベクトルを求める
			float3 V = normalize(WorldSpaceViewDir(v.vertex));
			//２等分ベクトルを求める
			float3 H = normalize(L + V);
			//拡散反射光の計算
			float diffuse = max(dot(N, L), 0);
			//鏡面反射光の計算
			float specular = pow(max(dot(N, H), 0), _SpecularShininess);
			//拡散反射光カラーの計算
			float4 diffuseColor = diffuse * _DiffuseColor * _LightColor0;
			//鏡面反射光カラーの計算
			float4 specularColor = specular * _SpecularColor * _LightColor0;
			//最終カラーの計算
			o.color = diffuseColor + specularColor + _AmbientColor;
			//テクスチャ座標の出力
			o.uv = v.uv;
			//ワールド・ビュー・プロジェクション変換
			o.vertex = UnityObjectToClipPos(v.vertex);
			return o;
		}
		//ピクセルシェーダー
		fixed4 frag(v2f i) : SV_Target
		{
			//テクスチャカラーの取得
			float4 baseColor = tex2D(_MainTex, i.uv);
			//最終カラーの出力
			return i.color * baseColor;
		}
			ENDCG
	}

		//2パス目、アウトライン描画
		Pass
		{
			//Cull Front
			ZTest Always
			Tags{ "LightMode" = "ForwardBase" }

			Stencil{
				Ref 2
				Comp notequal
			}

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			//アウトラインカラー
			float4 _OutlineColor;
			//アウトラインの太さ
			float _OutlineWidth;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			//頂点シェーダー
			v2f vert(appdata v)
			{
				float distance = UnityObjectToViewPos(v.vertex).z;
				//頂点の法線方向に太さをプラス
				v.vertex.xyz += v.normal * -distance * _OutlineWidth;

				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			//ピクセルシェーダー
			fixed4 frag(v2f i) : SV_Target
			{
				//アウトラインカラーをそのまま出力
				return _OutlineColor;
			}

				ENDCG
		}

		//3パス目
		Pass
			{
				Tags{ "LightMode" = "ShadowCaster" }

				CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_shadowcaster
#include "UnityCG.cginc"

			struct v2f
			{
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
					return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
				ENDCG
			}

				Pass
			{
				Tags
			{
				"LightMode" = "ForwardBase"
			}

				Cull Back
				AlphaToMask On

				CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "AutoLight.cginc"

#pragma multi_compile_fwdbase

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos      : SV_POSITION;
				SHADOW_COORDS(0)
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				TRANSFER_SHADOW(o)
					return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed shadow = SHADOW_ATTENUATION(i);
			return fixed4(0,0,0,1 - shadow);
			}
				ENDCG
			}

	}
}