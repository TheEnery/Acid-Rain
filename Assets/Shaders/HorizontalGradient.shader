Shader "Unlit/HorizontalGradient"
{
    Properties
    {
        _LeftColor("Left Gradient Color: ", Color) = (1, 1, 1, 1)
        _RightColor("Right Gradient Color: ", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

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
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            fixed4 _LeftColor;
            fixed4 _RightColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = lerp(_LeftColor, _RightColor, v.uv.x);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }

            ENDCG
        }
    }
}
