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
    parser.add_argument('-t', '--buildTarget', type=str, required=True, help='目标平台',
                        choices=["Android", "iOS", "StandaloneWindows64", "WebGL", "StandaloneOSX"])
    parser.add_argument('-l', '--localPath', type=str, required=True, help='本地上传目录')
    parser.add_argument('-p', '--packageName', type=str, required=True, help='包名')
    parser.add_argument('-s', '--remoteRoot', type=str, required=True, help='FTP远端根目录')
    parser.add_argument('-root', '--remoteRelative', type=str, required=False, help='FTP远端相对目录')
    parser.add_argument('-user', '--username', type=str, required=True, help='FTP用户名')
    parser.add_argument('-pass', '--password', type=str, required=True, help='FTP密码')
    parser.add_argument('-timeout', '--timeout', type=int, required=False, help='FTP超时时间', default=30)
    parser.add_argument('-port', '--port', type=int, required=False, help='FTP端口', default=21)
    parser.add_argument('-v', '--version', type=str, required=False, help='版本号', default="Latest")
    parser.add_argument('-d', '--isLatest', type=bool, required=False, help='默认最新版本', default=False, const=True,
                        nargs='?')
    return parser.parse_args()


def exit_handler():
    if exitCode != 0:
        try:  # 关闭FTP连接
            ftp.quit()
        except:
            pass
        end_time = time.time()  # 记录结束时间
        elapsed_time = end_time - start_time  # 计算经过的时间（单位为秒）
        print("[上传失败] 所用时间 ：{0:.2f}秒".format(elapsed_time))
        sys.exit(exitCode)

    exit_code = ftp_upload_file("{0}/PackageManifest_{1}.version".format(_Remote, args.packageName),
                                "{0}/PackageManifest_{1}.version".format(_Local, args.packageName))
    try:  # 关闭FTP连接
        ftp.quit()
    except:
        pass
    end_time = time.time()  # 记录结束时间
    elapsed_time = end_time - start_time  # 计算经过的时间（单位为秒）
    print("[上传{0}] 所用时间 ：{1:.2f}秒".format(exit_code == 0 and "成功" or "失败", elapsed_time))
    sys.exit(exit_code)


def enter_handler():
    if args.remoteRelative:
        try:
            ftp.cwd(args.remoteRelative)
        except:
            print("[创建目录] {0}".format(args.remoteRelative))
            ftp.mkd(args.remoteRelative)
            ftp.cwd(args.remoteRelative)


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
    md5 = hashlib.md5(data).hexdigest()
    return md5


def ftp_delete_files(remotes: List[str]):  # ftp删除远端文件
    ftp.cwd('/')
    for remote in remotes:
        try:
            if ftp_file_exists(remote):
                ftp.delete(remote)
                print(f"[删除文件] {remote} deleted successfully...")
        except:
            return -1
    return 0


def ftp_delete_file(remote):  # ftp删除远端文件
    try:
        ftp.cwd('/')
        ftp.delete(remote)
        print(f"[删除文件] {remote} deleted successfully...")
        return 0
    except:
        return -1


def ftp_get_file_md5(remote):  # ftp获取远端文件MD5
    if not ftp_file_exists(remote):
        return ""
    try:
        temp_mkdir = tempfile.mktemp()
        if not os.path.exists(temp_mkdir):
            os.makedirs(temp_mkdir, exist_ok=True)
        temp_path = os.path.join(temp_mkdir, "temp")
        ftp.cwd('/')
        ftp.retrbinary("RETR {0}".format(remote), open(temp_path, "wb").write)
        with open(temp_path, 'rb') as file:
            data = file.read()
        md5 = hashlib.md5(data).hexdigest()
        os.remove(temp_path)
        return md5
    except Exception as e:
        print(f"[读取异常] {remote} failed: {e}")
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
        print(f"[读取异常] {remote} failed: {e}")
        return ""


def ftp_file_exists(remote):  # ftp判断远端文件是否存在
    try:
        ftp.cwd('/')
        paths = ftp.nlst(os.path.dirname(remote))
        return remote in paths
    except:
        return False


def ftp_dir_exists(remote):  # ftp判断远端文件夹是否存在
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
        print(f"[上传文件] {remote} uploaded successfully ...")
        return 0
    except Exception as e:
        print(f"[上传异常] {remote} failed: {e}")
        return -1


def ftp_create_dir(remote):  # ftp创建远端文件夹
    names = remote.replace('\\', '/').split('/')
    template = ""
    ftp.cwd('/')
    for dirname in names:
        try:
            template = os.path.join(template, dirname).replace('\\', '/')
            if template == "":
                continue
            if not ftp_dir_exists(template):
                ftp.mkd(template)
                print(f"[创建目录] {template} created successfully ...")
        except Exception as e:
            print(f"[创建异常] {template} failed: {e}")
            return -1
    return 0


def ftp_upload_dir(remote, location):  # ftp上传文件夹
    location = location.replace('\\', '/').rstrip('/')
    if not os.path.exists(location):
        return -1

    for root, dirs, files in os.walk(location):
        for file in files:
            os.chmod(os.path.join(root, file), 0o777)

    remote = remote.replace('\\', '/').rstrip('/')
    if not ftp_dir_exists(remote):
        if ftp_create_dir(remote) != 0:
            return -1

    ftp.cwd('/')
    for item in os.listdir(location):  # 遍历本地文件夹中的所有文件和子文件夹
        local_item = os.path.join(location, item).replace('\\', '/')
        remote_item = os.path.join(remote, item).replace('\\', '/')
        try:
            if os.path.isfile(local_item):
                ftp_upload_file(remote_item, local_item)  # 如果是文件，上传文件到FTP服务器
            elif os.path.isdir(local_item):
                ftp_upload_dir(remote_item, local_item)  # 如果是文件夹，递归上传文件夹
            print(f"[上传文件] {remote_item} uploading successfully ...")
        except Exception as e:
            sys.stdout.flush()
            print(f"[上传异常] {remote_item} failed: {e}")
            return -1
    sys.stdout.write(f"\r[上传状态] 完成\n")
    sys.stdout.flush()
    return 0


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


start_time = time.time()  # 记录起始时间
args = parse_arguments()

exitCode = 0
if args.isLatest:
    args.version = "Latest"

if args.version == "":
    print("版本号不能为空")
    sys.exit(-1)

print("[开始连接] {0} (user:{1}|passwd:{2}|timeout:{3}|encoding:{4})".format(
    args.remoteRoot, args.username, args.password, args.timeout, "utf-8"))
ftp = FTP(args.remoteRoot, user=args.username, passwd=args.password, timeout=args.timeout, encoding="utf-8")

_Remote = "{0}/{1}/{2}/{3}".format(
    args.remoteRelative, args.buildTarget, args.packageName, args.version)
_RemoteManifest = "{0}/Manifest.json".format(_Remote)
_Local = "{0}/{1}/{2}/{3}".format(
    args.localPath, args.buildTarget, args.packageName, args.version).replace('\\', '/')
_LocalManifest = "{0}/Manifest.json".format(_Local)

enter_handler()
print("[清单状态] {0} 获取中".format(_RemoteManifest))
if ftp_file_exists(_RemoteManifest):
    _RemoteManifestMD5 = str(ftp_get_file_md5(_RemoteManifest)).strip().lower()
    _LocalManifestMD5 = str(local_calculate_md5(_LocalManifest)).strip().lower()
    print("[比较清单] MD5 (远端:{0}) {2} (本地:{1})".format(
        _RemoteManifestMD5, _LocalManifestMD5, (_RemoteManifestMD5 == _LocalManifestMD5 and "==" or "!=")))
    if _RemoteManifestMD5 == _LocalManifestMD5:
        print("[清单一致] 无需更新")
        exit_handler()
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
        print("[清单一致] 清单对比后 无需更新")
        exit_handler()
        pass

    temp_dir = os.path.join(tempfile.mktemp(), args.version)
    if not os.path.exists(temp_dir):
        os.makedirs(temp_dir, exist_ok=True)
    if len(add_files) > 0:
        print("[新增列表] - {0}".format(len(add_files)))
        for key, value in add_files.items():
            remoteManifest[key] = value
            locationTemp = "{0}/{1}".format(_Local, key)
            if os.path.exists(locationTemp):
                local_copy_file(locationTemp, "{0}/{1}".format(temp_dir, key))
                print("{0} : {1}".format(value, key))
            else:
                print("目标新增文件不存在 : {0} 目标源结构被篡改 请重新构建资源".format(locationTemp))

    if len(change_files) > 0:
        print("[修改列表] - {0}".format(len(change_files)))
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
        print("[删除列表] - {0}".format(len(delete_files)))
        for key, value in delete_files.items():
            if remoteManifest.get(key):
                del remoteManifest[key]
                print("{0} : {1}".format(value, key))
            else:
                print("目标删除文件不存在 : {0} 目标源结构被篡改 请重新构建资源".format(key))

    if len(delete_files.keys()) > 0:
        table = []
        for key, value in delete_files.items():
            table.append("{0}/{1}".format(_Remote, key))
        ftp_delete_files(table)

    if len(add_files.keys()) > 0:
        exitCode = ftp_upload_dir(_Remote, temp_dir)
        local_delete_dir(temp_dir)
        if exitCode != 0:
            print("[新增修改] 任务失败")
            sys.exit(exitCode)

    print("[更新清单] {0}".format(_RemoteManifest))
    # json 序列化的字典格式 不压缩 并且按照key排序 key排序忽略大小写
    remoteManifestStr = json.dumps(remoteManifest, indent=4, separators=(',', ': '), sort_keys=True,
                                   ensure_ascii=False)
    temp_file = tempfile.mktemp()
    with open(temp_file, "w") as f:
        f.write(remoteManifestStr)
    exitCode = ftp_upload_file(_RemoteManifest, temp_file)
    local_delete_file(temp_file)
else:
    print("[清单状态] {0} 不存在".format(_RemoteManifest))
    exitCode = ftp_upload_dir(_Remote, _Local)

if exitCode == 0:
    exitCode = ftp_update_version()
exit_handler()
