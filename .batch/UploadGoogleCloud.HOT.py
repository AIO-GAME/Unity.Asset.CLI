# -*- coding: UTF-8 -*-

import argparse
import base64
import hashlib
import hmac
import json
import os
import subprocess
import time
import urllib.parse
import urllib.request
from typing import List

import numpy as np


def parse_arguments():  # 获取传入的参数
    parser = argparse.ArgumentParser(description='上传谷歌云资源')
    parser.add_argument('-l', '--localPath', type=str, required=True, help='本地工程路径', default='')
    parser.add_argument('-rs', '--serverRoot', type=str, required=True, help='服务器', default='')
    parser.add_argument('-p', '--package', type=str, required=True, help='资源包', default='', nargs='*')
    parser.add_argument('-t', '--buildTarget', type=str, required=True, help='目标平台',
                        choices=["Android", "iOS", "StandaloneWindows64", "WebGL", "StandaloneOSX"])
    parser.add_argument('-v', '--version', type=str, required=False, help='版本号', default="Latest")
    parser.add_argument('-token', '--token', type=str, required=True, help='钉钉token')
    parser.add_argument('-secret', '--secret', type=str, required=True, help='钉钉安全密钥')
    return parser.parse_args()


def local_get_folder_size(folder_path):
    total_size = 0
    # 遍历文件夹中的所有文件和子文件夹
    for dir_path, dir_names, filenames in os.walk(folder_path):
        for filename in filenames:
            file_path = os.path.join(dir_path, filename)
            # 获取文件大小并累加到总大小
            total_size += os.path.getsize(file_path)
    return total_size


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


def notification(content: dict):
    import ssl
    dd_url = f'https://oapi.dingtalk.com/robot/send?access_token={args.token}'
    if args.secret:
        timestamp = str(round(time.time() * 1000))
        secret_enc = args.secret.encode('utf-8')
        string_to_sign = f'{timestamp}\n{args.secret}'
        string_to_sign_enc = string_to_sign.encode('utf-8')
        hmac_code = hmac.new(secret_enc, string_to_sign_enc, digestmod=hashlib.sha256).digest()
        sign = urllib.parse.quote_plus(base64.b64encode(hmac_code))
        dd_url = f'{dd_url}&timestamp={timestamp}&sign={sign}'
    # 忽略证书验证
    ssl._create_default_https_context = ssl._create_unverified_context

    data_str = json.dumps(content,
                          indent=4,
                          separators=(',', ': '),
                          sort_keys=True,
                          ensure_ascii=False,
                          cls=None)
    data = data_str.encode('utf-8')
    headers = {'Content-Type': 'application/json'}
    method = 'POST'
    req = urllib.request.Request(dd_url, data=data, headers=headers, method=method)
    with urllib.request.urlopen(req) as response:
        result = eval(response.read().decode('utf-8'))
        if 'errcode' in result and result['errcode'] != 0:
            exitCode = result['errcode']
            print(f'[钉钉] 消息发送失败 返回结果：{exitCode}')


def notification_markdown(content: str, at_mobiles: List[str] = None):
    data = {
        "msgtype": "markdown",
        "markdown": {
            "title": task_name,
            "text": content
        },
        "at": {
            "atMobiles": at_mobiles,
            "isAtAll": List is not None and np.size(at_mobiles) == 0
        }
    }
    notification(data)


def notification_text(content: str, at_mobiles: List[str] = None):
    data = {
        "msgtype": "text",
        "text": {
            "content": content
        },
        "at": {
            "atMobiles": at_mobiles,
            "isAtAll": List is not None and np.size(at_mobiles) == 0
        }
    }
    notification(data)


start_time = time.time()  # 记录起始时间
task_name = "资源上传 : 谷歌云"
args = parse_arguments()
exitCode = 0
# 获取当前文件所在目录
current_path = os.path.dirname(os.path.abspath(__file__))

if args.token == "":
    print("钉钉 token 不能为空")
    exit(1)

if args.secret == "":
    print("钉钉 secret 不能为空")
    exit(1)

error_list = []

if args.buildTarget == "":
    error_list.append('目标平台 参数为空')

if os.path.exists(args.localPath) is False:
    error_list.append(f'本地路径 不存在')
    error_list.append(f'>`{args.localPath}`')

if args.serverRoot == "":
    error_list.append('服务器 参数为空')

if args.package:
    for p in args.package:
        path = os.path.join(args.localPath, args.buildTarget, p, args.version)
        if os.path.exists(path) is False:
            error_list.append(f'[`{p}`] 目标路径不存在')
            error_list.append(f'>`{path}`')
else:
    error_list.append('资源包 参数为空')

if len(error_list) > 0:
    error_str = f'{task_name} 参数异常 \n'
    for e in error_list:
        error_str += f' > {e} \n'
    notification_markdown(error_str)
    exit(1)

task_list_info = []
all_start_time = time.time()
task_start_info = f'{task_name} {str(args.serverRoot).upper()} {args.buildTarget} {args.version.upper()} \n'
task_start_info += f'\n >提示 : 当前进入{str(args.serverRoot).upper()}服可能出现异常\n'
for p in args.package:
    path = str(os.path.join(args.localPath, args.buildTarget, p, args.version))
    task_start_info += f'\n >资源包 {p}\n'
    task_start_info += f'\n >>资源大小 : {local_get_folder_size(path) / 1024 / 1024:.2f}MB\n'
    task_start_info += f'\n >>资源数量 : {len(os.listdir(path))}个\n'
notification_markdown(task_start_info)

all_size = 0
all_num = 0
for p in args.package:
    start_time = time.time()

    path = str(os.path.join(args.localPath, args.buildTarget, p, args.version))
    size = local_get_folder_size(path)
    all_size += size
    num = len(os.listdir(path))
    all_num += num

    task_list_info.append(f"资源包 {p}")
    task_list_info.append(f">资源大小 : {size / 1024 / 1024:.2f}MB\n")
    task_list_info.append(f">资源数量 : {num}个\n")

    if os.name == 'nt':
        exe = "python"
    else:
        exe = "python3"
    command = (
        f'{exe} "{current_path}/UploadGoogleCloud.YooAsset.py" -p "{p}" -l "{args.localPath}" -r "rol-files/{args.serverRoot}"'
        f' -v {args.version} -t {args.buildTarget} -mk "cache-control" -mv "public, max-age=604800"'
        f' -rp "project-rise-of-legend"')
    print("[执行命令] {0}".format(command))
    # process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, text=True, bufsize=4)
    try:  # 开启子进程执行命令 需要实时输出日志
        # process.communicate()  # 等待进程结束
        process_temp = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, text=True, bufsize=1)
        while process_temp.poll() is None:
            output = process_temp.stdout.readline()
            if output:
                print(output.strip())
    except Exception as e:
        task_list_info.append(f">异常信息 : {e} \n")
        task_list_info.append(f">任务状态 : 失败 {e} \n")
        task_list_info.append(f">任务耗时 : {time.time() - start_time:.2f}秒\n")
        task_list_info.append(f">上传速度 : {size / (time.time() - start_time) / 1024 / 1024:.2f}MB/s\n")
        pass
    task_list_info.append(f">结束时间 : {time.strftime('%Y-%m-%d %H:%M:%S', time.localtime())}\n")
    task_list_info.append(f">任务状态 : 完成\n")
    task_list_info.append(f">任务耗时 : {time.time() - start_time:.2f}秒\n")
    task_list_info.append(f">上传速度 : {size / (time.time() - start_time) / 1024 / 1024:.2f}MB/s\n")
    continue

gcloud_update_meta(f"rol-files/{args.serverRoot}/Version/{args.buildTarget}.json", "cache-control", "no-cache")
# gcloud_clean_cache("project-rise-of-legend", args.serverRoot, True)

all_size_str = f'{all_size / 1024 / 1024:.2f}MB'
all_num_str = f'{all_num}个'
all_time_str = f'{time.time() - all_start_time:.2f}秒'
task_list_info_str = (f'{task_name} {str(args.serverRoot).upper()} {args.buildTarget} {args.version.upper()}]'
                      f'\n>[大小:{all_size_str}] [数量:{all_num_str}] [耗时:{all_time_str}]\n\n')
for info in task_list_info:
    task_list_info_str += f' >{info} \n'
notification_markdown(task_list_info_str)
