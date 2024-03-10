Shader "Unlit/Lighting"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog


            #include "UnityCG.cginc"

            struct LightSrc
            {
	            float2 position;
                float intensity;
                float3 color;
	        };

            StructuredBuffer<LightSrc> lightBuffer;
            int bufferSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 camIn = tex2D(_MainTex, i.uv);
                float3 col = float3(i.uv.x, 0, 0);
                float2 uv = i.uv;
                float light = 0;
                float dist = 0;
                float3 tint = 0;
                for(int i =0; i < bufferSize; i++){
		            dist = distance(uv, lightBuffer[i].position.xy); 
                    float brit = 0.02 * (lightBuffer[i].intensity) * (1 / (dist * dist));
                    tint += brit * lightBuffer[i].color;
		        }
                tint = pow(tint, 0.1);
                tint = min(1.3, tint);
                tint = pow(tint, 2.4);
                col = camIn + tint - 2;
                return float4(col, 1);
            }
            ENDCG
        }
    }
}
