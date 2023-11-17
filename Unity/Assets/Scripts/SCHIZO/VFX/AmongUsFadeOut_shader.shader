Shader "Hidden/AmongUsFadeOut_shader"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("_", 2D) = "white" {}
        _Color("Inner Color", Color) = (1, 0.3, 0.3, 1)
        _Color0("Outer Color", Color) = (0.3, 1, 0.3, 1)
        _Strength("Strength", Range(0, 1)) = 0.5
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
            float4 _Color;
            float4 _Color0;
            float _Strength;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 _ColorInner = _Color;
                float4 _ColorOuter = _Color0;
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 center = i.uv + 0.5;
                float dist = distance(i.uv, 0.5);
                _ColorOuter = lerp(col, _ColorOuter, _ColorOuter.w);
                _ColorInner = lerp(col, _ColorInner, _ColorInner.w);
                return lerp(col, (_ColorOuter * dist) + (_ColorInner * (1 - dist)), _Strength);
            }
            ENDCG
        }
    }
}
