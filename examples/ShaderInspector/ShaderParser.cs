using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Example
{
	public static class ShaderParser
	{
		public static string RemoveBlockComments(string shaderCode)
		{
			var pattern = $@"{Regex.Escape("/*")}(.|[\r\n])*?{Regex.Escape("*/")}"; //match everything till end block comment
			return Regex.Replace(shaderCode, pattern, string.Empty);
		}

		public static string RemoveLineComments(string shaderCode)
		{
			var pattern = "//.*"; //match everything till end of line
			return Regex.Replace(shaderCode, pattern, string.Empty);
		}

		public static string RemoveComments(string shaderCode)
		{
			return RemoveLineComments(RemoveBlockComments(shaderCode));
		}

		public static IEnumerable<(string type, string name)> ParseUniforms(string uncommentedShaderCode)
		{
			var pattern = @"uniform\s+([^\s]+)\s+([^\s]+)\s*;"; //matches uniform<spaces>type<spaces>name<spaces>; 
			foreach (Match match in Regex.Matches(uncommentedShaderCode, pattern))
			{
				var type = match.Groups[1].ToString();
				var name = match.Groups[2].ToString();
				yield return (type, name);
			}
		}
	}
}
