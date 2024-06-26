﻿Shader "MustHave/InstancedSpriteShader" 
{
	Properties     
    {
        _MainTex("Texture", 2D) = "white" {}     
    }  
	SubShader 
	{
		Pass 
		{
			Tags
			{ 
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
				"IgnoreProjector" = "True"
			}
			//Blend SrcAlpha OneMinusSrcAlpha
			Blend Off
			Cull Back
		
			CGPROGRAM
			#pragma multi_compile_instancing
			#pragma shader_feature INSTANCE_DATA_VARIANT INSTANCE_MATRIX_VARIANT

			#pragma vertex vert
			#pragma fragment frag

			#define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
			#define EPSILON 0.00001f

			#include "UnityCG.cginc"
			#include "UnityIndirect.cginc"

			struct InstanceData
			{
				float3 clipPosition;
				float4 color;
				float2 scale;
			};

			static const float ScreenAspect = _ScreenParams.y / _ScreenParams.x;

			StructuredBuffer<InstanceData> _InstanceBuffer;

			StructuredBuffer<float4> _ColorBuffer;

			sampler2D _MainTex;

			float _MinDepth;

			struct appdata
			{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v, uint vertexID: SV_VertexID, uint instanceID : SV_InstanceID)
			{
				UNITY_SETUP_INSTANCE_ID(v);

				//InitIndirectDrawArgs(0);
				//instanceID = GetIndirectInstanceID(instanceID);

				v2f o; //= (v2f)0;

				#ifdef INSTANCE_DATA_VARIANT
				{
					InstanceData instance = _InstanceBuffer[instanceID];
					float3 iClipPos = instance.clipPosition;
					float2 iScale = instance.scale;

					//float2 xy = v.vertex.xy * 2;
					float2 xy = (v.texcoord - 0.5) * 2;

					#if UNITY_REVERSED_Z
					iClipPos.z = clamp(UNITY_NEAR_CLIP_VALUE - iClipPos.z + _MinDepth, -1, 1);
					#else
					iClipPos.z = clamp(UNITY_NEAR_CLIP_VALUE + iClipPos.z - EPSILON, -1, 1);
					#endif

					#if UNITY_UV_STARTS_AT_TOP
					iClipPos.y *= -1;
					xy.y *= -1;
					#endif
					xy.x *= ScreenAspect;

					/* UnityObjectToClipPos must be called and used for Graphics.RenderMeshInstanced to work */
					float4 clipPos = UnityObjectToClipPos(v.vertex);

					o.position = float4(iClipPos.xy + xy * iScale, iClipPos.z, clipPos.w);
					o.color =  instance.color;
				}
				#elif defined (INSTANCE_MATRIX_VARIANT)
				{
					o.position = UnityObjectToClipPos(v.vertex);
					o.color = _ColorBuffer[instanceID];
				}
				#endif

				o.uv = v.texcoord;

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.uv);
				color.a = step(EPSILON, color.a);

				if (color.a  < EPSILON)
				{
					discard;
				}
				else
				{
					color.rgb *= i.color.rgb;
					color.a = i.color.a;
				}
				return color;
			}
			ENDCG
		}
	}
}
