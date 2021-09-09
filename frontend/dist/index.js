const express = require("express");
//const compression = require("compression"); //gzip包
const fs = require("fs");

const app = express();
// app.use(compression()); // 启用gzip代码压缩
app.use(express.static("./public")); //托管静态资源
app.get("/*", function(req, res) {
  const html = fs.readFileSync("./public/index.html", "utf-8");
  res.send(html);
});

//.env 本地配置文件
const dotenv = require("dotenv");
dotenv.config();

//启动应用 默认端口为5710
const port = process.env.PORT || 5710;

console.log("Start KingFeng success! listening port: " + port);
app.listen(port);
