Shader"SchizoVFX/ARGcensor"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("MainTex", 2D) = "gray" {}
        [NoScaleOffset]
        _Images ("Noise Texture", 2DArray) = "white" {}
        [HideInInspector]
        _ScreenPosition ("Position" , Vector) = (0,0,0,0)
        _Strength ("Strength", Range(0, 1)) = 1
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

            #pragma require 2darray
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
            
            float2 FixUV(float2 iuv)
            {   
                float2 res;
                float2 uv = abs(iuv);
                float asp = max(_ScreenParams.x, _ScreenParams.y) / min(_ScreenParams.x, _ScreenParams.y);
                res.x = uv.x * asp * step(_ScreenParams.y, _ScreenParams.x);
                res.y = uv.y * asp * step(_ScreenParams.x, _ScreenParams.y);
                return float2(max(res.x, uv.x) * sign(iuv.x), max(res.y, uv.y) * sign(iuv.y));
            }

            sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
            UNITY_DECLARE_TEX2DARRAY(_Images);
            float4 _ScreenPosition;
            float _Strength;

            fixed4 frag (v2f i) : SV_Target
            {
                _ScreenPosition.xy /= _ScreenParams.xy;

                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv));
                float depthDist = saturate(( _ScreenPosition.z - depth) * 10) ;

                float2 straightUV = FixUV((i.uv - _ScreenPosition.xy) * _ScreenPosition.z);

                float4 colors = UNITY_SAMPLE_TEX2DARRAY(_Images, float3(straightUV + _ScreenPosition.xy, _ScreenPosition.w));

                fixed4 col = tex2D(_MainTex, i.uv);

                return lerp(col, colors, colors.w * depthDist * _Strength);
            }
            ENDCG
        }
    }
}
