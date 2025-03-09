Shader "Unlit/TextureStepper"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _StartUV ("Start UV", Vector) = (0, 0, 0, 0)
        _StepSize ("Step Size", Vector) = (0.1, 0.1, 0, 0)
        _StepInterval ("Step Interval", Float) = 1.0
        _XCount ("XCount", int) = 5 
        _FirstXLen ("FirstXLen", int) = 2 // 第一行的长度
        
        _TextureRange ("Texture Range", Vector) = (1, 1, 0, 0) // 纹理范围（宽、高）
        _FlipX ("Flip X", float) = 0 // 0: 不翻转，1: 水平翻转

    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _StartUV;
            float2 _StepSize;
            float _StepInterval;
            int _XCount;
            int _FirstXLen;
            float2 _TextureRange; // 纹理范围（宽、高）
            float _FlipX;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                int steps = fmod(_Time.y / _StepInterval, _XCount);
                // 计算当前 UV 偏移
                float2 uvOffset = _StartUV ;
                if(_XCount > 1 && steps >= _FirstXLen)
                {
                    uvOffset.x = 0;
                    uvOffset.y -= _StepSize.y;
                    steps.x -= _FirstXLen;
                }
                uvOffset.x += steps * _StepSize.x;

                i.uv.x = _FlipX == 1 ? 1 - i.uv.x : i.uv.x;
                float2 uv = uvOffset + i.uv * _StepSize ;

                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}
