Shader "Custom/SequenceFrameAnimation"
{
    Properties
    {
        _MainTex ("Sprite Sheet", 2D) = "white" {}
        _Anim ("Anim", Int) = 1
        _Rows ("Number of Rows", Int) = 8
        _Columns ("Number of Columns", Int) = 8
        _FrameWidth ("Frame Width", Float) = 0.125
        _FrameHeight ("Frame Height", Float) = 0.125
        _StartFrame ("Start Frame Index", Int) = 0
        _FrameCount ("Total Frames", Int) = 64
        _AnimationSpeed ("Animation Speed", Float) = 1.0
        _StartUV ("Start UV", Vector) = (0, 0, 0, 0)
        _TimeStart ("Time Start", Float) = 0.0 // 每个序列帧的开始时间偏移
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
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
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _Rows;
            int _Columns;
            float _FrameWidth;
            float _FrameHeight;
            int _StartFrame;
            int _FrameCount;
            float _AnimationSpeed;
            float2 _StartUV;
            int _Anim;
            float _TimeStart;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            { 
                // 计算当前帧索引
                float frameIndex = fmod((_Time.y - _TimeStart) * _AnimationSpeed, _FrameCount);
                frameIndex = floor(frameIndex); // 取整
                frameIndex += _StartFrame; // 起始帧偏移

                // 计算当前帧的 UV 偏移
                float2 frameOffset = float2(
                    _Anim == 1?fmod(frameIndex, _Columns) * _FrameWidth:0,
                    _Anim == 1?-((floor(frameIndex / _Columns)) * _FrameHeight):0
                );

                // 计算最终的 UV 坐标
                float2 finalUV = _StartUV + i.uv * float2(_FrameWidth, _FrameHeight) + frameOffset;

                // 采样纹理
                fixed4 col = tex2D(_MainTex, finalUV);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}