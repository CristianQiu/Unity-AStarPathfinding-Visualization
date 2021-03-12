using UnityEngine;
using UnityEngine.Rendering;

namespace Shared
{
    /// <summary>
    /// Generic utilities mainly for GameObjects, Transforms, and other Components from Unity.
    /// </summary>
    public static class Utils
    {
        #region Transform Methods

        /// <summary>
        /// Reset a Transform to local position Vector3.zero, local rotation Quaternion.identity and
        /// local scale to Vector3.zero.
        /// </summary>
        /// <param name="t"></param>
        public static void ResetTransformLocal(Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        #endregion

        #region Render Methods

        /// <summary>
        /// Get a simple colored material used for debugging purposes.
        /// </summary>
        /// <returns></returns>
        public static Material GetNewDebugMaterial()
        {
            // seems that Unity has a hidden material useful for this: https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnPostRender.html
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            Material debugMat = new Material(shader);

            // set blend modes to allow transparency
            debugMat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            debugMat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);

            // turn on back culling, turn on depth writes and depth test
            debugMat.SetInt("_Cull", (int)CullMode.Back);
            debugMat.SetInt("_ZWrite", 1);
            debugMat.SetInt("_ZTest", (int)CompareFunction.LessEqual);

            return debugMat;
        }

        #endregion

        #region Destroy Methods

        /// <summary>
        /// Choose whether to use MonoBehaviour's Destroy or DestroyImmediate depending on if the
        /// application is playing or not.
        /// </summary>
        /// <param name="o"></param>
        public static void DestroyProper(Object o)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                Object.Destroy(o);
            else
                Object.DestroyImmediate(o);
#else
            Object.DestroyImmediate(o);
#endif
        }

        #endregion
    }
}