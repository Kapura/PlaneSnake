Shader "Custom/UnlitDistanceShader" {
 
    Properties
    {
		_MainTex ("Texture", 2D) = "white" { }
        _Color ("Color Tint", Color) = (1,1,1,1)
		_DarkColor ("Distant Color", Color) = (0,0,0,0)
        _MinAlphaDist ("Min Alpha Distance", float) = 0
        _MaxAlphaDist ("Max Alpha Distance", float) = 10
		_AlphaPower ("Alpha Fade Strength", range(1.0, 5.0)) = 1.0
        _MinBlackDist ("Min Black Distance", float) = 10
        _MaxBlackDist ("Max Black Distance", float) = 20
    }
 
    SubShader {
        Tags { "Queue" = "Transparent" }
 
        Pass {
            ZWrite Off
 
            Blend SrcAlpha OneMinusSrcAlpha
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
   
            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
            };
 
            fixed4 _DarkColor = 0;
            fixed4 _Color;
			sampler2D _MainTex;
            float _MinAlphaDist;
            float _MaxAlphaDist;
			float _AlphaPower;
            float _MinBlackDist;
            float _MaxBlackDist;
   
			float4 _MainTex_ST;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				float zPos = mul(_Object2World, v.vertex).z;
				float zDist = zPos - _WorldSpaceCameraPos.z;
				float blackPct = clamp((zDist - _MinBlackDist) / (_MaxBlackDist - _MinBlackDist), 0.0, 1.0);
                o.color.rgb = lerp(_Color.rgb, _DarkColor, blackPct);
				float alphaPct = clamp((zDist - _MinAlphaDist) / (_MaxAlphaDist - _MinAlphaDist), 0.0, 1.0);
                o.color.a = _Color.a * pow(alphaPct, _AlphaPower);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 texcol = tex2D (_MainTex, i.uv);
                return texcol * i.color;
            }
            ENDCG
        }
    }
}
