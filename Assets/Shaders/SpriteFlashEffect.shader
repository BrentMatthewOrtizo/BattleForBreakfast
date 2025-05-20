Shader "Custom/SpriteFlashEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlashColor ("Flash Color", Color) = (1,1,1,1) // RGBA: White and fully opaque
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }
        LOD 100

        Cull Off // Render both sides, common for sprites
        Lighting Off // No lighting needed for this effect
        ZWrite Off // Don't write to depth buffer for transparent objects
        Blend SrcAlpha OneMinusSrcAlpha // Standard alpha blending

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
                float4 color : COLOR; // To respect SpriteRenderer's original alpha if needed
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _FlashColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color; // Pass through vertex color (includes SpriteRenderer.color)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Combine the flash color with the texture's alpha and the SpriteRenderer's original alpha.
                // This makes sure only the visible parts of the sprite flash white,
                // and respects if the whole sprite was originally faded.
                fixed4 outputColor = _FlashColor;
                outputColor.a = _FlashColor.a * texColor.a * i.color.a;

                return outputColor;
            }
            ENDCG
        }
    }
}