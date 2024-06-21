import json
import os
import shutil
import subprocess
import time

import requests
from tqdm import tqdm


def get_latest_github_tag(repo_url) -> str:
    # 从仓库 URL 提取用户和仓库名称
    repo_path = repo_url.split("https://github.com/")[1].replace(".git", "")
    print(f"Fetching tags from {repo_path}")
    api_url = f"https://api.github.com/repos/{repo_path}/tags"

    response = requests.get(api_url, timeout=10)

    if response.status_code == 200:
        tags = response.json()
        if tags:
            latest_tag = tags[0]['name']
            print(f"Latest tag: {latest_tag}")
            return latest_tag
        else:
            print("No tags found.")
            return None
    else:
        print(f"Failed to fetch tags: {response.status_code}")
        return None


def get_local_tags() -> set[str]:
    result = subprocess.run(['git', 'tag'], stdout=subprocess.PIPE)
    tags = result.stdout.decode('utf-8').split()
    return set(tags)


def get_remote_tags(remote_name='origin') -> set[str]:
    result = subprocess.run(['git', 'ls-remote', '--tags', remote_name], stdout=subprocess.PIPE)
    remote_tags_output = result.stdout.decode('utf-8')
    remote_tags = set()
    for line in remote_tags_output.strip().split('\n'):
        if line:
            tag_ref = line.split()[1]
            if tag_ref.startswith('refs/tags/'):
                remote_tags.add(tag_ref[len('refs/tags/'):])
    return remote_tags


def delete_local_tag(tag) -> None:
    subprocess.run(['git', 'tag', '-d', tag], check=True)
    print(f"Deleted local tag {tag}")


def delete_remote_tag() -> None:
    subprocess.run(['git', 'fetch', '--prune', 'origin', '+refs/tags/*:refs/tags/*'], check=True)
    local_tags = get_local_tags()
    remote_tags = get_remote_tags()
    tags_to_delete = local_tags - remote_tags

    for tag in tags_to_delete:
        delete_local_tag(tag)


def remove_readonly(func, path, _) -> None:
    # os.chmod(path, stat.S_IWRITE)
    subprocess.run(['attrib', '-R', path], shell=True)
    func(path)


def delete_folder(folder_path) -> None:
    try:
        if os.path.exists(folder_path):
            shutil.rmtree(folder_path, onerror=remove_readonly)
        else:
            print(f"文件夹 '{folder_path}' 不存在。")
    except Exception as exp:
        print(f"文件夹 '{folder_path}' 删除失败。")
        print(exp)


def read_current_branch() -> str:  # 判断当前分支
    branches = os.popen("git branch").read().split("\n")
    for branch in branches:
        if branch.startswith("*"):
            return branch.replace("* ", "")


def read_local_username() -> str:
    result = subprocess.run(['git', 'config', 'user.name'], stdout=subprocess.PIPE)
    return result.stdout.decode('utf-8').strip()


def read_local_email() -> str:
    result = subprocess.run(['git', 'config', 'user.email'], stdout=subprocess.PIPE)
    return result.stdout.decode('utf-8').strip()


def read_current_version() -> str:
    try:
        subprocess.run(['git', 'fetch', '--tags'], check=True)
        tags = os.popen("git tag").read().split("\n")
        if len(tags) == 0:  # 需要判断是否有标签列表
            return None
        # 所有标签去掉空字符串 -preview标签去掉preview 然后按照version排序 1.2.3-preview -> 1.3.0-preview
        tags = sorted([tag.replace("-preview", "") for tag in tags if tag], key=lambda x: list(map(int, x.split("."))))
        return tags[-1]
    except Exception:
        return None


# 切换上一级目录
os.chdir(os.path.dirname(os.path.dirname(os.path.realpath(__file__))))
current_path = os.getcwd()
print("当前路径: " + current_path)

username = read_local_username()
email = read_local_email()
print("用户名称: " + username)
print("用户邮箱: " + email)

# 是否为preview版本
is_preview = True

# 忽略列表
ignore_list = [
    ".chglog/*",
    "*.yaml",
    "*.yml",
    ".github/API_USAGE/*",
    ".github/ISSUE_TEMPLATE/*",
    ".github/PULL_REQUEST_TEMPLATE/*",
    ".github/Template/*",
    ".github/workflows/*",
    ".github/*.py",
    ".github/*.sh",
    ".github/*.bat",
]

github = os.popen("git remote get-url origin").read().strip()
print("仓库地址: " + github)
current_branch = read_current_branch()  # 读取当前分支
print("仓库分支: " + current_branch)

steps = [
    ("删除不存在的标签", lambda: delete_remote_tag()),
]
for step_description, step_function in tqdm(steps, desc="检查标签"):
    step_function()

version = read_current_version()  # 读取当前版本号
if version is None:
    version = "1.0.0" + ("-preview" if is_preview else "")
    new_version = "1.0.0" + ("-preview" if is_preview else "")
else:
    # 递增版本号
    version_list = version.split(".")
    if is_preview:
        version_list[2] = str(int(version_list[2]) + 1) + "-preview"
    else:
        version_list[2] = str(int(version_list[2]) + 1)
    new_version = ".".join(version_list)
print("版本号: {0} -> {1}".format(version, new_version))

# 写入新版本号
with open("package.json", "r+") as f:
    package = json.load(f)
    current_version = package["version"]
    package["version"] = new_version
    package["relatedPackages"]["com.aio.package"] = get_latest_github_tag('https://github.com/AIO-GAME/Common.git')
    package["relatedPackages"]["com.aio.runner"] = get_latest_github_tag('https://github.com/AIO-GAME/Unity.Runner.git')
    f.seek(0)
    f.truncate()
    json.dump(package, f, indent=2)
    print("写入配置: 版本号 {0} -> {1}".format(current_version, new_version))
    f.close()

# 上传到远程仓库 捕获异常
if current_version != new_version:
    try:
        subprocess.run(['git', 'pull', 'origin', current_branch], check=True)
        subprocess.run(['git', 'add', 'package.json'], check=True)
        subprocess.run(['git', 'commit', '-m', f"✨ up version {current_branch} -> {new_version}"], check=True)
        subprocess.run(['git', 'push', 'origin', current_branch], check=True)
        print("推送仓库: ({0})成功".format(current_branch))
    except Exception as e:
        print("推送仓库: ({0})失败".format(current_branch))
        print(e)

steps = [
    ("设置用户名",
     lambda: subprocess.run(['git', 'config', 'user.name', username], check=True, stdout=-3, stderr=-3)),
    ("设置邮箱",
     lambda: subprocess.run(['git', 'config', 'user.email', email], check=True, stdout=-3, stderr=-3)),
    ("开启GPG签名",
     lambda: subprocess.run(['git', 'config', 'commit.gpgSign', 'true'], check=True, stdout=-3, stderr=-3)),
]
for step_description, step_function in tqdm(steps, desc="设置环境"):
    step_function()
# 克隆指定分支 到目标文件夹路径
os.chdir(os.path.dirname(current_path))
new_branch_path = os.path.join(os.path.dirname(current_path), new_version)
new_branch = "release/{0}_{1}".format(new_version, str(int(time.time())))

steps = []
if os.path.exists(new_branch_path) is False:
    steps.append(("克隆指定分支",
                  lambda: subprocess.run(['git', 'clone', github, '-b', current_branch, '--single-branch', new_branch_path], check=True, stdout=-3, stderr=-3)))

# 切换环境变量路径 为指定分支路径
steps.append(("切换路径", lambda: os.chdir(new_branch_path)))
steps.append(("重置分支", lambda: subprocess.run(['git', 'reset', '--hard'], check=True, stdout=-3, stderr=-3)))
steps.append(("拉取分支", lambda: subprocess.run(['git', 'pull'], check=True, stdout=-3, stderr=-3)))
steps.append(("切换分支", lambda: subprocess.run(['git', 'checkout', '-b', new_branch], check=True, stdout=-3, stderr=-3)))
for step_description, step_function in tqdm(steps, desc="创建分支", total=len(steps)):
    step_function()
print("创建分支: {0}".format(new_branch))

# 在新的分支上忽略指定文件和文件夹 如果没有则创建 如果有则拼接
with open(os.path.join(new_branch_path, ".gitignore"), "a+") as f:
    for ignore in ignore_list:
        if ignore.startswith("*"):
            f.write(ignore + "\n")
        else:
            f.write("/" + ignore + "\n")
    print("修改成功: .gitignore ")

# 删除指定文件和文件夹
errorList = []
for ignore in tqdm(ignore_list, desc="删除列表"):
    try:
        subprocess.run(['git', 'rm', '-r', '--cached', ignore], check=True, stdout=-3, stderr=-3)
        subprocess.run(['git', 'clean', '-fdx', ignore], check=True, stdout=-3, stderr=-3)
    except subprocess.CalledProcessError as e:
        errorList.append(ignore)

if len(errorList) > 0:
    for error in errorList:
        print("删除失败: " + error)
else:
    print("删除成功")

# 写入新版本号
with open("package.json", "r+") as f:
    package = json.load(f)
    package["type"] = "module"
    f.seek(0)
    f.truncate()
    json.dump(package, f, indent=2)
    f.close()

steps = [
    ("删除标签", lambda: delete_remote_tag()),
    ("设置用户名",
     lambda: subprocess.run(['git', 'config', 'user.name', username], check=True, stdout=-3, stderr=-3)),
    ("设置邮箱",
     lambda: subprocess.run(['git', 'config', 'user.email', email], check=True, stdout=-3, stderr=-3)),
    ("开启签名",
     lambda: subprocess.run(['git', 'config', 'commit.gpgSign', 'true'], check=True, stdout=-3, stderr=-3)),
    ("添加文件",
     lambda: subprocess.run(['git', 'add', '.'], check=True, stdout=-3, stderr=-3)),
    ("提交文件",
     lambda: subprocess.run(['git', 'commit', '-s', '-m', f"✨ up version {current_version} -> {new_version}"], check=True, stdout=-3, stderr=-3)),
    ("推送分支",
     lambda: subprocess.run(['git', 'push', 'origin', new_branch], check=True, stdout=-3, stderr=-3)),
    ("创建标签",
     lambda: subprocess.run(['git', 'tag', '-s', new_version, '-m', f"✨ up version {current_version} -> {new_version}"], check=True, stdout=-3, stderr=-3)),
    ("推送标签",
     lambda: subprocess.run(['git', 'push', 'origin', new_version], check=True, stdout=-3, stderr=-3)),
    ("删除分支",
     lambda: subprocess.run(['git', 'push', 'origin', '--delete', new_branch], check=True, stdout=-3, stderr=-3)),
    ("切换路径", lambda: os.chdir(current_path)),
    ("删除目标", lambda: delete_folder(new_branch_path)),
]
for step_description, step_function in tqdm(steps, desc="上传标签"):
    step_function()

print("升级标签版本成功")