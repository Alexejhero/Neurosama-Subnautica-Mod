Shader "Hidden/ImageOverlay_shader"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset]
        _Image("Image", 2D) = "gray" {}
        _Position("Position", vector) = (0.5,0.5,0,0)
        _Scale("Scale" , Range(0, 10)) = 1
        _Strength("Opacity", Range(0, 1)) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass // ADD
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
            float _Scale;
            float _Strength;
            float2 _Position;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 fixedUV = FixUV(i.uv * (1 / _Scale)) + 0.5;
                float2 fixedPos = FixUV(_Position * (1 / _Scale));

                float4 image = tex2D(_Image, fixedUV - fixedPos);

                fixed4 col = tex2D(_MainTex, i.uv);

                return lerp(col, col + image, image.w * _Strength);
            }
            ENDCG
        }

        Pass // NORMAL
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
            float _Scale;
            float _Strength;
            float2 _Position;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 fixedUV = FixUV(i.uv * (1 / _Scale)) + 0.5;
                float2 fixedPos = FixUV(_Position * (1 / _Scale));

                float4 image = tex2D(_Image, fixedUV - fixedPos);

                fixed4 col = tex2D(_MainTex, i.uv);

                return lerp(col, image, image.w * _Strength);
            }
            ENDCG
        }

        Pass // MULTIPLY
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
            float _Scale;
            float _Strength;
            float2 _Position;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 fixedUV = FixUV(i.uv * (1 / _Scale)) + 0.5;
                float2 fixedPos = FixUV(_Position * (1 / _Scale));

                float4 image = tex2D(_Image, fixedUV - fixedPos);

                fixed4 col = tex2D(_MainTex, i.uv);

                return lerp(col, col * image, image.w * _Strength);
            }
            ENDCG
        }

        Pass // SCREEN
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
            float _Scale;
            float _Strength;
            float2 _Position;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 fixedUV = FixUV(i.uv * (1 / _Scale)) + 0.5;
                float2 fixedPos = FixUV(_Position * (1 / _Scale));

                float4 image = tex2D(_Image, fixedUV - fixedPos);

                fixed4 col = tex2D(_MainTex, i.uv);

                return lerp(col, 1 - ((1 - col) * (1 - image)), image.w * _Strength);
            }
            ENDCG
        }

        Pass // SUBSTRACT
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
            float _Scale;
            float _Strength;
            float2 _Position;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 fixedUV = FixUV(i.uv * (1 / _Scale)) + 0.5;
                float2 fixedPos = FixUV(_Position * (1 / _Scale));

                float4 image = tex2D(_Image, fixedUV - fixedPos);

                fixed4 col = tex2D(_MainTex, i.uv);

                return half4(lerp(col.rgb, col.rgb - image.rgb, image.w * _Strength), col.a);
            }
            ENDCG
        }
    }
}
