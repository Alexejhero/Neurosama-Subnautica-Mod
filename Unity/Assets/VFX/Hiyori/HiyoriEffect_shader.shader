Shader"SchizoVFX/HiyoriEffect"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("Texture", 2D) = "white" {}
        _NoizeTex ("Noize Texture", 2D) = "white" {}
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
            sampler2D _NoizeTex;
            float4 _ScreenPosition;


            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 noiseCol = tex2D(_NoizeTex, float2(_SinTime.x, _CosTime.w));
                noiseCol.rgb * clamp(distance(i.uv, _ScreenPosition.xy), 0, 1) ;
                col.rgb += 1 - noiseCol;
                return col;
            }
            ENDCG
        }
    }
}
