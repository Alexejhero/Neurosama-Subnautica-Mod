Shader "Hidden/VanishSplash_shader"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("MainTex", 2D) = "gray" {}
        [NoScaleOffset]
        _Image ("image", 2D) = "gray" {}
        [HideInInspector]
        _ScreenPosition ("Position" , Vector) = (0,0,0,0)
        _Strength ("Strength", float) = 1
        _Scale ("Scale", float) = 2
        _Phase("Phase", float) = 0
        _DispStr("Displacement Strength", float) = 1
    }
    SubShader
    {
        Tags {"Queue" = "AlphaTest" "PreviewType" = "Plane" }
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
           

            sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
            sampler2D _Image;
            float4 _ScreenPosition;
            float _Strength;
            float _Scale;
            float _Phase;
            float4 _Image_TexelSize;
            float _DispStr;

            float2 FixUV(float2 iuv, float2 tex)
            {   
                float2 asp = float2(_ScreenParams.x / _ScreenParams.y, 1);
                float2 imAsp = float2(tex.x / tex.y, 1); 
                return iuv * ( asp / imAsp);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                _ScreenPosition.xy /= _ScreenParams.xy;

                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv));
                float depthDist = saturate((depth - _ScreenPosition.z) * 100) ;

                _Scale = (1 / _Scale) * _ScreenPosition.z;

                float2 straightUV = FixUV(i.uv * _Scale, _Image_TexelSize.zw) + 0.5;
                float2 straightPos = FixUV(_ScreenPosition.xy * _Scale, _Image_TexelSize.zw);

                float2 distFactor = (_ScreenPosition.xy - i.uv) * _DispStr * distance(i.uv, _ScreenPosition.xy) * _Phase;
                fixed4 col = tex2D(_MainTex, i.uv + distFactor);
                fixed4 image = tex2D(_Image, straightUV - straightPos);
                image.w = (distFactor.x + distFactor.y) ;
                return col + (image * image.w * depthDist * _Strength * _Phase);
            }
            ENDCG
        }
    }
}
