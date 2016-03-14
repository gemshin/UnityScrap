Shader "BGShader/Transparent/Cutout/TwoSide" {
	Properties 
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_GlowTex("GlowTex", 2D) = "black" {}
		_IntensityGlow("IntensityGlow", Range(0.0,5.0)) = 1.5
		_GlowColor("GlowColor", Color) = (0.5,0.5,0.5,1)
		_SpecTex("SpecTex", 2D) = "black" {}
		_Spec_Shininess("Spec_Shininess", Range(0.3,0.0)) = 0.2
		_SpecColors("SpecColors", Color) = (0.5,0.5,0.5,1)
		_Cube("Reflection Cubemap", Cube) = "black" {}
		_CubeBright("CubeBright", Float) = 0
		_IntensityCube("IntensityCube", Range(0.0,2.0)) = 0
		_Cutoff ("Alpha cutoff",Range(0.0,1.0)) = 0.5
		_GlowBlinkSpeed("GlowBlinkSpeed", Float) = 2
		_GlowBlinkAlpha("GlowBlinkAlpha", Float) = 1
		_ScrollDirection("ScrollDirection", Vector) = (0,0,0,0)
		_ScrollRange("ScrollRange", Vector) = (0,0,0,0)
		_ScrollRange2("ScrollRange2", Vector) = (0,0,0,0)
		
	}
	
	SubShader 
	{
		Tags
		{
			"Queue"="Transparent+1"
			"IgnoreProjector"="True"
			"RenderType"="TransparentCutout"

		}
		LOD 200
		Cull Off
		ZWrite On
		ZTest LEqual
		ColorMask RGBA
		

		CGPROGRAM
		#pragma surface surf BlinnPhongEditor  vertex:vert alphatest:_Cutoff
		#pragma target 3.0

		sampler2D _MainTex;
		float4 _Color;
		sampler2D _GlowTex;
		float4 _GlowColor;
		float _IntensityGlow;
		float _GlowBlinkSpeed;
		float _GlowBlinkAlpha;
		float4 _ScrollDirection;
		float4 _ScrollRange;
		float4 _ScrollRange2;
		samplerCUBE _Cube;
		float _CubeBright;
		float _IntensityCube;
		sampler2D _SpecTex;
		float4 _SpecColors;
		float _Spec_Shininess;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
				half3 spec = light.a * s.Gloss;
				half4 c;
				c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
				c.a = s.Alpha;
				return c;
			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float2 uv_MainTex;
				float3 simpleWorldRefl;
				float2 uv_GlowAlphaTex;
				float2 uv_SpecTex;
				float2 uv_GlowTex;
			};

			void vert (inout appdata_full v, out Input o) {
					UNITY_INITIALIZE_OUTPUT(Input,o)
					float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
					float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
					float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
					float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);

					o.simpleWorldRefl = -reflect( normalize(WorldSpaceViewDir(v.vertex)), normalize(mul((float3x3)_Object2World, SCALED_NORMAL)));

			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				
			
			float4 Tex2D0=tex2D(_MainTex,(IN.uv_MainTex.xyxy).xy);
			float4 TexCUBE0=texCUBE(_Cube,float4( IN.simpleWorldRefl.x, IN.simpleWorldRefl.y,IN.simpleWorldRefl.z,1.0 ));
			float4 Multiply5=TexCUBE0 * _IntensityCube.xxxx;
			float4 Add2=Multiply5 + _CubeBright.xxxx;
			float4 Add3 = (IN.uv_SpecTex.xyxy) + _ScrollRange2;
			float4 Tex2D2=tex2D(_SpecTex,Add3.xy);
			float4 Multiply10=Add2 * Tex2D2;
			float4 Add1=Tex2D0 + Multiply10;
			float4 Multiply0=Add1 * _Color;
			float4 Add0=(IN.uv_GlowTex.xyxy) + _ScrollRange;
			float4 Tex2D1=tex2D(_GlowTex,Add0.xy);
			float4 Multiply4=_GlowColor * _GlowBlinkAlpha.xxxx;
			float4 Multiply3=_IntensityGlow.xxxx * Multiply4;
			float4 Multiply7=Tex2D1 * Multiply3;
			float4 Multiply9=Tex2D2 * _SpecColors;
			float4 Master0_1_NoInput = float4(0,0,1,1);
			float4 Master0_7_NoInput = float4(0,0,0,0);
			float4 Master0_6_NoInput = float4(1,1,1,1);
			o.Albedo = Multiply0;
			o.Emission = Multiply7;
			o.Specular = _Spec_Shininess.xxxx;
			o.Gloss = Multiply9;
			o.Alpha = Tex2D0.aaaa;

			o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Transparent/Cutout/VertexLit"
	//CustomEditor "CustomMaterialInspector"
}
