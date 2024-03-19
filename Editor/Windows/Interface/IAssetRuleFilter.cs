using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MonoHook;
using UnityEditor;

namespace AIO.UEditor
{
    public interface IAssetRuleFilter
    {
        /// <summary>
        /// 显示的过滤器名称
        /// </summary>
        string DisplayFilterName { get; }

        /// <summary>
        /// 验证资源是否符合规则
        /// </summary>
        bool IsCollectAsset(AssetRuleData Data);
    }

    public static class Test
    {
        [MenuItem("AIO/ProfilerSpace/Test")]
        public static void Test11()
        {
            var methodToHook1 = typeof(Test).GetMethod(nameof(Test1), BindingFlags.NonPublic | BindingFlags.Static);
            var methodToHook2 = typeof(Test).GetMethod(nameof(Test2), BindingFlags.NonPublic | BindingFlags.Static);
            var replaceMethod = GetMethodBody(methodToHook1);
            //
            // replaceMethod.Invoke(null, null);
            // ReplaceMethodIL(methodToHook, replaceMethod);
            // Console.WriteLine("-----------------");
            Test1();
            var temp = SwapMethodBodies(methodToHook1, methodToHook2);
            Test1();
        }

        private static void Test1()
        {      
            Console.WriteLine("111111111111111");
        }

        private static void Test2()
        {
            Console.WriteLine("2222222222222222");
        }

        public struct MethodReplacementState : IDisposable
        {
            internal IntPtr Location;
            internal IntPtr OriginalValue;

            public void Dispose()
            {
                this.Restore();
            }

            public unsafe void Restore()
            {
#if DEBUG
                *(int*)Location = (int)OriginalValue;
#else
                *(IntPtr*)Location = OriginalValue;
#endif
            }
        }

        /// <summary>
        /// Swaps the function pointers for a and b, effectively swapping the method bodies.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// a and b must have same signature
        /// </exception>
        /// <param name="methodToReplace">Method to swap</param>
        /// <param name="methodToInject">Method to swap</param>
        public static unsafe MethodReplacementState SwapMethodBodies(MethodInfo methodToReplace,
            MethodInfo methodToInject)
        {
//#if DEBUG
            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);
//#endif
            MethodReplacementState state;

            var tar = methodToReplace.MethodHandle.Value;
            if (!methodToReplace.IsVirtual)
                tar += 8;
            else
            {
                var index = (int)(((*(long*)tar) >> 32) & 0xFF);
                var classStart =
                    *(IntPtr*)(methodToReplace.DeclaringType.TypeHandle.Value + (IntPtr.Size == 4 ? 40 : 64));
                tar = classStart + IntPtr.Size * index;
            }

            var inj = methodToInject.MethodHandle.Value + 8;
#if DEBUG
            tar = *(IntPtr*)tar + 1;
            inj = *(IntPtr*)inj + 1;
            state.Location = tar;
            state.OriginalValue = new IntPtr(*(int*)tar);

            *(int*)tar = *(int*)inj + (int)(long)inj - (int)(long)tar;
            return state;

#else
            state.Location = tar;
            state.OriginalValue = *(IntPtr*)tar;
            * (IntPtr*)tar = *(IntPtr*)inj;
            return state;
#endif
        }

        private static bool HasSameSignature(MethodInfo a, MethodInfo b)
        {
            bool sameParams = !a.GetParameters().Any(x => !b.GetParameters().Any(y => x == y));
            bool sameReturnType = a.ReturnType == b.ReturnType;
            return sameParams && sameReturnType;
        }

        private static MethodInfo GetMethodBody(MethodInfo method)
        {
            var dynamicMethod = new DynamicMethod("NewTest1", typeof(void), null, typeof(Test).Module, true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // 输出 "0"
            ilGenerator.Emit(OpCodes.Ldstr, "0");
            ilGenerator.EmitCall(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }),
                null);

            // 调用原始方法体
            foreach (var param in method.GetParameters())
                ilGenerator.Emit(OpCodes.Ldarg, param.Position + 1);
            ilGenerator.Emit(OpCodes.Call, method);

            // 输出 "2"
            ilGenerator.Emit(OpCodes.Ldstr, "2");
            ilGenerator.EmitCall(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }),
                null);

            // 返回
            ilGenerator.Emit(OpCodes.Ret);

            return dynamicMethod;
        }

        /// <summary>
        /// 替换方法体
        /// </summary>
        /// <param name="replace">替换的目标函数</param>
        /// <param name="inject">注入函数</param>
        private static void ReplaceMethodIL(MethodInfo replace, MethodInfo inject)
        {
            unsafe
            {
                RuntimeHelpers.PrepareMethod(replace.MethodHandle);
                RuntimeHelpers.PrepareMethod(inject.MethodHandle);
                if (IntPtr.Size == 4)
                {
                    int* inj = (int*)inject.MethodHandle.Value.ToPointer() + 2;
                    int* tar = (int*)replace.MethodHandle.Value.ToPointer() + 2;
#if DEBUG
                    Console.WriteLine("Version x86 Debug");
                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;
                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);
                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                               Console.WriteLine("Version x86 Relaese");
                               *tar = *inj;
#endif
                }
                else
                {
                    long* inj = (long*)inject.MethodHandle.Value.ToPointer() + 1;
                    long* tar = (long*)replace.MethodHandle.Value.ToPointer() + 1;
#if DEBUG
                    Console.WriteLine("Version x64 Debug");
                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;
                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);
                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
#else
                               Console.WriteLine("Version x64 Relaese");
                               *tar = *inj;
#endif
                }
            }
        }
    }
}