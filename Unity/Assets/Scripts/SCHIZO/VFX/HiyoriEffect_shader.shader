Shader"SchizoVFX/HiyoriEffect"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("MainTex", 2D) = "gray" {}
        [NoScaleOffset]
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        [Normal][NoScaleOffset]
        _DispMap ("Displacement map", 2D) = "white" {}
        [HideInInspector]
        _ScreenPosition ("Position" , Vector) = (0,0,0,0)
        _ZeroDistance("Zero Distance", Range(2, 500)) = 50
        _Strength ("Strength", Range(0, 1)) = 1
        _DisplacementStrength("Displacement Strength", Range(0, 1)) = 1
        _Radius ("Max Radius at Zero Distance", Range(0, 1)) = 1
        _FlickerTreshold("Flicker Treshold", Range (0, 1)) = 0.5
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

            sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            sampler2D _DispMap;
            float4 _ScreenPosition;
            float _Radius;
            float _ZeroDistance;
            float _Strength;
            float _FlickerTreshold;
            float _DisplacementStrength;

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv));
                float depthDist = saturate( -(_ScreenPosition.z - depth));

                float2 straightUV = FixUV(i.uv);
                float2 randomishUV = straightUV + float2(sin(_Time.w * 24) , cos(_Time.w * 24));
                _ScreenPosition.xy /= _ScreenParams.xy;

                _Radius *= 1 - ( (1 / _ZeroDistance) * _ScreenPosition.z);

                float mask = (1 - saturate(distance(straightUV, FixUV(_ScreenPosition.xy)) + (1 - _Radius))) * _Strength;
                mask *= depthDist;
                mask *= step((_FlickerTreshold + 1) / 2, _ScreenPosition.w);

                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 noiseCol = tex2D(_NoiseTex, randomishUV);
                float2 displacement = ((0.5 - tex2D(_DispMap, randomishUV)) * 2).zw;
                fixed4 displacedCol = tex2D(_MainTex, i.uv + (displacement * _DisplacementStrength));

                return (col + (noiseCol * mask) + (displacedCol * mask)) * (1 - mask);
            }
            ENDCG
        }
    }
}
