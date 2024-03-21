using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using MonoHook;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

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

    public interface IProfilerHook : IDisposable
    {
        /// <summary>
        /// 注入
        /// </summary>
        void Inject();
    }

    public abstract class ProfilerHook : IProfilerHook
    {
        public string Title { get; }
        private MethodHook Hook { get; }

        protected MethodBase ProxyMethod => Hook.proxyMethod;

        public virtual MethodBase MethodTarget { get; } = null;
        public virtual MethodBase MethodReplace { get; } = null;
        public virtual MethodBase MethodProxy { get; } = null;

        protected ProfilerHook(string title)
        {
            if (MethodTarget is null) throw new ArgumentNullException(nameof(MethodTarget));
            if (MethodReplace is null) throw new ArgumentNullException(nameof(MethodReplace));
            if (MethodProxy is null) throw new ArgumentNullException(nameof(MethodProxy));
            Title = title;
            Hook = new MethodHook(MethodTarget, MethodReplace, MethodProxy);
        }

        public void Inject() => Hook.Install();

        public void Dispose() => Hook.Uninstall();
    }

    public static class Test
    {
        private static MethodHook Hook1;

        private static MethodInfo MethodInfo1;
        private static MethodInfo MethodInfo2;
        private static MethodInfo MethodInfo3;

        [MenuItem("AIO/ProfilerSpace/Test")]
        public static void Test11()
        {
            MethodInfo1 = typeof(Test).GetMethod(nameof(Test1), BindingFlags.NonPublic | BindingFlags.Static);
            if (MethodInfo1 is null) return;
            MethodInfo2 = typeof(Test).GetMethod(nameof(Test2), BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo3 = typeof(Test).GetMethod(nameof(Test3), BindingFlags.NonPublic | BindingFlags.Static);

            // 创建一个和 MethodInfo1 参数一样 返回值等等全都一样 的空内容函数
            var parameterInfos = MethodInfo1.GetParameters();
            var dynamicMethod = new DynamicMethod(
                string.Concat(MethodInfo1.Name, "_Proxy"),
                MethodInfo1.Attributes,
                MethodInfo1.CallingConvention,
                MethodInfo1.ReturnType,
                parameterInfos.Select(p => p.ParameterType).ToArray(),
                MethodInfo1.DeclaringType,
                true);
            for (var i = 0; i < parameterInfos.Length; i++)
            {
                dynamicMethod.DefineParameter(i + 1, parameterInfos[i].Attributes, parameterInfos[i].Name);
            }

            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldstr, "Begin Hook");
            il.Emit(OpCodes.Call, typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(string) }));
            il.Emit(OpCodes.Ret);

            Hook1 = new MethodHook(MethodInfo1, MethodInfo2, dynamicMethod);
            Hook1.Install();
            Test1("111", "222");
            Hook1.Uninstall();
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static void Test1(string a, string b)
        {
            Console.WriteLine($"Hook Console {a} {b}");
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static void Test2(string a, string b)
        {
            Console.WriteLine("Begin Hook");
            Hook1.proxyMethod.Invoke(null, new object[] { a, b });
            Console.WriteLine("End Hook");
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static void Test3(string a, string b)
        {
            Console.WriteLine($"3333333333333 {a} {b}");
        }


        [MenuItem("AIO/ProfilerSpace/Create")]
        private static async void Create()
        {
            const string Template = @"
        internal sealed class ProfilerHook_FULL_NAME_PTR : ProfilerHook
        {
            public ProfilerHook_FULL_NAME_PTR() : base(""TITLE"")
            {
            }

            public override MethodBase MethodTarget =>
                typeof(CLASS_NAME).GetMethod(nameof(CLASS_NAME.METHOD_NAME), METHOD_ARGUMENTS);

            public override MethodBase MethodReplace =>
                typeof(ProfilerHook_FULL_NAME_PTR).GetMethod(nameof(HookReplace), BindingFlags.NonPublic | BindingFlags.Static);

            public override MethodBase MethodProxy =>
                typeof(ProfilerHook_FULL_NAME_PTR).GetMethod(nameof(HookProxy), BindingFlags.NonPublic | BindingFlags.Static);

            [MethodImpl(MethodImplOptions.NoOptimization)]
            private METHOD_RETURN HookReplace(METHOD_PARAMETERS)
            {
                Profiler.BeginSample(Title);
                ProxyMethod.Invoke(null, METHOD_PARAMETERS_INVOKE);
                Profiler.EndSample();
            }

            [MethodImpl(MethodImplOptions.NoOptimization)]
            private METHOD_RETURN HookProxy(METHOD_PARAMETERS)
            {
            }
        }";

            var output = Path.Combine(EHelper.Path.Assets, "Editor/Gen");
            if (!AHelper.IO.ExistsDirEx(output)) AHelper.IO.CreateDir(output);
            var path = Path.Combine(output, $"{nameof(ProfilerHook)}.cs");
            if (AHelper.IO.ExistsFileEx(path)) AHelper.IO.DeleteFile(path);
            await using var writer = new StreamWriter(path, true, Encoding.UTF8);
            await writer.WriteLineAsync("using System;");
            await writer.WriteLineAsync("using System.Reflection;");
            await writer.WriteLineAsync("using System.Runtime.CompilerServices;");
            await writer.WriteLineAsync("using UnityEngine;");
            await writer.WriteLineAsync("using UnityEngine.Profiling;");
            await writer.WriteLineAsync("using MonoHook;");
            await writer.WriteLineAsync("using UnityEditor;");
            await writer.WriteLineAsync("using AIO;");
            await writer.WriteLineAsync("using System.Linq;");
            await writer.WriteLineAsync("using System.Reflection;");
            await writer.WriteLineAsync("using System.Reflection.Emit;\n");
            await writer.WriteLineAsync("namespace AIO.UEditor\n{");
            await writer.FlushAsync();

            var str = new StringBuilder();
            var temp = new StringBuilder();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().
                         Where(assembly => assembly.GetCustomAttribute<ProfilerScopeAttribute>() != null))
            {
     
                foreach (var type in assembly.GetTypes().
                             Where(type => type.IsInterface == false))
                {
                    foreach (var method in type.GetMethods().Where(method =>
                                     method.IsConstructor == false && method.IsFinal == false &&
                                     method.IsSpecialName == false && method.IsAbstract == false).
                                 Where(method => method.GetCustomAttribute<ProfilerScopeAttribute>(false) != null))
                    {
                        await writer.WriteLineAsync($"// {type.FullName}.{method.Name}");
                        await writer.FlushAsync();
                        try
                        {
                            var FULL_NAME = $"{type.FullName}_{method.Name}".Replace('.', '_');
                            var METHOD_NAME = method.Name;
                            var CLASS_NAME = type.FullName;
                            
                            // 需要考虑泛型函数 将泛型函数的名称转换为泛型函数的名称
                            var TITLE = string.Concat(
                                type.FullName, ".", method.Name, "(",
                                string.Join(".", method.GetParameters().Select(p => p.ParameterType.Name))
                                , ")");

                            var METHOD_ARGUMENTS =
                                method.IsPublic ? "BindingFlags.Public" : "BindingFlags.NonPublic";
                            METHOD_ARGUMENTS = string.Concat(METHOD_ARGUMENTS, "|",
                                method.IsStatic ? "BindingFlags.Static" : "BindingFlags.Instance");

                            temp.Clear();
                            foreach (var parameter in method.GetParameters()) // 获取参数的特性 param out ref 等等
                            {
                                foreach (var variable in parameter.GetCustomAttributes())
                                {
                                    temp.Append(variable.
                                        GetType().Name.
                                        ToLower().
                                        Replace("attribute", "").
                                        Replace("paramarray", "params")
                                    ).Append(' ');
                                }

                                temp.Append(parameter.ParameterType.FullName);
                                temp.Append(' ');
                                temp.Append(parameter.Name.ToLower());
                                temp.Append(", ");
                            }

                            var METHOD_PARAMETERS = temp.ToString().TrimEnd(',', ' '); // 获取函数参数 包含参数属性 类型 名称

                            var METHOD_PARAMETERS_INVOKE = string.Concat("new object[] {",
                                string.Join(", ", method.GetParameters().Select(p => p.Name.ToLower())),
                                "}");
                            // 获取函数指针地址
                            var PTR = method.MethodHandle.GetFunctionPointer().ToString("X");
                            var METHOD_RETURN = method.ReturnType.FullName;
                            str.Clear();
                            str.AppendLine(Template);
                            str.Replace("CLASS_NAME", CLASS_NAME);
                            str.Replace("METHOD_NAME", METHOD_NAME);
                            str.Replace("FULL_NAME_PTR", string.Concat(FULL_NAME, "_", PTR));
                            str.Replace("TITLE", TITLE);
                            str.Replace("METHOD_PARAMETERS_INVOKE", METHOD_PARAMETERS_INVOKE);
                            str.Replace("METHOD_PARAMETERS", METHOD_PARAMETERS);
                            str.Replace("METHOD_RETURN", METHOD_RETURN);
                            str.Replace("METHOD_ARGUMENTS", METHOD_ARGUMENTS);
                            str.Replace("System.Void", "void");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                            continue;
                        }

                        await writer.WriteAsync(str.ToString());
                        await writer.FlushAsync();
                    }
                }
            }
            str.Clear();
            await writer.WriteLineAsync("}");
            await writer.FlushAsync();
        }

        // 进入PlayMode之前调用
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract) continue;
                    foreach (var method in type.GetMethods())
                    {
                        foreach (var attribute in method.GetCustomAttributes<ProfilerScopeAttribute>(false))
                        {
                            if (attribute is null) continue;
                            // Hook(method);
                        }
                    }
                }
            }
        }
    }
}