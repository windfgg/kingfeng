# KingFeng

技术栈 `vue 2` `asp.net core` `docker` 

[苹果捷径获取pinck](https://www.icloud.com/shortcuts/f6046f1e79ad4ee6bcca6d2b078bd25a)

[docker image](https://hub.docker.com/r/ranqi03/kingfeng)

## 说明

本项目使用后端OpenAPI 地址:http://localhost:5000/swagger/index.html

KingFeng 仅供学习参考使用，请于下载后的 24 小时内删除，本人不对使用过程中出现的任何问题负责，包括但不限于 `数据丢失` `数据泄露`。

KingFeng 仅支持 qinglong 2.9+

不提供 `技术上的任何帮助`

[TG 频道](https://t.me/joinchat/H3etBWYzLKpiMWVl)    [TG 群组](https://t.me/joinchat/XV2AZcvzFIUxNjI9)
## 特性
- [x] 支持wsck ptkey
- [x] docker一键部署
- [x] 添加/更新cookies 添加备注 自动执行wskey任务
- [x] 管理员登录 执行任务/任务日志查看
- [x] 扫码推送卡片
- [ ] 多容器添加ck
- [ ] 环境变量导出/恢复
- [ ] 各种助力脚本执行
- [ ] 自建推送日志数据库
- [ ] 用户wskey管理

### 配置文件

配置文件第一次部署后端会自动生成
配置文件所有项必填 如不填(无法预知的后果)

`backend` 地址为docker映射的目录下的config.yaml
```yaml
#青龙地址 默认为空 必须以/结尾
QL_URL: 
#青龙OpenAPI Client_ID
QL_Client_ID: 
#青龙OpenAPI Client_Secret
QL_Client_Secret: 
#管理员密钥 第一次启动会自动生成 登录页面直接输入即可登录管理员
SecretKey: 
#wskey定时任务的 任务名 
WsKeyTaskFullName: wskey转换
# 首页的抓取wskey教程链接 默认 http://www.baidu.com
Course: http://www.baidu.com
# 推送图片图床地址
PushImageUrl:
```
### wskey转换库
[Zy143L](https://github.com/Zy143L/wskey)

请按照使用文档正确拉取 wskey转换库

## 项目指南
有多种部署方法 请自行研究 下面只提供一种

![KingFeng](https://i0.hdslb.com/bfs/album/d5e1df6f75e7835b699bdda295bbff4a4dce5a81.png)

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

配置docker映射目录下的config.yaml

<!-- ## 常见问题 -->
