# -*- coding: UTF-8 -*-

import os
import stat
import ctypes, sys

def enum(label, keyList, valueList):
	print("\n======================================================================")
	print("- 请选择" + label + ":")
	for x in range(len(keyList)):
		print("- " + str(x + 1) + ":" + keyList[x])
		pass
	print("- other:忽略")
	index = int(input('')) - 1
	maxIndex = len(valueList)
	if (index > maxIndex):
		return "null"
	for x in range(maxIndex):
		if (x == index):
			return valueList[x]
		pass
	return "null"

def UnityRun(MethodArgs):
	print("- 请输入 unity.exe 文件路径:")
	value = str(input(''))
	unityExe = "D:\\Unity\\Unity2020.3.37f1\\Editor\\Unity.exe"
	if (len(value) > 0): unityExe = value

	# 项目路径
	print("- 请输入 unity 项目路径:")
	value = str(input(''))
	projectPath = "E:\\TencentGit\\AIO20200337\\"
	if (len(value) > 0): projectPath = value
	
	# 日志文件输出路径
	print("- 请输入 日志文件输出路径:")
	value = str(input(''))
	logfile = "E:\\TencentGit\\AIO20200337\\Build\\test\\beedom20230104.android.log"
	if (len(value) > 0): logfile = value

	# Unity执行函数
	print("- 请输入 Unity 执行函数:")
	value = str(input(''))
	executeMethod = "AIO.Build.Editor.YooAssetBuild.ArtBuild"
	if (len(value) > 0): executeMethod = value

	command = unityExe + " -projectPath " + projectPath +" -quit -batchmode -logfile " + logfile + " -executeMethod " + executeMethod + " \"" + MethodArgs + "|\""

	print(command)
	print("\n开始执行 请稍等")
	os.system(command)

def main():
	os.system("@echo off&@setlocal enabledelayedexpansion")
	os.system("@chcp 65001&@color F")

	## -----说明文字-----
	print("\n======================================================================")
	print("- Welcome  :美术资源打包")
	print("======================================================================\n")

	args = []
	#--------------------------------------------------------------
	keyList = []
	keyList.append("BuiltinBuildPipeline (传统内置构建管线)")
	keyList.append("ScriptableBuildPipeline (可编程构建管线)")

	valueList = []
	valueList.append("b@BuildPipeline BuiltinBuildPipeline")
	valueList.append("b@BuildPipeline ScriptableBuildPipeline")

	value = enum("BuildPipeline 构建管线类型 (必选)", keyList, valueList)
	if (value != "null"): args.append(value)
	#--------------------------------------------------------------
	keyList = []
	keyList.append("None (不拷贝任何文件)")
	keyList.append("ClearAndCopyAll (先清空已有文件，然后拷贝所有文件)")
	keyList.append("ClearAndCopyByTags (先清空已有文件，然后按照资源标签拷贝文件)")
	keyList.append("OnlyCopyAll (不清空已有文件，直接拷贝所有文件)")
	keyList.append("OnlyCopyByTags (不清空已有文件，直接按照资源标签拷贝文件)")
	
	valueList = []
	valueList.append("b@BuildPipeline None")
	valueList.append("b@BuildPipeline ClearAndCopyAll")
	valueList.append("b@BuildPipeline ClearAndCopyByTags")
	valueList.append("b@BuildPipeline OnlyCopyAll")
	valueList.append("b@BuildPipeline OnlyCopyByTags")
	
	value = enum("BuildPipeline 首包资源文件的拷贝方式: (可忽略)", keyList, valueList)
	if (value != "null"): args.append(value)
	#--------------------------------------------------------------
	keyList = []
	keyList.append("HashName (哈希值名称)")
	keyList.append("BundleName_HashName (资源包名称 + 哈希值名称)")
	
	valueList = []
	valueList.append("b@OutputNameStyle HashName")
	valueList.append("b@OutputNameStyle BundleName_HashName")
	
	value = enum("OutputNameStyle 输出文件名称的样式: (可忽略)", keyList, valueList)
	if (value != "null"): args.append(value)
	#--------------------------------------------------------------
	keyList = []
	keyList.append("ForceRebuild (强制重建模式)")
	keyList.append("IncrementalBuild (增量构建模式)")
	keyList.append("DryRunBuild (演练构建模式)")
	keyList.append("SimulateBuild (模拟构建模式)")
	
	valueList = []
	valueList.append("b@BuildMode ForceRebuild")
	valueList.append("b@BuildMode IncrementalBuild")
	valueList.append("b@BuildMode DryRunBuild")
	valueList.append("b@BuildMode SimulateBuild")
	
	value = enum("BuildMode 资源包流水线的构建模式: (可忽略)", keyList, valueList)
	if (value != "null"): args.append(value)
	#--------------------------------------------------------------
	keyList = []
	keyList.append("Android (安卓)")
	keyList.append("iOS (苹果)")
	keyList.append("WebGL (Web网页)")
	keyList.append("StandaloneWindows (PC)")
	
	valueList = []
	valueList.append("b@ActiveTarget Android")
	valueList.append("b@ActiveTarget iOS")
	valueList.append("b@ActiveTarget WebGL")
	valueList.append("b@ActiveTarget StandaloneWindows")
	
	value = enum("ActiveTarget 资源打包目标平台: (可忽略)", keyList, valueList)
	if (value != "null"): args.append(value)
	#--------------------------------------------------------------
	keyList = []
	keyList.append("Uncompressed")
	keyList.append("LZMA")
	keyList.append("LZ4")
	
	valueList = []
	valueList.append("b@CompressOption Uncompressed")
	valueList.append("b@CompressOption LZMA")
	valueList.append("b@CompressOption LZ4")
	
	value = enum("CompressOption AssetBundle压缩选项: (可忽略)", keyList, valueList)
	if (value != "null"): args.append(value)
	#--------------------------------------------------------------
	print("- 请输入 输出根目录:")
	value = str(input(''))
	if (len(value) > 0): args.append("b@OutputRoot " + value)
	#--------------------------------------------------------------
	print("- 请输入 资源目标包:")
	value = str(input(''))
	if (len(value) > 0): args.append("b@BuildPackage " + value)
	#--------------------------------------------------------------
	print("- 请输入 资源目标版本:")
	value = str(input(''))
	if (len(value) > 0): args.append("b@PackageVersion " + value)
	#--------------------------------------------------------------
	print("- 请输入 首包资源文件的标签集合: (可忽略)")
	value = str(input(''))
	if (len(value) > 0): args.append("b@CopyBuildinFileTags " + value)
	#--------------------------------------------------------------
	print("- 请输入 加密类名称: (可忽略)")
	value = str(input(''))
	if (len(value) > 0): args.append("b@EncyptionClassName " + value)
	#--------------------------------------------------------------
	print("- 请输入 验证构建结果 1:验证 other:忽略 (可忽略)")
	value = str(input(''))
	if (value == "1"): args.append("b@VerifyBuildingResult ")

	print("\n======================================================================")
	arg = ''
	for index in range(len(args)):
		arg = arg + ' ' +args[index]
		print(args[index])
		pass
	print("======================================================================\n")

	UnityRun(arg)
	

	print("- 打包完成: 输入任意键退出本程序")
	str(input(''))

def is_admin():
	try: return ctypes.windll.shell32.IsUserAnAdmin()
	except: return False

if is_admin(): main()
else: ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, __file__, None, 1)