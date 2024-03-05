# -*- coding: UTF-8 -*-

import argparse
import hashlib
import json
import os
import shutil
import sys
import tempfile
import time
from ftplib import FTP
from typing import Dict, Tuple, List


# -t ${Platform} -l ${localPath} -r ${server} -u ${username} -s ${password} -p ${packageName}
def parse_arguments():  # 获取传入的参数
    parser = argparse.ArgumentParser(description='上传FTP资源')
    parser.add_argument('-t', '--buildTarget', type=str, required=True,
                        choices=["Android", "iOS", "StandaloneWindows64", "WebGL", "StandaloneOSX"], help='目标平台')
    parser.add_argument('-l', '--localPath', type=str, required=True, help='本地上传目录')
    parser.add_argument('-r', '--remoteRoot', type=str, required=True, help='FTP远端根目录')
    parser.add_argument('-rr', '--remoteRelative', type=str, required=False, help='FTP远端相对目录')
    parser.add_argument('-u', '--username', type=str, required=True, help='FTP用户名')
    parser.add_argument('-s', '--password', type=str, required=True, help='FTP密码')
    parser.add_argument('-p', '--packageName', type=str, required=True, help='包名')
    parser.add_argument('-rp', '--port', type=int, required=False, help='FTP端口', default=21)
    parser.add_argument('-v', '--version', type=str, required=False, help='版本号', default="Latest")
    parser.add_argument('-d', '--isLatest', type=bool, required=False, help='默认最新版本', default=False)
    return parser.parse_args()


def local_copy_file(source_path, destination_path):
    try:
        shutil.copy(source_path, destination_path)
    except FileNotFoundError:
        print(f"Source file {source_path} not found.")
    except PermissionError:
        print(f"Permission denied. Unable to copy {source_path} to {destination_path}")
    except Exception as e:
        print(f"An error occurred: {e}")


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


def ftp_delete_files(remotes: List[str]):  # ftp删除远端文件
    ftp.cwd('/')
    for remote in remotes:
        try:
            if ftp_file_exists(remote):
                ftp.delete(remote)
                print(f"[删除文件] {remote} deleted successfully.")
        except Exception as e:
            return -1
    return 0


def ftp_delete_file(remote):  # ftp删除远端文件
    try:
        ftp.cwd('/')
        ftp.delete(remote)
        print(f"File {remote} deleted successfully.")
        return 0
    except Exception as e:
        print(f"File {remote} deleted failed: {e}")
        return -1


def ftp_get_file_md5(remote):  # ftp获取远端文件MD5
    if not ftp_file_exists(remote):
        return ""
    try:
        temp_mkdir = tempfile.mktemp()
        temp_path = os.path.join(temp_mkdir, "temp")
        os.makedirs(temp_mkdir, exist_ok=True)
        ftp.cwd('/')
        ftp.retrbinary("RETR {0}".format(remote), open(temp_path, "wb").write)
        with open(temp_path, 'rb') as file:
            data = file.read()
        md5 = hashlib.md5(data).hexdigest()
        os.remove(temp_path)
        return md5
    except Exception as e:
        print(f"Error: {e}")
        return ""


def ftp_read_text(remote):  # ftp读取远端文件
    if not ftp_file_exists(remote):
        return ""
    try:
        temp_mkdir = tempfile.mktemp()
        temp_path = os.path.join(temp_mkdir, "temp")
        os.makedirs(temp_mkdir, exist_ok=True)
        ftp.cwd('/')
        ftp.retrbinary("RETR {0}".format(remote), open(temp_path, "wb").write)
        with open(temp_path, 'rb') as file:
            data = file.read()
        os.remove(temp_path)
        return data.decode("utf-8")
    except Exception as e:
        print(f"Error: {e}")
        return ""


def ftp_file_exists(remote):
    # ftp判断远端文件是否存在
    try:
        ftp.cwd('/')
        paths = ftp.nlst(os.path.dirname(remote))
        return remote in paths
    except Exception as e:
        print(f"ftp not exists {remote} : {e}")
        return False


def ftp_dir_exists(remote):
    # ftp判断远端文件夹是否存在
    try:
        ftp.cwd('/')
        ftp.cwd(remote)
        return True
    except:
        return False


def ftp_upload_file(remote, location):  # ftp上传文件
    location = location.replace('\\', '/').rstrip('/')
    if not os.path.exists(location):
        return -1
    try:
        remote = remote.replace('\\', '/').rstrip('/')
        dirname = os.path.dirname(remote)
        if not ftp_dir_exists(dirname):
            if ftp_create_dir(dirname) != 0:
                return -1

        ftp.cwd('/')
        with open(location, 'rb') as file:
            ftp.storbinary('STOR ' + remote, file)
        return 0
    except Exception as e:
        print(f"uploaded {remote} location failed: {e}")
        return -1


def ftp_create_dir(remote):
    try:  # ftp创建远端文件夹
        names = remote.replace('\\', '/').split('/')
        template = ""
        for dirname in names:
            template = os.path.join(template, dirname).replace('\\', '/')
            if template == "":
                continue
            if not ftp_dir_exists(template):
                ftp.mkd(template)
        return 0
    except Exception as e:
        print(f"Dir {remote} created failed: {e}")
        return -1


def ftp_upload_dir(remote, location):  # ftp上传文件夹
    location = location.replace('\\', '/').rstrip('/')
    if not os.path.exists(location):
        return -1
    try:
        remote = remote.replace('\\', '/').rstrip('/')
        if not ftp_dir_exists(remote):
            if ftp_create_dir(remote) != 0:
                return -1

        ftp.cwd('/')
        for item in os.listdir(location):  # 遍历本地文件夹中的所有文件和子文件夹
            local_item = os.path.join(location, item).replace('\\', '/')
            remote_item = os.path.join(remote, item).replace('\\', '/')
            if os.path.isfile(local_item):
                ftp_upload_file(remote_item, local_item)  # 如果是文件，上传文件到FTP服务器
            elif os.path.isdir(local_item):
                ftp_upload_dir(remote_item, local_item)  # 如果是文件夹，递归上传文件夹
            sys.stdout.write(f"\r[上传进度] {remote_item} 上传中...")
            sys.stdout.write("\n")
            sys.stdout.flush()
        sys.stdout.write(f"\r[上传状态] 完成\n")
        sys.stdout.flush()
        return 0
    except Exception as e:
        sys.stdout.flush()
        print(f"Dir {remote} uploaded failed: {e}")
        return -1


def ftp_update_version():
    print("[更新版本] {0} : {1}".format(args.packageName, args.version))
    version_content = []
    version_path = "{0}/Version/{1}.json".format(args.remoteRelative, args.buildTarget)
    if ftp_file_exists(version_path):
        temp = ftp_read_text(version_path)
        version_content = json.loads(temp) if temp else []
        has = False
        for item in version_content:
            if item["Name"] == args.packageName:
                item["Version"] = args.version
                item["CompressMode"] = 2
                has = True
                break
        if not has:
            version_content.append({"Name": args.packageName, "Version": args.version, "CompressMode": 2})
    else:
        version_content.append({"Name": args.packageName, "Version": args.version, "CompressMode": 2})
    version_str = json.dumps(version_content, indent=4, separators=(',', ': '), sort_keys=True)
    version_table = tempfile.mktemp()
    with open(version_table, "w") as file:
        file.write(version_str)
    exit_code = ftp_upload_file(version_path, version_table)
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


args = parse_arguments()
if args.isLatest:
    args.version = "Latest"

if args.version == "":
    print("版本号不能为空")
    sys.exit(-1)

start_time = time.time()  # 记录起始时间

print("[开始连接] {0}".format(args.remoteRoot))
ftp = FTP(args.remoteRoot, user=args.username, passwd=args.password, timeout=10, encoding="utf-8")
if args.remoteRelative:
    try:
        ftp.cwd(args.remoteRelative)
    except:
        print("[创建目录] {0}".format(args.remoteRelative))
        ftp.mkd(args.remoteRelative)
        ftp.cwd(args.remoteRelative)

# 需要拿到执行命令的输出字符串
_Remote = "{0}/{1}/{2}/{3}".format(args.remoteRelative, args.buildTarget, args.packageName, args.version)
_RemoteManifest = "{0}/Manifest.json".format(_Remote)
_Local = "{0}/{1}/{2}/{3}".format(args.localPath, args.buildTarget, args.packageName, args.version).replace('\\', '/')
_LocalManifest = "{0}/Manifest.json".format(_Local)

exitCode = 0
sys.stdout.write("\r[清单状态] {0} 获取中".format(_RemoteManifest))
if ftp_file_exists(_RemoteManifest):
    sys.stdout.write("\r[清单状态] {0} 已存在".format(_RemoteManifest))
    sys.stdout.write("\n")
    sys.stdout.flush()
    _RemoteManifestMD5 = ftp_get_file_md5(_RemoteManifest)
    _LocalManifestMD5 = local_calculate_md5(_LocalManifest)
    print("[比较清单] MD5 (远端:{0}) {2} (本地:{1})".format(
        _RemoteManifestMD5, _LocalManifestMD5, (_RemoteManifestMD5 == _LocalManifestMD5 and "==" or "!=")))
    if _RemoteManifestMD5 == _LocalManifestMD5:
        print("[清单一致] 无需更新")
        sys.exit(0)
        pass

    # 获取本地清单列表
    with open(_LocalManifest, "r") as f:
        locationManifestStr = f.read()
    locationManifest = json.loads(locationManifestStr)
    # 获取远端清单列表
    remoteManifestStr = ftp_read_text(_RemoteManifest)
    remoteManifest = json.loads(remoteManifestStr)
    add_files, delete_files, change_files = comparison_manifest(locationManifest, remoteManifest)
    if len(add_files) == 0 and len(delete_files) == 0 and len(change_files) == 0:
        print("[清单一致] 无需更新")
        sys.exit(0)
        pass

    print("[^-^-^-->]")
    temp_dir = os.path.join(tempfile.mktemp(), args.version)
    if not os.path.exists(temp_dir):
        os.makedirs(temp_dir, exist_ok=True)
    if len(add_files) > 0:
        print("[新增文件] - {0}".format(len(add_files)))
        for key, value in add_files.items():
            remoteManifest[key] = value
            locationTemp = "{0}/{1}".format(_Local, key)
            if os.path.exists(locationTemp):
                local_copy_file(locationTemp, "{0}/{1}".format(temp_dir, key))
                print("{0} : {1}".format(value, key))
            else:
                print("目标新增文件不存在 : {0} 目标源结构被篡改 请重新构建资源".format(locationTemp))

    if len(change_files) > 0:
        print("[修改文件] - {0}".format(len(change_files)))
        for key, value in change_files.items():
            locationTemp = "{0}/{1}".format(_Local, key)
            if os.path.exists(locationTemp):
                remoteManifest[key] = value
                local_copy_file(locationTemp, "{0}/{1}".format(temp_dir, key))
                print("{0} : {1}".format(remoteManifest[key], key))
                add_files[key] = remoteManifest[key]
            else:
                print("目标修改文件不存在 : {0} 目标源结构被篡改 请重新构建资源".format(locationTemp))

    if len(delete_files) > 0:
        print("[删除文件] - {0}".format(len(delete_files)))
        for key, value in delete_files.items():
            if remoteManifest.get(key):
                del remoteManifest[key]
                print("{0} : {1}".format(value, key))
            else:
                print("目标删除文件不存在 : {0} 目标源结构被篡改 请重新构建资源".format(key))

    if len(delete_files.keys()) > 0:
        print("[^-^-^-->]")
        table = []
        for key, value in delete_files.items():
            table.append("{0}/{1}".format(_Remote, key))
        ftp_delete_files(table)

    if len(add_files.keys()) > 0:
        print("[^-^-^-->]")
        exitCode = ftp_upload_dir(_Remote, temp_dir)
        local_delete_dir(temp_dir)
        if exitCode != 0:
            print("[新增修改] 任务失败")
            sys.exit(exitCode)

    print("[更新清单] {0}".format(_RemoteManifest))
    # json 序列化的字典格式 不压缩 并且按照key排序
    remoteManifestStr = json.dumps(remoteManifest, indent=4, separators=(',', ': '), sort_keys=True)
    temp_file = tempfile.mktemp()
    with open(temp_file, "w") as f:
        f.write(remoteManifestStr)
    exitCode = ftp_upload_file(_RemoteManifest, temp_file)
    local_delete_file(temp_file)
else:
    sys.stdout.write("\r[清单状态] {0} 不存在".format(_RemoteManifest))
    sys.stdout.write("\n")
    sys.stdout.flush()
    exitCode = ftp_upload_dir(_Remote, _Local)

if exitCode == 0:
    exitCode = ftp_update_version()
else:
    print("[上传失败] {0}".format(exitCode))

end_time = time.time()  # 记录结束时间
ftp.quit()
elapsed_time = end_time - start_time  # 计算经过的时间（单位为秒）
print("[上传{0}] 所用时间 ：{1:.2f}秒".format(exitCode == 0 and "成功" or "失败", elapsed_time))
