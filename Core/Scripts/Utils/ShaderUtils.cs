using UnityEngine;

namespace MustHave
{
    public struct ShaderUtils
    {
        public static int GetThreadGroupsCount(uint numthreads, int size, bool clamp = true)
        {
            return GetThreadGroupsCount((int)numthreads, size, clamp);
        }

        public static int GetThreadGroupsCount(int numthreads, int size, bool clamp = true)
        {
            if (numthreads == 0)
            {
                return 0;
            }
            int n = numthreads;
            int count = (size + n - 1) / n;
            return clamp ? Mathf.Clamp(count, 1, 65535) : count;
        }

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
