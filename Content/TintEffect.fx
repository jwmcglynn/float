//  Change tint.

float3 TintColor = float3(1, 1, 1);
sampler TextureSampler : register(s0);


float4 main(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0 {
	// Look up the texture color.
	float4 tex = tex2D(TextureSampler, texCoord) * color;
	
	// Tint.
	tex.rgb *= TintColor;
	return tex;
}


technique TintEffect {
	pass Pass1 {
		PixelShader = compile ps_2_0 main();
	}
}
