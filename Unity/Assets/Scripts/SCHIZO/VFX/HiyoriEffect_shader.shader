Shader"SchizoVFX/HiyoriEffect"
{
    Properties
    {
        [HideInInInspector]
        _MainTex ("MainTex", 2D) = "gray" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        [Normal]     
        _DispMap ("Displacement map", 2D) = "white" {}
        [HideInInInspector]
        _ScreenPosition ("Position" , Vector) = (0,0,0,0)
        _Distance ("Distance" , float) = 0.5
        _Strength ("Strength", float) = 0.5
        _Condition("Frequency", Range (-1, 1)) = -0.5
        _Blend ("Blend", float) = 0.75
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
            sampler2D _DispMap;
            float4 _ScreenPosition;
            float _Distance;
            float _Strength;
            float _Condition;
            float _Blend;

            float2 FixUV(float2 iuv)
            {   
                float2 res;
                float2 uv = abs(iuv);
                float asp = max(_ScreenParams.x, _ScreenParams.y) / min(_ScreenParams.x, _ScreenParams.y);
                res.x = uv.x * asp * step(_ScreenParams.y, _ScreenParams.x);
                res.y = uv.y * asp * step(_ScreenParams.x, _ScreenParams.y);
                return float2(max(res.x, uv.x) * sign(iuv.x), max(res.y, uv.y) * sign(iuv.y));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                _Blend = max(0, 10 - (_Blend * _ScreenParams.w * _Distance));
                _Strength = max(0, _Strength);
                _ScreenPosition.xy /= _ScreenParams.xy;
                float2 aspectUV = FixUV(i.uv);
                float4 offset = (0.5 - tex2D(_DispMap, aspectUV + (_ScreenPosition.w * 3))) * _Strength;
                float fac = saturate(distance(FixUV(_ScreenPosition.xy) *_Distance, aspectUV * _Distance ));
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 colDist = tex2D(_MainTex, i.uv + ((offset.wz) * (1 - fac)));
                fixed4 noiseCol = tex2D(_NoiseTex, aspectUV + (_ScreenPosition.w * 5));
                bool isInfront = (_ScreenPosition.z > 0)? true : false;
                if(isInfront)
                {
                    int condition = step(_ScreenPosition.w , _Condition + _Distance * 0.05);
                    col.rgb = (col.rgb * condition) + ((1 - condition) * lerp((noiseCol.rgb + (col.rgb * _Blend)) / (_Blend + 1), colDist.rgb, fac));
                }
                return col;
            }
            ENDCG
        }
    }
}
