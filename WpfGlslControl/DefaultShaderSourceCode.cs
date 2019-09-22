namespace WpfGlslControl
{
	internal static class DefaultShaderSourceCode
	{
		internal const string Quad = @"
			#version 130
			uniform vec2 iResolution;

			out vec2 uv; 
			out vec2 fragCoord;

			void main()
			{
				const vec2 vertices[4] = vec2[4](vec2(-1.0, -1.0),
					vec2(1.0, -1.0),
					vec2(1.0, 1.0),
					vec2(-1.0, 1.0));

				vec2 pos = vertices[gl_VertexID];
				uv = pos * 0.5 + 0.5;
				fragCoord = uv * iResolution;
				gl_Position = vec4(pos, 0.0, 1.0);
			}";

		internal const string Checker = @"
			#version 430 core
			in vec2 uv;

			out vec4 color;

			void main()
			{
				vec2 uv10 = floor(uv * 10.0f);
				bool black = 1.0 > mod(uv10.x + uv10.y, 2.0f);
				color = black ? vec4(0, 0, 0, 1) : vec4(1, 1, 0, 1);
			}";
	}
}
