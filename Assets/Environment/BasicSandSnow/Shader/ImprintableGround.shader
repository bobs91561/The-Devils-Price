Shader "Custom/ImprintableGround"
{
	Properties
	{
		//Basic stuff
		_Color("Color", Color) = (1,1,1,1)
		_UpperTex("Upper Texture", 2D) = "white" {}
		_UpperNormal("Upper Normals", 2D) = "bump" {}
		_UnderTex("Under Texture", 2D) = "white" {}
		_UnderNormal("Under Normals", 2D) = "bump" {}

		//Custom effect
		_HeightTex("Height Map", 2D) = "white" {}
		_HeightMul("Height Multiplier", float) = -0.5
		_BlurHeight("Height Blur", float) = 0.01
		_NormalRadius("Normal Radius", float) = 0.01

		//Unity Standard Shader		
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		//Properties
		sampler2D _UnderTex;
		sampler2D _UpperTex;
		sampler2D _UpperNormal;
		sampler2D _UnderNormal;
		sampler2D _HeightTex;

		float _HeightMul;
		float _BlurHeight;
		float _NormalRadius;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		struct Input
		{
			float2 uv_UpperTex;
		};

		//------------------
		// Vertex Shader
		//------------------
		void vert(inout appdata_full v)
		{
			//Smooth the edges
			float VerticeHeight;
			float4 Median;
			for (int i = -1; i <= 1; ++i)
			{
				for (int j = -1; j <= 1; ++j)
				{
					// ".r" because RenderTextures in Depth mode only use the red channel anyway
					VerticeHeight+= tex2Dlod(_HeightTex, float4(v.texcoord.x + i * _BlurHeight, v.texcoord.y + j * _BlurHeight, 0 , 0)).r;
				}
			}
			VerticeHeight = VerticeHeight/ 9.0;

			//Dynamically change the vertex's position
			v.vertex.y += VerticeHeight * _HeightMul;
		}
		
		//------------------
		//Pixel shader
		//------------------
		void surf(Input InPut, inout SurfaceOutputStandard OutPut)
		{
			//Extract stuff from textures
			float2 Coord = float2(InPut.uv_UpperTex.x, InPut.uv_UpperTex.y);
			fixed4 UnderColor = tex2D(_UnderTex, Coord);
			fixed4 UpperColor = tex2D(_UpperTex, Coord);
			float PixelHeight = tex2D(_HeightTex, Coord).r;
			fixed3 NormalUpper = UnpackNormal(tex2D(_UpperNormal, Coord));
			fixed3 NormalUnder = UnpackNormal(tex2D(_UnderNormal, Coord));
			
			//Calculate "main" normal from the neighbours' height
			fixed3 Normal;
			float A = tex2D(_HeightTex, float2(InPut.uv_UpperTex.x + _NormalRadius, InPut.uv_UpperTex.y)).r;
			float C = tex2D(_HeightTex, float2(InPut.uv_UpperTex.x - _NormalRadius, InPut.uv_UpperTex.y)).r;
			float B = tex2D(_HeightTex, float2(InPut.uv_UpperTex.x, InPut.uv_UpperTex.y + _NormalRadius)).r;
			float D = tex2D(_HeightTex, float2(InPut.uv_UpperTex.x, InPut.uv_UpperTex.y - _NormalRadius)).r;
			Normal.x = A - C;
			Normal.y = B - D;
			Normal.z = 0;
			normalize(Normal);

			//Combine Under and Upper based on how hight the pixel is
			fixed4 CombinedColor = lerp(UpperColor, UnderColor, PixelHeight) *_Color;
			fixed3 CombinedNormal = lerp(NormalUpper, NormalUnder, PixelHeight) + Normal;
			normalize(CombinedNormal);
			OutPut.Albedo = CombinedColor.rgb;
			OutPut.Alpha = CombinedColor.a;
			OutPut.Normal = CombinedNormal;
			
			//Standard
			OutPut.Metallic = _Metallic;
			OutPut.Smoothness = _Glossiness;			
		}
	ENDCG
	}

	//In case this shader can't be displayed
	FallBack "Diffuse"
}
