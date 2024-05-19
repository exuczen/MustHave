using UnityEngine;

namespace MustHave.Utils
{
    public struct ShaderUtils
    {
        public static void PrintComputeShaderKeywords(ComputeShader shader)
        {
            Debug.Log($"{shader.name}: keywordSpace.keywordCount: {shader.keywordSpace.keywordCount}");
            int i = 0;
            foreach (var kw in shader.keywordSpace.keywords)
            {
                Debug.Log($"{shader.name}: keywordSpace.keywords[{i++}]: {kw}");
            }
            i = 0;
            foreach (var kw in shader.shaderKeywords)
            {
                Debug.Log($"{shader.name}: shaderKeywords[{i++}]: {kw}");
            }
            i = 0;
            foreach (var kw in shader.enabledKeywords)
            {
                Debug.Log($"{shader.name}: enabledKeywords[{i++}]: {kw}");
            }
        }
    }
}
