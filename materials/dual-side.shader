Shader "AngryLabs/props/drunkopoly/dual-side"
{
  Properties
  {
    _MainTex ("Front Texture", 2D) = "white" {}
    _MainTexBack  ("Back Texture",  2D) = "white" {}
    _ColorFront   ("Front Tint",    Color) = (1,1,1,1)
    _ColorBack    ("Back Tint",     Color) = (1,1,1,1)
    _AlphaCutoff  ("Alpha Cutoff",  Range(0,1)) = 0.5
  }

  SubShader
  {
    Tags {
      "RenderType"="Transparent"
      "Queue"="Transparent"
      "IgnoreProjector"="True"
      "CanUseSpriteAtlas"="True"
      "PreviewType"="Plane"
    }

    Cull Off
    Lighting Off
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 4.0

      #include "UnityCG.cginc"

      struct appdata_t
      {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
        float4 color : COLOR; // sprite tint
      };

      struct v2f
      {
        float4 pos : SV_POSITION;
        float2 uv  : TEXCOORD0;
        float4 color : COLOR;
      };

      sampler2D _MainTex;
      sampler2D _MainTexBack;
      float4    _MainTex_ST;
      float4    _MainTexBack_ST;
      fixed4    _ColorFront;
      fixed4    _ColorBack;
      float     _AlphaCutoff;

      v2f vert(appdata_t v)
      {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv  = v.texcoord;
        o.color = v.color;
        return o;
      }

      fixed4 frag(v2f i, bool isFront : SV_IsFrontFace) : SV_Target
      {
        float2 uvF = TRANSFORM_TEX(i.uv, _MainTex);
        float2 uvB = TRANSFORM_TEX(i.uv, _MainTexBack);

        uvB.x = 1.0f - uvB.x;

        fixed4 texF = tex2D(_MainTex, uvF) * _ColorFront;
        fixed4 texB = tex2D(_MainTexBack,  uvB) * _ColorBack;

        // Combine with sprite vertex color (tint)
        fixed4 col = (isFront ? texF : texB) * i.color;

        // alpha clip
        clip(col.a - _AlphaCutoff);

        return col;
      }
      ENDCG
    }
  }

  FallBack "Sprites/Default"
}