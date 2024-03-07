# -*- coding: UTF-8 -*-

import argparse
import base64
import hashlib
import hmac
import json
import sys
import time
import urllib.parse
import urllib.request


def parse_arguments():  # 获取传入的参数
    parser = argparse.ArgumentParser(description='钉钉发送消息')
    parser.add_argument('-t', '--access_token', type=str, required=True, help='钉钉群机器人的access_token')
    parser.add_argument('-m', '--msgtype', type=str, required=False, help='钉钉群机器人的access_token',
                        choices=['text', 'link', 'markdown', 'md', 'actionCard', 'feedCard', 'custom'], default='text')
    parser.add_argument('-c', '--content', type=str, required=True, help='消息内容')
    parser.add_argument('-atm', '--atMobiles', type=str, required=False, help='被@人的手机号', nargs='*')
    parser.add_argument('-atu', '--atUserIds', type=str, required=False, help='被@人的userid', nargs='*')
    parser.add_argument('-all', '--atAll', type=bool, required=False, help='是否@所有人', default=False, const=True,
                        nargs='?')
    parser.add_argument('-title', '--title', type=str, required=False, help='消息标题')
    parser.add_argument('-picUrl', '--picUrl', type=str, required=False, help='图片链接')
    parser.add_argument('-messageUrl', '--messageUrl', type=str, required=False, help='消息链接')
    # 加签
    parser.add_argument('-s', '--secret', type=str, required=False, help='加签密钥', default='')

    return parser.parse_args()


start_time = time.time()  # 记录起始时间
args = parse_arguments()
if not args.access_token:
    print('请传入钉钉群机器人的access_token')
    sys.exit(1)

if not args.content:
    print('请传入消息内容')
    sys.exit(1)

exitCode = 0
dd_url = f'https://oapi.dingtalk.com/robot/send?access_token={args.access_token}'
if args.secret:
    timestamp = str(round(time.time() * 1000))
    secret_enc = args.secret.encode('utf-8')
    string_to_sign = f'{timestamp}\n{args.secret}'
    string_to_sign_enc = string_to_sign.encode('utf-8')
    hmac_code = hmac.new(secret_enc, string_to_sign_enc, digestmod=hashlib.sha256).digest()
    sign = urllib.parse.quote_plus(base64.b64encode(hmac_code))
    dd_url = f'{dd_url}&timestamp={timestamp}&sign={sign}'

msg_content = {"msgtype": args.msgtype}
if args.atAll:
    if 'at' not in msg_content:
        msg_content['at'] = {}
    msg_content['at']['isAtAll'] = True

if args.atMobiles:
    if 'at' not in msg_content:
        msg_content['at'] = {}
    msg_content['at']['atMobiles'] = args.atMobiles
    msg_content['at']['isAtAll'] = False

if args.atUserIds:
    if 'at' not in msg_content:
        msg_content['at'] = {}
    msg_content['at']['atUserIds'] = args.atUserIds
    msg_content['at']['isAtAll'] = False

if args.msgtype == 'link':
    msg_content['link'] = {
        "text": args.content,
        "title": args.title,
        "picUrl": args.picUrl,
        "messageUrl": args.messageUrl
    }
elif args.msgtype == 'text':
    msg_content['text'] = {"content": args.content}
elif args.msgtype == 'markdown' or args.msgtype == 'md':
    msg_content['msgtype'] = 'markdown'
    msg_content['markdown'] = {"title": args.title, "text": args.content}
elif args.msgtype == 'actionCard':
    msg_content['actionCard'] = {
        "title": args.title,
        "text": args.content,
        "hideAvatar": "0",
        "btnOrientation": "0",
        "singleTitle": "阅读全文",
        "singleURL": args.messageUrl
    }
elif args.msgtype == 'feedCard':
    msg_content['feedCard'] = {
        "links": [
            {
                "title": args.title,
                "messageURL": args.messageUrl,
                "picURL": args.picUrl
            }
        ]
    }
elif args.msgtype == 'custom':
    try:
        msg_content = json.loads(args.content)
    except Exception as e:
        print(f'自定义消息内容解析失败: {e}')
        sys.exit(1)
else:
    print('msgtype 参数错误')
    exitCode = 1
    sys.exit(exitCode)

try:
    import ssl

    # 忽略证书验证
    ssl._create_default_https_context = ssl._create_unverified_context

    data_str = json.dumps(msg_content,
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
            print(f'[钉钉] 消息发送失败 返回结果：{result}')
            sys.exit(exitCode)
        else:
            exitCode = 0
            sys.exit(exitCode)
except Exception as e:
    print(e)
    exitCode = 1
    sys.exit(exitCode)
