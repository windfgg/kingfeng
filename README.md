# KingFeng

技术栈 `vue 2` `asp.net core` `docker` 

本项目后端使用OpenAPI

[docker image](https://hub.docker.com/r/ranqi03/kingfeng)

## 说明
KingFeng 专注于用户无感体验 不会添加用户一对一推送

KingFeng 仅供学习参考使用，请于下载后的 24 小时内删除，本人不对使用过程中出现的任何问题负责，包括但不限于 `数据丢失` `数据泄露`。

KingFeng 仅支持 qinglong 2.9+

不提供 `技术上的任何帮助`

[TG 频道](https://t.me/joinchat/H3etBWYzLKpiMWVl)    [TG 群组](https://t.me/joinchat/XV2AZcvzFIUxNjI9)
## 特性
- [x] docker一键部署
- [x] 支持wsck ptkey
- [x] 用户添加/更新cookies 添加备注 
- [x] 用户添加cookies 自动执行wskey转换任务
- [x] 推送卡片
- [x] 管理员登录
- [x] 多节点支持
- [ ] 用户日资产卡片 用户上线/下线推送
- [ ] 环境变量导出/恢复
- [ ] 各种助力脚本执行
- [ ] 自建推送日志数据库
- [ ] 用户wskey管理

### 插件
[苹果捷径获取pinck](https://www.icloud.com/shortcuts/f6046f1e79ad4ee6bcca6d2b078bd25a)
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
#管理员密钥 会自动生成
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
#管理员密钥 会自动生成
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
有多种部署方法 请自行研究 下面只提供一种

### 第一次部署
![KingFeng](https://i0.hdslb.com/bfs/album/d5e1df6f75e7835b699bdda295bbff4a4dce5a81.png)

```docker
docker pull ranqi03/kingfeng:latest

docker run -dit \
   -v $PWD/kingfeng/:/app/config/ \
   -p 5000:80 \
   --name kingfeng \
   --hostname kingfeng \
   ranqi03/kingfeng:latest
```
配置docker映射目录下的config.yaml

### 更新
```docker
docker kill kingfeng && docker rm kingfeng

docker pull ranqi03/kingfeng:latest

docker run -dit \
   -v $PWD/kingfeng/:/app/config/ \
   -p 5000:80 \
   --name kingfeng \
   --hostname kingfeng \
   ranqi03/kingfeng:latest
```
## 常见问题
问：是否支持内网端口？  
答：支持,请百度docker容器互通网络。

问：为什么访问不了？  
答：一般为端口映射错误/失败，请自行检查配置文件。

问：是否支持N1 Arm架构？  
答：暂不支持。
