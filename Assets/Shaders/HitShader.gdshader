shader_type canvas_item;

uniform vec4 flashColor = vec4(1.0, 0.0, 0.0, 1.0);
uniform float mixAmount = 0.0;

void fragment() {
	vec4 baseColor = texture(TEXTURE, UV);
	
	vec4 mixed = mix(baseColor, flashColor, mixAmount);
	mixed.a = baseColor.a;
	COLOR = mixed;
}

