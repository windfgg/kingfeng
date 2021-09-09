const express = require("express");
//const compression = require("compression"); //gzip包
const fs = require("fs");

const app = express();
// app.use(compression()); // 启用gzip代码压缩
app.use(express.static("./public")); //托管静态资源
app.get("/*", function(req, res) {
  console.log("请求" + req);
  const html = fs.readFileSync("./public/index.html", "utf-8");
  console.log(html);
  res.send(html);
});

//.env 本地配置文件
const dotenv = require("dotenv");
dotenv.config();

//启动应用 默认端口为5800
const port = process.env.PORT;
app.listen(port);
console.log("Start KingFeng success! listening port: " + port);
