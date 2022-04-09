Shader "Mondi/UnlitGridShader"
{
	// https://www.youtube.com/watch?v=EBrAdahFtuo
	// https://gamedev.stackexchange.com/questions/156902/how-can-i-create-an-outline-shader-for-a-plane
	// https://assetstore.unity.com/packages/tools/simple-grid-shader-119988

	// https://github.com/nanclin/Rounded-Corners/tree/master/Assets

    Properties
    {
		[HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		[Toggle] _UseOutline ("Use Outline", Int) = 0
		[Toggle] _UseGradient ("Use Gradient", Int) = 0
		_Speed ("Speed", Int) = 2
		_Alpha ("Alpha", Range(0, 1)) = 1
		_Thickness ("Thickness", Range(0, 0.5)) = 0.2
		_ColorA ("Color A", Color) = (1, 1, 1, 1)
		_ColorB ("Color B", Color) = (0, 1, 0, 1)
    }
    SubShader
    {
        Tags 
		{ 
			"RenderType"="Opaque"
			"PreviewType"="Plane"
		}

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
				float2 _uv: TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 _uv: TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			int _UseOutline;
			int _UseGradient;

			int _Speed;
			float _Alpha;
			float _Thickness;

			fixed4 _ColorA;
			fixed4 _ColorB;

			//float4 _Points[1];

			fixed4 drawGrid(float2 uv)
			{
				fixed3 black = fixed3(0, 0, 0);
				fixed3 white = fixed3(1, 1, 1);

				fixed4 col = fixed4(black, _Alpha);
				col = 0;

				float2 gv = frac(uv) - 0.5;

				//col.rg = gv;

				float value = 0.5 - _Thickness;

				if (gv.x > value || gv.y > value) col = fixed4(white, _Alpha);

				col *= _Alpha;

				return col;
			}
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o._uv = TRANSFORM_TEX(v._uv, _MainTex);
                return o;
            }

			float rect(float thickess, float2 uv)
			{
				float2 bl = step(thickess, uv);
				float2 tr = step(thickess, 1 - uv);
				return bl.x * bl.y * tr.x * tr.y;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				//float2 uv = i.uv * float2(3, 3);
				//return drawGrid(uv);

				// getting default uv
				float2 uv = i._uv;

				// borders are black, center is white
				float value = rect(_Thickness, uv);

				// dont draw black color (its needed if no texture)
				//if (value == 0) discard;

				// outline controlled by alpha
				fixed4 outline = (1 - value) * _Alpha;

				// getting color from cropped uv
				fixed4 tex = tex2D(_MainTex, i.uv);

				// texture + outline
				fixed4 color = tex + outline;

				bool drawOutline = _UseOutline > 0;
				if (!drawOutline)
				{
					return tex;
				}

				bool drawGradient = _UseGradient > 0;
				if (drawGradient)
				{
					// gradient calculations
					float sinValue = sin(_Time.y * _Speed);
					float percent01 = (sinValue + 1) / 2;

					fixed4 gradientColor = lerp(_ColorA, _ColorB, percent01);

					// texure + gradient outline
					color = tex * value + outline * gradientColor;
				}

				return color;
            }
            ENDCG
        }
    }
}
