Shader "AngryLabs/Props/Drunkopoly/ImageMixer"
{
  Properties
  {
    _MainTex ("Sprite Texture", 2D) = "white" {}
    _BgTex   ("Background Texture", 2D) = "white" {}
    _Color   ("Tint", Color) = (1,1,1,1)
  }

  SubShader
  {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
    LOD 100

    Cull Back               // backface culling enabled
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct appdata_t
      {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
        float4 color : COLOR;
      };

      struct v2f
      {
        float4 pos : SV_POSITION;
        float2 uv  : TEXCOORD0;
        float4 color : COLOR;
      };

      sampler2D _MainTex;
      float4 _MainTex_ST;
      sampler2D _BgTex;
      float4 _BgTex_ST;
      float4 _Color;

      v2f vert (appdata_t v)
      {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
        o.color = v.color * _Color;
        return o;
      }

      fixed4 frag (v2f i) : SV_Target
      {
        fixed4 src = tex2D(_MainTex, i.uv) * i.color;
        if( length(src.rgb) < 0.1f)
            src.a = 0.0f;

        float2 bgUV = TRANSFORM_TEX(i.uv, _BgTex);
        fixed4 bg = tex2D(_BgTex, bgUV);

        fixed4 outCol;
        outCol.rgb = src.rgb * bg.rgb;
        outCol.a = src.a * bg.a;

        // Premultiply alpha for correct blending
        outCol.rgb *= outCol.a;

        return outCol;
      }
      ENDCG
    }
  }

  FallBack "Sprites/Default"
}