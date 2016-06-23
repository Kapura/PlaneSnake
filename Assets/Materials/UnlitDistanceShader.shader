Shader "Custom/UnlitDistanceShader" {
 
    Properties
    {
        _Color ("Color Tint", Color) = (1,1,1,1)
		_DarkColor ("Distant Color", Color) = (0,0,0,0)
        _MinAlphaDist ("Min Alpha Distance", float) = 0
        _MaxAlphaDist ("Max Alpha Distance", float) = 10
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
            };
 
            fixed4 _DarkColor = 0;
            fixed4 _Color;
            float _MinAlphaDist;
            float _MaxAlphaDist;
            float _MinBlackDist;
            float _MaxBlackDist;
   
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				float zPos = mul(_Object2World, v.vertex).z;
				float zDist = zPos - _WorldSpaceCameraPos.z;
				float blackPct = clamp((zDist - _MinBlackDist) / (_MaxBlackDist - _MinBlackDist), 0.0, 1.0);
                o.color.rgb = lerp(_Color.rgb, _DarkColor, (blackPct * (1 - blackPct)) + blackPct);
				float alphaPct = clamp((zDist - _MinAlphaDist) / (_MaxAlphaDist - _MinAlphaDist), 0.0, 1.0);
                o.color.a = _Color.a * alphaPct * alphaPct;
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
