Shader"SchizoVFX/HiyoriEffect"
{
    Properties
    {
        [HideInInInspector]
        _MainTex ("MainTex", 2D) = "gray" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        [HideInInInspector]
        _ScreenPosition ("Position" , Vector) = (0,0,0,0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _ScreenPosition;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 noiseCol = tex2D(_NoiseTex, float2(_SinTime.x + i.uv.x, _CosTime.x + i.uv.y) * _ScreenParams.wz);
                _ScreenPosition.xy /= _ScreenParams.xy;
                bool isInfront = (_ScreenPosition.z > 0)? true: false;
                float fac = saturate(distance(_ScreenPosition.xy, i.uv * _ScreenParams.wz) * 10);
                if(isInfront)
                {
                    col.rgb *= fac;
                    col.rgb += noiseCol.rgb * (1 - fac);
                }
                return col;
            }
            ENDCG
        }
    }
}
