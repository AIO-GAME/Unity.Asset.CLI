# -*- coding: UTF-8 -*-

import argparse
import base64
import hashlib
import json
import os
import shutil
import subprocess
import sys
import tempfile
import time
from typing import Dict, Tuple, List


# -t ${Platform} -l ${localPath} -r ${server}
# -mk 'cache-control' -mv 'public, max-age=604800'
# -mk 'content-type' -mv 'application/json'
# -mk 'content-encoding' -mv 'gzip'
def parse_arguments():  # 获取传入的参数
    parser = argparse.ArgumentParser(description='上传谷歌云资源')
    parser.add_argument('-v', '--version', type=str, required=False, help='版本号', default="Latest")
    parser.add_argument('-d', '--isLatest', type=bool, required=False, help='默认最新版本', default=False, const=True,
                        nargs='?')
    parser.add_argument('-mk', '--metaDataKey', type=str, required=False, help='元数据键', nargs='*')
    parser.add_argument('-mv', '--metaDataValue', type=str, required=False, help='元数据值', nargs='*')
    parser.add_argument('-l', '--localPath', type=str, required=True, help='本地上传目录')
    parser.add_argument('-r', '--remotePath', type=str, required=True, help='谷歌云远端目录')
    parser.add_argument('-p', '--packageName', type=str, required=True, help='包名')
    parser.add_argument('-t', '--buildTarget', type=str, required=True, help='目标平台',
                        choices=["Android", "iOS", "StandaloneWindows64", "WebGL", "StandaloneOSX"])
    return parser.parse_args()


def exit_handler():  # 退出处理
    if exitCode != 0:
        elapsed_time = time.time() - start_time  # 计算经过的时间（单位为秒）
        print("[上传失败] 所用时间 ：{0:.2f}秒".format(elapsed_time))
        exit(exitCode)
        pass
    exit_code = gcloud_upload_file("{0}/PackageManifest_{1}.version".format(_Remote, args.packageName),
                                   "{0}/PackageManifest_{1}.version".format(_Local, args.packageName))
    elapsed_time = time.time() - start_time  # 计算经过的时间（单位为秒）
    print("[上传{0}] 所用时间 ：{1:.2f}秒".format(exit_code == 0 and "成功" or "失败", elapsed_time))
    exit(exit_code)


def local_copy_file(source_path, destination_path):
    try:
        shutil.copy(source_path, destination_path)
    except FileNotFoundError:
        print(f"Source file {source_path} not found.")
    except PermissionError:
        print(f"Permission denied. Unable to copy {source_path} to {destination_path}")
    except Exception as e:
        print(f"An error occurred: {e}")


def local_get_folder_size(folder_path):
    total_size = 0

    # 遍历文件夹中的所有文件和子文件夹
    for dir_path, dir_names, filenames in os.walk(folder_path):
        for filename in filenames:
            file_path = os.path.join(dir_path, filename)
            # 获取文件大小并累加到总大小
            total_size += os.path.getsize(file_path)

    # 将字节大小转换为更合适的单位（如MB）
    total_size_mb = total_size / (1024 * 1024)
    return total_size_mb


def local_delete_dir(path: str):
    if os.path.exists(path):
        shutil.rmtree(path)


def local_delete_file(path: str):
    if os.path.exists(path):
        os.remove(path)


def local_calculate_md5(location):
    with open(location, 'rb') as file:
        data = file.read()
    return hashlib.md5(data).hexdigest()


def gcloud_delete_files(remotes: List[str]):
    messages = " ".join(["\"gs://{0}\"".format(item.replace('\\', '/').rstrip('/')) for item in set(remotes) if item])
    command = f"gsutil -m -o \"GSUtil:parallel_process_count=1\" rm -r {messages}"
    print(f"[执行命令] {command}")
    try:
        process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, text=True, bufsize=1)
        process.communicate()
        return process.returncode
    except subprocess.CalledProcessError:
        return -1


def gcloud_get_file_md5(remote):
    remote = remote.replace('\\', '/')
    command = "gsutil hash -m \"gs://{0}\"".format(remote)
    print("[执行命令] {0}".format(command))
    try:
        output = subprocess.check_output(command, shell=True, text=True)
        if output:
            hash_value = output.split('\n')[1].split(' ')[1].replace("(md5):", "").strip()
            return base64.b64decode(hash_value).hex()
    except subprocess.CalledProcessError:
        return ""
    return ""


def gcloud_read_text(remote):
    remote = remote.replace('\\', '/')
    local = tempfile.mktemp()
    command = "gsutil cp \"gs://{0}\" \"{1}\"".format(remote, local)
    print("[执行命令] {0}".format(command))
    content = ""
    try:
        process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, text=True, bufsize=1)
        process.communicate()
        if process.returncode == 0:
            with open(local, "r") as file:
                content = file.read()
        if os.path.exists(local):
            os.remove(local)
    except subprocess.CalledProcessError:
        return content
    return content


def gcloud_exists(remote):
    remote = remote.replace('\\', '/')
    command = "gsutil ls \"gs://{0}\"".format(remote)
    print("[执行命令] {0}".format(command))
    try:
        output = subprocess.check_output(command, shell=True, text=True)
        if output and output.startswith("gs://"):
            for item in output.split("gs://"):
                if item.rstrip().endswith(remote):
                    return True
    except subprocess.CalledProcessError:
        return False
    return False


def gcloud_upload_file(remote, location):
    location = location.replace('\\', '/').rstrip('/')
    if not os.path.exists(location):
        return
    remote = os.path.dirname(remote).replace('\\', '/').rstrip('/')
    command = "gsutil -m -o \"GSUtil:parallel_process_count=1\" {0} cp \"{1}\" \"gs://{2}/\"".format(
        metaData, location, remote)
    print("[执行命令] {0}".format(command))
    try:
        # os.system(command)
        process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, text=True, bufsize=1)
        while True:
            output = process.stdout.readline()
            if output == '' and process.poll() is not None:
                break
            if output:  # 输出命令执行结果 更新上传进度
                print("\r{0}".format(output.strip()), end="")
            process.communicate()
            return process.returncode
    except subprocess.CalledProcessError:
        return -1
    return 0


def gcloud_clean_cache(project: str, root: str, is_async: bool = False):
    command_temp = f'gcloud compute url-maps invalidate-cdn-cache --project={project} rol-balancer --path=/{root}/*'
    if is_async:
        command_temp += ' --async'
    print("[执行命令] {0}".format(command_temp))
    try:
        process_temp = subprocess.Popen(command_temp, shell=True, stdout=subprocess.PIPE, text=True, bufsize=1)
        process_temp.communicate()
        return process_temp.returncode
    except subprocess.CalledProcessError:
        return -1


def gcloud_upload_dir(remote, location):
    location = location.replace('\\', '/').rstrip('/')
    if not os.path.exists(location):
        return -1
    # 给temp_dir目录下的文件添加权限
    for root, dirs, files in os.walk(location):
        for file in files:
            os.chmod(os.path.join(root, file), 0o777)
    try:
        remote = remote.replace('\\', '/').rstrip('/')
        command = "gsutil -m -o \"GSUtil:parallel_process_count=1\" {0} cp -r \"{1}\" \"gs://{2}\"".format(
            metaData, location, remote)
        print("[执行命令] {0}".format(command))
        # 循环读取子进程的输出 会导致多线程上传失效
        process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, text=True, bufsize=1)
        while True:
            output = process.stdout.readline()
            if output == '' and process.poll() is not None:
                break
            if output:  # 输出命令执行结果 更新上传进度
                print("\r{0}".format(output.strip()), end="")
            process.communicate()
            return process.returncode
    except subprocess.CalledProcessError:
        return -1
    return 0


def gcloud_update_meta(remote, meta_data_key, meta_data_value):
    remote = remote.replace('\\', '/').rstrip('/')
    command = "gsutil setmeta -h \"{0}:{1}\" \"gs://{2}\"".format(meta_data_key, meta_data_value, remote)
    print("[执行命令] {0}".format(command))
    try:
        process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, text=True, bufsize=1)
        process.communicate()
        return process.returncode
    except subprocess.CalledProcessError:
        return -1


def gcloud_update_version():
    version_path = "{0}/Version/{1}.json".format(args.remotePath, args.buildTarget)
    print("[更新版本] {0} : {1}".format(args.packageName, args.version))
    version_content = []
    if gcloud_exists(version_path):
        temp = gcloud_read_text(version_path)
        version_content = json.loads(temp) if temp else []
        has = False
        for item in version_content:
            if item["Name"] == args.packageName:
                item["Version"] = args.isLatest and "Latest" or args.version
                has = True
                break
        if not has:
            version_content.append({"Name": args.packageName, "Version": args.version})
    else:
        version_content.append({"Name": args.packageName, "Version": args.version})
    version_str = json.dumps(version_content, indent=4, separators=(',', ': '), sort_keys=True)
    version_table = tempfile.mktemp()
    with open(version_table, "w") as file:
        file.write(version_str)
    exit_code = gcloud_upload_file(version_path, version_table)
    os.remove(version_table)
    return exit_code


def comparison_manifest(current: Dict[str, str], target: Dict[str, str]) \
        -> Tuple[Dict[str, str], Dict[str, str], Dict[str, str]]:
    delete = {}  # 删除
    change = {}  # 修改

    add = {aKey: aValue for aKey, aValue in current.items() if aKey not in target}  # 新增
    for iKey, iValue in target.items():
        if iKey not in current:  # 如果当前清单文件中不存在 则代表需要删除目标版本指定文件
            delete[iKey] = iValue
            continue

        if current[iKey] != iValue:  # 修改
            change[iKey] = current[iKey]

    return add, delete, change


start_time = time.time()  # 记录起始时间
args = parse_arguments()
exitCode = 0

if args.version == "":
    print("版本号不能为空")
    exit(1)

if args.localPath == "":
    print("本地路径不能为空")
    exit(1)

if args.remotePath == "":
    print("远端路径不能为空")
    exit(1)

if args.isLatest:
    args.version = "Latest"

metaData = ""
if args.metaDataKey and args.metaDataValue:
    for i in range(len(args.metaDataKey)):
        metaData += "-h \"{0}:{1}\" ".format(args.metaDataKey[i], args.metaDataValue[i])
    metaData = metaData.strip()

# 需要拿到执行命令的输出字符串
_Remote = "{0}/{1}/{2}/{3}".format(args.remotePath, args.buildTarget, args.packageName, args.version)
_RemoteManifest = "{0}/Manifest.json".format(_Remote)
_Local = "{0}/{1}/{2}/{3}".format(args.localPath, args.buildTarget, args.packageName, args.version)
_LocalManifest = "{0}/Manifest.json".format(_Local)

print("[清单状态] {0} 获取中".format(_RemoteManifest))
if gcloud_exists(_RemoteManifest):
    _RemoteManifestMD5 = gcloud_get_file_md5(_RemoteManifest)
    _LocalManifestMD5 = local_calculate_md5(_LocalManifest)
    print("[比较清单] MD5 (远端:{0}) {2} (本地:{1})".format(
        _RemoteManifestMD5, _LocalManifestMD5, (_RemoteManifestMD5 == _LocalManifestMD5 and "==" or "!=")))
    if _RemoteManifestMD5 == _LocalManifestMD5:
        print("[清单一致] 无需更新")
        exit(0)
        pass

    # 获取本地清单列表
    with open(_LocalManifest, "r") as f:
        locationManifestStr = f.read()
    locationManifest = json.loads(locationManifestStr)
    # 获取远端清单列表
    remoteManifestStr = gcloud_read_text(_RemoteManifest)
    remoteManifest = json.loads(remoteManifestStr)
    add_files, delete_files, change_files = comparison_manifest(locationManifest, remoteManifest)
    if len(add_files) == 0 and len(delete_files) == 0 and len(change_files) == 0:
        print("[清单一致] 无需更新")
        exit(0)
        pass

    temp_dir = os.path.join(tempfile.mktemp(), args.version)
    if not os.path.exists(temp_dir):
        os.makedirs(temp_dir, exist_ok=True)
    if len(add_files) > 0:
        print("[新增列表] - {0}".format(len(add_files)))
        for key, value in add_files.items():
            if key.startswith("BuildReport_"):
                delete_files[key] = value
                if remoteManifest.get(key):
                    del remoteManifest[key]
                continue

            remoteManifest[key] = value
            locationTemp = "{0}/{1}".format(_Local, key)
            if os.path.exists(locationTemp):
                local_copy_file(locationTemp, "{0}/{1}".format(temp_dir, key))
                print("{0} : {1}".format(value, key))
            else:
                print("目标新增文件不存在 : {0} 目标源结构被篡改 请重新构建资源".format(locationTemp))

    if len(change_files) > 0:
        print("[修改列表](先删除后上传) - {0}".format(len(change_files)))
        for key, value in change_files.items():
            remoteManifest[key] = value
            locationTemp = "{0}/{1}".format(_Local, key)
            if os.path.exists(locationTemp):
                local_copy_file(locationTemp, "{0}/{1}".format(temp_dir, key))
                print("{0} : {1}".format(value, key))
                add_files[key] = value
                delete_files[key] = value
            else:
                if key.startswith("BuildReport_"):
                    continue
                print("目标修改文件不存在 : {0} 目标源结构被篡改 请重新构建资源".format(locationTemp))

    if len(delete_files) > 0:
        print("[删除列表] - {0}".format(len(delete_files)))
        for key, value in delete_files.items():
            if remoteManifest.get(key):
                del remoteManifest[key]
                print("{0} : {1}".format(value, key))
            else:
                if key.startswith("BuildReport_"):
                    continue
                print("目标删除文件不存在 : {0} 目标源结构被篡改 请重新构建资源".format(key))

    if len(delete_files.keys()) > 0:
        table = []
        for key, value in delete_files.items():
            table.append("{0}/{1}".format(_Remote, key))
        gcloud_delete_files(table)

    if len(add_files.keys()) > 0:
        exitCode = gcloud_upload_dir(
            "{0}/{1}/{2}".format(args.remotePath, args.buildTarget, args.packageName), temp_dir)
        local_delete_dir(temp_dir)
        if exitCode != 0:
            print("[新增修改] 任务失败")
            exit(exitCode)

    remoteManifestStr = json.dumps(remoteManifest, indent=4, separators=(',', ': '), sort_keys=True)
    temp_file = tempfile.mktemp()
    with open(temp_file, "w") as f:
        f.write(remoteManifestStr)
    exitCode = gcloud_upload_file(_RemoteManifest, temp_file)
    local_delete_file(temp_file)
else:
    exitCode = gcloud_upload_dir(_Remote, _Local)

if exitCode == 0:
    print("[清单状态] {0} 不存在".format(_RemoteManifest))
    exitCode = gcloud_update_version()

exit_handler()
