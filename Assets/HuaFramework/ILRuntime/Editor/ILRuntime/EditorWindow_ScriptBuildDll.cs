using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using ILRuntime.Runtime.CLRBinding;
using HuaFramework;

public class EditorWindow_ScriptBuildDll : EditorWindow
{
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.Space(20);

            //if (GUILayout.Button("1.编译dll[.net版]", GUILayout.Width(305), GUILayout.Height(30)))
            //{
            //    //u3d的 各种dll
            //    var u3dUI = @"G:\Unity2018.2.5\Editor\Data\UnityExtensions\Unity\GUISystem";
            //    var u3dEngine = @"G:\Unity2018.2.5\Editor\Data\Managed\UnityEngine";
            //    if (Directory.Exists(u3dUI) == false || Directory.Exists(u3dEngine) == false)
            //    {
            //        EditorUtility.DisplayDialog("提示", "请修改u3dui 和u3dengine 的dll目录", "OK");
            //    }
            //    ScriptBiuld_Service.BuildDLL_DotNet(Application.dataPath, Application.dataPath + "/Resource/Hotfix", u3dUI,
            //        u3dEngine);
            //    AssetDatabase.Refresh();
            //}

            //第二排
            GUILayout.BeginHorizontal();
            {
                //
                if (GUILayout.Button("1.编译dll(Roslyn-Release)", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    RoslynBuild(ScriptBiuld_Service.BuildMode.Release);
                }

                if (GUILayout.Button("编译dll(Roslyn-Debug)", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    RoslynBuild(ScriptBiuld_Service.BuildMode.Debug);
                }

            }
            //GUILayout.EndHorizontal();
            //GUILayout.BeginHorizontal();
            ////第三排
            //if (GUILayout.Button("2.生成CLRBinding", GUILayout.Width(305), GUILayout.Height(30)))
            //{
            //    GenCLRBindingByAnalysis();
            //}
            //GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();
            //if (GUILayout.Button("3.生成跨域Adapter[没事别瞎点]", GUILayout.Width(305), GUILayout.Height(30)))
            //{
            //    GenCrossBindAdapter();
            //}

            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// 编译模式
    /// </summary>
    /// <param name="mode"></param>
    static void RoslynBuild(ScriptBiuld_Service.BuildMode mode)
    {
        //1.build dll
        var outpath_win = Application.streamingAssetsPath + "/" + "windows";
        ScriptBiuld_Service.BuildDll(Application.dataPath, outpath_win, mode);

        //2.同步到Resource文件夹
        var outpath_AB = Application.dataPath + "/Resource/Hotfix/hotfix.bytes";
        var source = outpath_win + "/hotfix/hotfix.dll";
        var bytes = File.ReadAllBytes(source);
        FileHelper.WriteAllBytes(outpath_AB, bytes);

        //2.同步到其他两个目录
        var outpath_android = Application.streamingAssetsPath + "/" + "android" +
                              "/hotfix/hotfix.dll";
        var outpath_ios = Application.streamingAssetsPath + "/" +
                          "ios" + "/hotfix/hotfix.dll";

        source = outpath_win + "/hotfix/hotfix.dll";
        bytes = File.ReadAllBytes(source);
        if (source != outpath_android)
            FileHelper.WriteAllBytes(outpath_android, bytes);
        if (source != outpath_ios)
            FileHelper.WriteAllBytes(outpath_ios, bytes);

        //3.生成CLRBinding
        GenCLRBindingByAnalysis();
        //4.删除无用dll
        Directory.Delete(outpath_win, true);
        AssetDatabase.WriteImportSettingsIfDirty(Application.dataPath);
        AssetDatabase.Refresh();
        HuaFramework.Debug.Log("脚本打包完毕");
    }

    /// <summary>
    /// 生成跨域继承类适配器
    /// </summary>
    static void GenCrossBindAdapter()
    {
        var types = new List<Type>();
        types.Add((typeof(UnityEngine.ScriptableObject)));
        types.Add((typeof(System.Exception)));
        types.Add(typeof(System.Collections.IEnumerable));
        GenAdapter.CreateAdapter(types, "Assets/Code/Game/ILRuntime/Adapter");
    }

    /// <summary>
    /// 生成clr绑定
    /// </summary>
    static void GenCLRBindingByAnalysis()
    {
        var dllPath = Path.Combine(Application.dataPath, "Resource/Hotfix/hotfix.bytes");
        var dllText = File.ReadAllBytes(dllPath);
        //加载dll
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntimeHelper.LoadHotfix(dllText, null);
        BindingCodeGenerator.GenerateBindingCode(ILRuntimeHelper.AppDomain, "Assets/Code/Game/ILRuntime/Binding");
        AssetDatabase.Refresh();
    }
}
