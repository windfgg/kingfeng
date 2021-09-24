# KingFeng
![预览图片](https://raw.githubusercontent.com/WindFgg/kingfeng/main/Preview.png)
<p align="center">
    <a href="https://github.com/WindFgg/kingfeng"><img src="https://img.shields.io/pypi/l/dailycheckin?style=popout-square" alt="license"></a>
    <a href="https://github.com/WindFgg/kingfeng"><img src="https://img.shields.io/github/stars/WindFgg/kingfeng.svg?style=popout-square" alt="GitHub stars"></a>
    <a href="https://github.com/WindFgg/kingfeng"><img src="https://img.shields.io/github/forks/WindFgg/kingfeng.svg?style=popout-square" alt="GitHub forks"></a>
    <a href="https://hub.docker.com/r/ranqi03/kingfeng"><img src="https://img.shields.io/docker/pulls/ranqi03/kingfeng?style=popout-square" alt="Docker Pulls"></a>
    <a href="https://hub.docker.com/r/ranqi03/kingfeng"><img src="https://img.shields.io/docker/image-size/ranqi03/kingfeng?style=popout-square" alt="Docker Size"></a>
    <a href="https://hub.docker.com/r/ranqi03/kingfeng"><img src="https://img.shields.io/docker/stars/ranqi03/kingfeng?style=popout-square" alt="Docker Stars"></a>
</p>

技术栈 `vue 2` `asp.net core` `docker` 

本项目后端使用Swagger UI 生成的 OpenAPI文档

## 说明
KingFeng 专注于用户无感体验 不会添加用户一对一推送

KingFeng 仅供学习参考使用，请于下载后的 24 小时内删除，本人不对使用过程中出现的任何问题负责，包括但不限于 `数据丢失` `数据泄露`。

KingFeng 仅支持 qinglong 2.9+

KingFeng不支持任何arm架构设备

本项目 不提供 `技术上的任何帮助`

本项目 **暂停更新一段时间**

[TG 频道](https://t.me/joinchat/H3etBWYzLKpiMWVl)    [TG 群组](https://t.me/joinchat/XV2AZcvzFIUxNjI9)
## 特性
- [x] docker一键部署
- [x] 支持wsck ptkey
- [x] 用户添加/更新cookies 检测是否过期 添加备注 
- [x] 用户添加cookies 自动执行wskey转换任务
- [x] 推送卡片 自定义公告 支持html语法
- [x] 管理员在线登录修改配置
- [x] 多节点支持
- [ ] 用户日资产卡片 用户上线/下线推送
- [ ] 环境变量导出/恢复
- [ ] 各种助力脚本执行
- [ ] 自建推送日志数据库
- [ ] 用户wskey管理

### 插件
[苹果捷径获取pinck 需要国外Apple Id安装ScriptsTable](https://www.icloud.com/shortcuts/f6046f1e79ad4ee6bcca6d2b078bd25a)
### 配置文件
配置文件第一次部署后端会自动生成
配置文件所有项必填 如不填(**无法预知的后果**)
配置文件地址为docker映射的目录下的config.yaml
**QL_URL为你的青龙地址**
#### 单节点配置
```yaml
Servers: 
  #显示的名称
- QL_Name: 广州节点1 
  #青龙
  QL_URL: http://localhost:5710/ 
  #OpenAPI Client_ID
  QL_Client_ID: b5lTVasddL_4Z_5zxxc123c 
  #OpenAPI Client_Secret
  QL_Client_Secret: LWasdpB4axklasdasdasd123Fr1i0O_ZMc 
  #最大添加ck容量 当前有的ck按照名称来统计 只要名称为JD_WSCK或者JD_COOKIE就会计数
  MaxCount: 100 
#管理员密钥 会自动生成 修改规则为必须包涵大小写字母+数字
SecretKey: Hcw022703 
#wskey转换任务名
WsKeyTaskFullName: wskey转换 
#用户自定义公告内容 支持html语法
Notice: 你好,这里可以自定义公告 
#管理员名称 
UserName: QiFengg
#推送图片
PushImageUrl: https://img2.baidu.com/it/u=1007188585,453085648&fm=26&fmt=auto&gp=0.jpg 
```

#### 多节点配置
```yaml
Servers: 
  #显示的名称
- QL_Name: 广州节点1 
  #青龙
  QL_URL: http://localhost:5710/ 
  #OpenAPI Client_ID
  QL_Client_ID: b5lTVasddL_4Z_5zxxc123c 
  #OpenAPI Client_Secret
  QL_Client_Secret: LWasdpB4axklasdasdasd123Fr1i0O_ZMc 
  #最大添加ck容量 当前有的ck按照名称来统计 只要名称为JD_WSCK或者JD_COOKIE就会计数
  MaxCount: 100 
- QL_Name: 广州节点2
  QL_URL: http://localhost:8710/
  QL_Client_ID: b5lTVasdasdL_4Z_xczxc123c
  QL_Client_Secret: LWOsdpB4axklasdaasdasdsdd1FrMc
  MaxCount: 100
#管理员密钥 会自动生成 修改规则为必须包涵大小写字母+数字
SecretKey: Hcw022703 
#wskey转换任务名
WsKeyTaskFullName: wskey转换 
#用户自定义公告内容 支持html语法
Notice: 你好,这里可以自定义公告 
#管理员名称 
UserName: QiFengg
#推送图片
PushImageUrl: https://img2.baidu.com/it/u=1007188585,453085648&fm=26&fmt=auto&gp=0.jpg 
```

### wskey转换库
[Zy143L](https://github.com/Zy143L/wskey)

请按照使用文档正确拉取 wskey转换库

## 项目指南
有多种部署方式 下面只提供一种docker部署

**Arm暂不支持 可以不用试了**

### 第一次部署
1. 在ssh执行:`docker exec -it 你的容器名称 bash`进入容器 `ql update`将更新青龙到最新 `ql check`检查青龙状态是否正确

2. 进入青龙的控制面板创建应用 应用名称`KingFeng` 权限 `环境变量` `定时任务`
![KingFeng](https://i0.hdslb.com/bfs/album/d5e1df6f75e7835b699bdda295bbff4a4dce5a81.png)

3. 复制粘贴到ssh执行下列命令 如出现无法拉取 请自行设置docker国内源
```docker
docker pull ranqi03/kingfeng:latest

docker run -dit \
   -v $PWD/kingfeng/:/app/config/ \
   -p 5000:80 \
   --name kingfeng \
   --hostname kingfeng \
   ranqi03/kingfeng:latest
```
4.配置docker映射目录下的config.yaml 默认地址 `/用户名/kingfeng/config.yaml`
- 查看容器IP命令 `docker inspect --format='{{.NetworkSettings.IPAddress}}' 容器名`
- 如QL_URL使用容器IP 青龙默认部署IP为`5700` 例如我容器IP是`172.13.1.33` 那我QL_URL就是`http://172.13.1.33:5700/`
- 管理员登录 选择任意节点 输入配置文件的密钥 登录即可
### 更新
```docker
docker kill kingfeng && docker rmi kingfeng

docker pull ranqi03/kingfeng:latest

docker run -dit \
   -v $PWD/kingfeng/:/app/config/ \
   -p 5000:80 \
   --name kingfeng \
   --hostname kingfeng \
   ranqi03/kingfeng:latest
```
<!-- ### 其他部署方式
我提供发布文件压缩包 有`linux-arm64` `liunx-arm` `liunx-x64` 可自行百度liunx安装.Net 运行时SDK 并尝试运行软件
[.Net RunTime SDK](https://dotnet.microsoft.com/download) 请下载.Net5.0
![](https://i0.hdslb.com/bfs/album/06d16311d2b8db23c295a3fc4a7a21033ac09cc3.png)
切换到软件根目录 执行后台运行前 请打开网页检查是否可以正常访问
**下列命令仅为参考**
```bash
chmod 777 KingFeng #给权限
./KingFeng #运行KingFeng
nohup ./KingFeng & #后台运行KingFeng

ps -ajx|grep JDC #查看KingFeng 进程ID 有两行的话默认是第二行第二列的ID
kill -9 进程ID #通过进程ID杀掉KingFeng 
``` -->
## 常见问题
Q：配置填写正确但是节点加载不出来
A：请检查服务器CPU是否爆高,如未爆高请在青龙容器内执行`ql update`以及`ql check` 具体内容请查看[项目指南](#第一次部署) 第1步

Q：是否支持内网端口？  
A：支持公网IP 域名 以及容器IP 推荐容器IP(安全性略高,速度稍微快)。

Q：为什么访问主页出现错误空提示？  
A：一般为端口映射错误/失败，请自行检查配置文件。

Q：是否支持N1 Arm架构？  
A：不支持。
