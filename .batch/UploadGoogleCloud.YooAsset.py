# -*- coding: UTF-8 -*-

import argparse
import base64
import hashlib
import json
import os
import shutil
import subprocess
import tempfile
import time
from typing import Dict, Tuple, List
import logging


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
    parser.add_argument('-rp', '--project', type=str, required=True, help='google cloud project', default="")
    return parser.parse_args()


def exit_handler():  # 退出处理
    elapsed_time = time.time() - start_time  # 计算经过的时间（单位为秒）
    if exitCode == 0:
        print("[上传失败] 所用时间 ：{0:.2f}秒".format(elapsed_time))
    else:
        print("[上传成功] 所用时间 ：{0:.2f}秒".format(elapsed_time))
    exit(exitCode)


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
        metaData, location, remote).strip()
    try:
        print("[执行命令] {0}".format(command))
        process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, text=True, bufsize=1)
        while True:
            output = process.stdout.readline()
            if output == '' and process.poll() is not None:
                break
            if output:  # 输出命令执行结果 更新上传进度
                print(output.strip())
            process.communicate()
            return process.returncode
    except subprocess.CalledProcessError:
        return -1
    return 0


def gcloud_clean_cache(project: str, root: str, is_async: bool = False):
    command_temp = f'gcloud compute url-maps invalidate-cdn-cache --project={project} rol-balancer --path=/{root}/*'
    if is_async:
        command_temp += ' --async'
    try:
        print("[执行命令] {0}".format(command_temp))
        process_temp = subprocess.Popen(command_temp, shell=True, stdout=subprocess.PIPE, text=True, bufsize=1)
        process_temp.communicate()
        return process_temp.returncode
    except subprocess.CalledProcessError:
        return -1


def gcloud_upload_dir(remote, location, project: str = ""):
    location = location.replace('\\', '/').rstrip('/')
    if not os.path.exists(location):
        return -1
    for root, dirs, files in os.walk(location):  # 给temp_dir目录下的文件添加权限
        for file in files:
            os.chmod(os.path.join(root, file), 0o777)
    remote = remote.replace('\\', '/').rstrip('/')
    try:
        # command = "gsutil -m -o \"GSUtil:parallel_process_count=1\" {0} cp -r \"{1}\" \"gs://{2}\"".format(
        #     metaData, location, remote)
        # 从 metaData 中 获取 cache-control 的值

        cache_control = None
        if metaData:
            temp_control = str(metaData).split('cache-control:')
            if len(temp_control) > 1:
                cache_control = temp_control[1].split('\"')[1]
        command = ('gcloud storage rsync {0} gs://{1} --delete-unmatched-destination-objects{2}{3}'.format(
            location, remote,
            ' --project=\"{0}\"'.format(project) if project else '',
            ' --cache-control=\"{0}\"'.format(cache_control) if cache_control else '')
        ).strip()

        print("[执行命令] {0}".format(command))
        # 循环读取子进程的输出 会导致多线程上传失效
        process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, text=True, bufsize=1)
        while True:
            output = process.stdout.readline()
            if output == '' and process.poll() is not None:
                break
            if output:  # 输出命令执行结果 更新上传进度
                print(output.strip())
            process.communicate()
        return process.returncode
    except subprocess.CalledProcessError:
        return -1


def gcloud_update_meta(remote, meta_data_key, meta_data_value):
    remote = remote.replace('\\', '/').rstrip('/')
    cmd = "gsutil setmeta -h \"{0}:{1}\" \"gs://{2}\"".format(meta_data_key, meta_data_value, remote)
    print("[执行命令] {0}".format(cmd))
    try:
        process_temp = subprocess.Popen(cmd, shell=True, stdout=subprocess.PIPE, text=True, bufsize=1)
        process_temp.communicate()
        return process_temp.returncode
    except subprocess.CalledProcessError:
        return -1


def gcloud_update_version():
    print("[更新版本] {0} : {1}".format(args.packageName, args.version))
    # 输出的日志 其他进程调用需要接收到
    version_path = "{0}/Version/{1}.json".format(args.remotePath, args.buildTarget)
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
    version_temp_dir = tempfile.mktemp()
    version_temp_file = os.path.join(version_temp_dir, f"{args.buildTarget}.json")
    if not os.path.exists(version_temp_dir):
        os.makedirs(version_temp_dir, exist_ok=True)
    with open(version_temp_file, "w") as file:
        content = json.dumps(version_content, indent=4, separators=(',', ': '), sort_keys=True)
        file.write(content)
        file.flush()
        file.close()
    exit_code = gcloud_upload_file(version_path, version_temp_file)
    os.remove(version_temp_file)
    os.rmdir(version_temp_dir)
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

if args.packageName == "":
    print("包名不能为空")
    exit(1)

if args.project == "":
    print("谷歌云项目名不能为空")
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
_Local = "{0}/{1}/{2}/{3}".format(args.localPath, args.buildTarget, args.packageName, args.version)

if exitCode == 0:
    exitCode = gcloud_upload_dir(_Remote, _Local, args.project)

if exitCode == 0:
    exitCode = gcloud_update_version()

if exitCode == 0:
    exitCode = gcloud_update_meta(
        f"{_Remote}/PackageManifest_{args.packageName}.version",
        "cache-control",
        "no-cache"
    )

exit_handler()
