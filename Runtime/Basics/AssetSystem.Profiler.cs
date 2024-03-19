using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine.Profiling;

namespace AIO
{
    [AttributeUsage(
        AttributeTargets.Method |
        AttributeTargets.Constructor |
        AttributeTargets.Property,
        Inherited = false)]
    [Conditional("UNITY_EDITOR"), DebuggerNonUserCode]
    public class ProfilerSpace : Attribute
    {
        public string Label { get; private set; }

        public ProfilerSpace()
        {
            Label = string.Empty;
        }

        public ProfilerSpace(string label1, string label2)
        {
            Label = string.Intern(string.Concat(label1, ".", label2));
        }

        public ProfilerSpace(string label1, string label2, string label3)
        {
            Label = string.Intern(string.Concat(label1, ".", label2, ".", label3));
        }

        public ProfilerSpace(string label1, string label2, string label3, string label4)
        {
            Label = string.Intern(string.Concat(label1, ".", label2, ".", label3, ".", label4));
        }

        internal void Hook(MethodInfo methodToHook)
        {
            // 获取原始方法的 IL 指令
            var originalIL = GetMethodIL(methodToHook);
            if (originalIL is null) return;

            // 获取新的 IL 指令，包括前后 hook 的调用
            var newIL = GenerateHookedIL(methodToHook, originalIL);
            if (newIL is null) return;

            // 使用 Emit 替换原始方法的 IL 指令
            ReplaceMethodIL(methodToHook, newIL);
        }

        private byte[] GetMethodIL(MethodInfo method)
        {
            var methodBody = method.GetMethodBody();
            return methodBody?.GetILAsByteArray();
        }

        private static MethodInfo HookBeforeType;
        private static MethodInfo HookAfterType;
        private static readonly Type[] HookBeforeTypeParams;

        static ProfilerSpace()
        {
            HookBeforeTypeParams = new[] { typeof(object), typeof(object) };
        }

        private byte[] GenerateHookedIL(MethodInfo method, byte[] originalIL)
        {
            if (method.DeclaringType is null) return null;

            HookBeforeType ??=
                typeof(Profiler).GetMethod(nameof(Profiler.BeginSample), new Type[] { typeof(string) });

            HookAfterType ??=
                typeof(Profiler).GetMethod(nameof(Profiler.EndSample), BindingFlags.Public | BindingFlags.Static);

            if (HookBeforeType is null) return null;
            if (HookAfterType is null) return null;
            if (string.IsNullOrEmpty(Label)) Label = string.Concat(method.DeclaringType.Name, ".", method.Name);
            var dynamicMethod = new DynamicMethod(
                string.Concat(method.Name, "_Hooked"),
                method.ReturnType,
                method.GetParameters().Select(p => p.ParameterType).ToArray(),
                method.DeclaringType.Module,
                skipVisibility: true
            );
            var ilGenerator = dynamicMethod.GetILGenerator();

            // 在方法开始处调用 HookBefore 传递参数 Label:string 
            Console.WriteLine(Label);
            ilGenerator.Emit(OpCodes.Ldstr, Label);
            ilGenerator.Emit(OpCodes.Call, HookBeforeType);

            // 加载原始方法的参数
            foreach (var param in method.GetParameters())
                ilGenerator.Emit(OpCodes.Ldarg, param.Position + 1);

            // 调用原始方法
            ilGenerator.Emit(OpCodes.Call, method);

            // 在方法结束处调用 Hook After
            ilGenerator.Emit(OpCodes.Call, HookAfterType);

            // 返回值
            ilGenerator.Emit(OpCodes.Ret);
            var body = dynamicMethod.GetMethodBody();
            if (body is null) throw new Exception("Method body is null");

            // 生成新的 IL 指令
            return body.GetILAsByteArray();
        }

        private void ReplaceMethodIL(MethodInfo method, byte[] newIL)
        {
            unsafe
            {
                fixed (byte* pointer = newIL)
                {
                    // 替换方法的 IL 指令
                    int size = newIL.Length;
                    RuntimeHelpers.PrepareMethod(method.MethodHandle);
                    byte* destination = (byte*)method.MethodHandle.Value.ToPointer() + 8;
                    for (int i = 0; i < size; i++)
                    {
                        destination[i] = pointer[i];
                    }
                }
            }
        }

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var method in type.GetMethods())
                    {
                        foreach (var attribute in method.GetCustomAttributes<ProfilerSpace>(false))
                        {
                            if (attribute is null) continue;
                            attribute.Hook(method);
                        }
                    }
                }
            }
        }
    }

    partial class AssetSystem
    {
    
    }
}