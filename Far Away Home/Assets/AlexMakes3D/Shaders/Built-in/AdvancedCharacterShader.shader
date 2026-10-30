// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AdvancedShader"
{
	Properties
	{
		_Color1("Color 1", Color) = (0,0,0,0)
		_Color2("Color 2", Color) = (0,0,0,0)
		_Color3("Color 3", Color) = (0,0,0,0)
		_Color4("Color 4", Color) = (0,0,0,0)
		_Color5("Color 5", Color) = (0,0,0,0)
		_Color6("Color 6", Color) = (0,0,0,0)
		[NoScaleOffset]_BaseColorMap("Base Color Map", 2D) = "white" {}
		[NoScaleOffset]_NormalMap("NormalMap", 2D) = "bump" {}
		[NoScaleOffset]_MetallicAOSmoothness("MetallicAOSmoothness", 2D) = "white" {}
		[NoScaleOffset]_Mask_1("Mask_1", 2D) = "white" {}
		[NoScaleOffset]_Mask_2("Mask_2", 2D) = "white" {}
		[Toggle]_Emission("Emission", Float) = 1
		[NoScaleOffset]_EmissionMap("Emission Map", 2D) = "bump" {}
		[HDR]_Emissioncolor("Emission color", Color) = (0,0,0,0)
		_Dirtintensity("Dirt intensity", Range( 0.1 , 1)) = 0.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform sampler2D _Mask_1;
		uniform float4 _Color3;
		uniform float4 _Color4;
		uniform sampler2D _BaseColorMap;
		uniform sampler2D _Mask_2;
		uniform float4 _Color5;
		uniform float _Dirtintensity;
		uniform float4 _Color6;
		uniform float _Emission;
		uniform float4 _Emissioncolor;
		uniform sampler2D _EmissionMap;
		uniform sampler2D _MetallicAOSmoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap8 = i.uv_texcoord;
			o.Normal = UnpackNormal( tex2D( _NormalMap, uv_NormalMap8 ) );
			float2 uv_Mask_12 = i.uv_texcoord;
			float4 tex2DNode2 = tex2D( _Mask_1, uv_Mask_12 );
			float4 lerpResult13 = lerp( _Color1 , _Color2 , tex2DNode2.g);
			float4 lerpResult18 = lerp( lerpResult13 , _Color3 , tex2DNode2.b);
			float4 lerpResult20 = lerp( lerpResult18 , _Color4 , tex2DNode2.r);
			float2 uv_BaseColorMap21 = i.uv_texcoord;
			float2 uv_Mask_223 = i.uv_texcoord;
			float4 tex2DNode23 = tex2D( _Mask_2, uv_Mask_223 );
			float4 lerpResult22 = lerp( lerpResult20 , tex2D( _BaseColorMap, uv_BaseColorMap21 ) , tex2DNode23.r);
			float4 lerpResult25 = lerp( lerpResult22 , _Color5 , ( _Dirtintensity * tex2DNode23.g ));
			float4 lerpResult27 = lerp( lerpResult25 , _Color6 , tex2DNode23.b);
			o.Albedo = lerpResult27.rgb;
			float2 uv_EmissionMap31 = i.uv_texcoord;
			o.Emission = (( _Emission )?( ( _Emissioncolor * tex2D( _EmissionMap, uv_EmissionMap31 ) ) ):( float4( 0,0,0,0 ) )).rgb;
			float2 uv_MetallicAOSmoothness10 = i.uv_texcoord;
			float4 tex2DNode10 = tex2D( _MetallicAOSmoothness, uv_MetallicAOSmoothness10 );
			o.Metallic = tex2DNode10.r;
			o.Smoothness = tex2DNode10.a;
			o.Occlusion = tex2DNode10.g;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.LerpOp;13;112.2635,326.0827;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;18;287.1411,113.6646;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;20;504.5787,-40.67916;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;22;657.7001,-293.0474;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;15;-235.2006,586.9333;Inherit;False;Property;_Color1;Color 1;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.7568628,0.7607843,0.7647059,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;-238.66,394.9478;Inherit;False;Property;_Color2;Color 2;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.7921569,0.4784314,0.07058824,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-245.2288,118.8309;Inherit;False;Property;_Color3;Color 3;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.6415094,0.6415094,0.6415094,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;16;-242.3955,-79.88073;Inherit;False;Property;_Color4;Color 4;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.8117647,0.5568628,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;25;875.062,-457.8296;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;24;-234.9782,-794.4875;Inherit;False;Property;_Color5;Color 5;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.2078431,0.2078431,0.2078431,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;26;-249.2439,-1046.569;Inherit;False;Property;_Color6;Color 6;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1466.18,-281.3695;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;AdvancedShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.ToggleSwitchNode;32;1145.158,-359.9833;Inherit;False;Property;_Emission;Emission;11;0;Create;True;0;0;0;False;0;False;1;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;1159.448,-211.0921;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;27;1060.648,-769.7968;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;674.688,-566.821;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;837.0872,370.7884;Inherit;True;Property;_NormalMap;NormalMap;7;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;beeec318ef073c14685df082f9cec175;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;21;-259.6025,-312.176;Inherit;True;Property;_BaseColorMap;Base Color Map;6;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;6120744f4ba3d4f4aaca3ad9bbb73f8b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;-257.0289,-527.3178;Inherit;True;Property;_Mask_2;Mask_2;10;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;1f943dac066bda3409132000ae309b34;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-740.9868,307.782;Inherit;True;Property;_Mask_1;Mask_1;9;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;5fecf0d7e37e1024f8a48d49ceb033de;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;773.2108,66.72939;Inherit;True;Property;_MetallicAOSmoothness;MetallicAOSmoothness;8;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;96a48535f008d954e8ca57cff8673b72;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;31;734.6409,-168.0564;Inherit;True;Property;_EmissionMap;Emission Map;12;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;29;405.1909,-672.1638;Inherit;False;Property;_Dirtintensity;Dirt intensity;14;0;Create;True;0;0;0;False;0;False;0.1;0.2;0.1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;35;909.1646,-327.2358;Inherit;False;Property;_Emissioncolor;Emission color;13;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;13;0;15;0
WireConnection;13;1;14;0
WireConnection;13;2;2;2
WireConnection;18;0;13;0
WireConnection;18;1;17;0
WireConnection;18;2;2;3
WireConnection;20;0;18;0
WireConnection;20;1;16;0
WireConnection;20;2;2;1
WireConnection;22;0;20;0
WireConnection;22;1;21;0
WireConnection;22;2;23;1
WireConnection;25;0;22;0
WireConnection;25;1;24;0
WireConnection;25;2;30;0
WireConnection;0;0;27;0
WireConnection;0;1;8;0
WireConnection;0;2;32;0
WireConnection;0;3;10;1
WireConnection;0;4;10;4
WireConnection;0;5;10;2
WireConnection;32;1;33;0
WireConnection;33;0;35;0
WireConnection;33;1;31;0
WireConnection;27;0;25;0
WireConnection;27;1;26;0
WireConnection;27;2;23;3
WireConnection;30;0;29;0
WireConnection;30;1;23;2
ASEEND*/
//CHKSM=C92F746894CB46290398CAA3908D251665064D45