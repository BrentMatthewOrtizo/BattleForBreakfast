Shader "Custom/LightMaskShader"
{
    Properties
    {
        _MainTex ("Texture (not used, but required)", 2D) = "white" {} // Standard, but we won't use it for color
        _PlayerWorldPos ("Player World Position", Vector) = (0,0,0,0)
        _LightRadius ("Light Radius", Float) = 5.0
        _DarknessColor ("Darkness Color", Color) = (0,0,0,1) // RGBA, black and opaque by default
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        ZWrite Off
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD0; // To pass world position of the vertex
            };

            float4 _PlayerWorldPos;
            float _LightRadius;
            fixed4 _DarknessColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex); // Calculate world position of the vertex
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate distance between the current fragment's world position and the player's world position
                // We only care about XY for 2D distance
                float dist = distance(i.worldPos.xy, _PlayerWorldPos.xy);

                // Calculate alpha for the darkness overlay
                // Alpha = 0 (transparent) at player, Alpha = 1 (opaque) at radius edge
                float lightAlpha = saturate(dist / _LightRadius);

                // The final color of the overlay is the darkness color with the calculated alpha
                fixed4 finalColor = _DarknessColor;
                finalColor.a = finalColor.a * lightAlpha; // Modulate darkness base alpha with light falloff

                return finalColor;
            }
            ENDCG
        }
    }
}