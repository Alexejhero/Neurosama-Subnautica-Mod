Shader "Hidden/ScreenNoiseVignette"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("-", 2D) = "white" {}
        [NoScaleOffset]
        _Image ("Noise", 2D) = "gray" {}
        [Normal][NoScaleOffset]
        _Displacement ("Displacement Normal", 2D) = "black" {}
        _DispStrength ("Displacement Strength", Range(0,1)) = 1
        _Strength ("Effect Strength", Range(0,1)) = 1
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

            float2 FixUV(float2 iuv)
            { 
                float2 res;
                float2 uv = abs(iuv);
                float asp = max(_ScreenParams.x, _ScreenParams.y) / min(_ScreenParams.x, _ScreenParams.y);
                res.x = uv.x * asp * step(_ScreenParams.y, _ScreenParams.x);
                res.y = uv.y * asp * step(_ScreenParams.x, _ScreenParams.y);
                return float2(max(res.x, uv.x) * sign(iuv.x), max(res.y, uv.y) * sign(iuv.y));
            }

            sampler2D _MainTex;
            sampler2D _Image;
            sampler2D _Displacement;
            float _DispStrength;
            float _Strength;

            float4 frag (v2f i) : SV_Target
            {
                float2 straightUV = FixUV(i.uv);
                float2 randomishUV = straightUV + float2(sin(_Time.w * 24) , cos(_Time.w * 24));
                fixed4 col = tex2D(_MainTex, i.uv);
                float4 displacedCol = tex2D(_MainTex, i.uv + ((0.5 - tex2D(_Displacement, randomishUV).zw) * 2) * _DispStrength);
                fixed4 noise = tex2D(_Image, randomishUV);
                float distMask = distance(i.uv, 0.5) * _Strength;
                return (col + (noise * distMask) + (displacedCol * distMask)) * (1 - distMask);
            }
            ENDCG
        }
    }
}
