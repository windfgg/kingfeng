<template>
  <div class="content">
    <!--个人中心-->
    <div class="Card ant-card ant-card-bordered">
      <div class="ant-card-head">
        <div class="ant-card-head-wrapper">
          <a-icon type="crown" theme="twoTone" />
          <div class="ant-card-head-title">
            个人中心
          </div>
        </div>
      </div>
      <div class="ant-card-body">
        <br />
        <div>
          <p>昵称：{{ remarks }}</p>
          <p>添加时间：{{ timestamp }}</p>
          <p>
            状态：
            <a-icon :type="statuss" theme="twoTone" :two-tone-color="color" />{{
              status == 0 ? " 正常" : " 被禁用"
            }}
          </p>
        </div>
        <br />
        <a-input v-model="wskey" placeholder="请输入新的cookies" />
        <div>
          <br />
          <a-space>
            <a-button type="primary" shape="round" @click="updatewskey">
              更新cookies
            </a-button>
            <a-button type="danger" shape="round" @click="remove">
              删除账号
            </a-button>
          </a-space>
        </div>
      </div>
    </div>

    <!--推送卡片-->
    <div class="Card ant-card ant-card-bordered" v-show="pushIsShow">
      <div class="ant-card-head">
        <div class="ant-card-head-wrapper">
          <a-icon type="pushpin" theme="twoTone" />
          <div class="ant-card-head-title">
            扫码接收通知
          </div>
        </div>
      </div>

      <div class="ant-card-body">
        <a-spin :spinning="!pushLoding">
          <img class="img" :src="push" />
        </a-spin>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      pushIsShow: false,
      pushLoding: false,
      push: "",
      wskey: "",
      remarks: "",
      timestamp: undefined,
      status: 0,
    };
  },
  computed: {
    statuss: function() {
      return this.status == 0 ? "check-circle" : "close-circle";
    },
    color: function() {
      return this.status == 0 ? "#52c41a" : "#DC143C";
    },
  },
  created() {
    document.title = "KingFeng - 个人中心";
    const uid = localStorage.getItem("uid");
    if (!uid) {
      this.$router.push("/");
    }
    this.remarks = localStorage.getItem("name");
    this.timestamp = localStorage.getItem("timestamp");
  },
  mounted() {
    const uid = localStorage.getItem("uid");
    const address = localStorage.getItem("address");

    this.$http
      .get("api/exitst?uid=" + uid + "&ql_url=" + address)
      .then((response) => {
        if (response.data.code === 200) {
          let time = response.data.msg;
          this.time = this.dateFormat(response.data.msg);
          localStorage.setItem("timestamp", this.dateFormat(time));
          if (!response.data.data) {
            //如果被禁用 更改状态
            this.status = 1;
            this.$message.error("用户被管理员禁用,请联系管理员处理", 3);
          }
        } else {
          this.$message.error("用户被删除,请联系管理员处理", 3);
          this.delete();
          this.$router.push("/");
        }
      });

    this.getConfig();
  },
  methods: {
    //时间转换
    dateFormat(date) {
      let data = new Date(date);
      let y = data.getFullYear();
      let m = data.getMonth() + 1;
      m = m < 10 ? "0" + m : m;
      let d = data.getDate();
      d = d < 10 ? "0" + d : d;
      let h = data.getHours();
      h = h < 10 ? "0" + h : h;
      let M = data.getMinutes();
      M = M < 10 ? "0" + M : M;
      let s = data.getSeconds();
      s = s < 10 ? "0" + s : s;
      let dateTime = y + "-" + m + "-" + d + " " + h + ":" + M + ":" + s;
      return dateTime;
    },
    //删除账号
    remove() {
      const address = localStorage.getItem("address");
      const uid = localStorage.getItem("uid");
      this.$http
        .delete("api/deleteEnv?ql_url=" + address + "&uid=" + uid)
        .then((response) => {
          if (response.data.code == 200) {
            this.delete();
            this.$message.success("删除账号成功", 2);
            this.$router.push("/");
          } else {
            this.$message.error("删除失败,请联系管理员处理");
          }
        });
    },
    //更新环境变量
    updatewskey() {
      const address = localStorage.getItem("address");

      const pin =
        this.wskey.match(/pin=(.*?);/) && this.wskey.match(/pin=(.*?);/)[1];
      pin == decodeURIComponent(pin);
      const wskey =
        this.wskey.match(/wskey=(.*?);/) && this.wskey.match(/wskey=(.*?);/)[1];

      //判断pin格式
      const pt_key =
        this.wskey.match(/pt_key=(.*?);/) &&
        this.wskey.match(/pt_key=(.*?);/)[1];
      pin == decodeURIComponent(pin);
      const pt_pin =
        this.wskey.match(/pt_pin=(.*?);/) &&
        this.wskey.match(/pt_pin=(.*?);/)[1];

      if (pin && wskey) {
        this.$http
          .post(
            "api/updateEnv?ql_url=" +
              address +
              "&uid=" +
              localStorage.getItem("uid") +
              "&wskey=" +
              this.wskey
          )
          .then((response) => {
            if (response.data.code == 200) {
              this.wskey = "";
              this.$message.success("更新wskey成功", 2);
            } else {
              this.$message.error("更新失败,请联系管理员处理");
            }
          });
      } else if (pt_key && pt_pin) {
        //判断是否pinck
        this.$http
          .get("api/CheeckPinCk?pinck=" + this.wskey)
          .then((response) => {
            if (response.data.code != 200) {
              this.$message.error("pinck过期,请填写状态正常的pinck", 1.5);
              this.$set(this.isLogin, "loading", false);
              return;
            } else {
              this.$http
                .post(
                  "api/updateEnv?ql_url=" +
                    address +
                    "&uid=" +
                    localStorage.getItem("uid") +
                    "&wskey=" +
                    this.wskey
                )
                .then((response) => {
                  if (response.data.code == 200) {
                    this.wskey = "";
                    this.$message.success("更新pinck成功", 2);
                  } else {
                    this.$message.error("更新失败,请联系管理员处理");
                  }
                });
            }
          });
      }
    },
    checkPinck() {
      this.$http.get("api/").then((response) => {
        if ((response.data.data.code = 200)) {
          return false;
        } else {
          return true;
        }
      });
    },
    //获取配置文件
    getConfig() {
      this.$http.get("api/config").then(
        async (response) => {
          if (response.data.code === 200) {
            setTimeout(() => {}, 200);
            this.push = response.data.data.push;
            if (response.data.data.push != null) {
              this.pushIsShow = true;
            }
            this.pushLoding = !this.pushLoding;
          } else {
            this.$message.error("连接服务器错误,请稍后再试", 1.5);
          }
        },
        (response) => {
          response;
          this.$message.error(response.data.msg, 2);
        }
      );
    },
    //删除本地缓存
    delete() {
      localStorage.removeItem("uid");
      localStorage.removeItem("name");
      localStorage.removeItem("address");
    },
  },
};
</script>

<style>
.img {
  width: auto;
  height: auto;
  max-width: 100%;
  max-height: 100%;
}
.content {
  margin: auto;
  width: 91.666667%;
  max-width: 64rem;
}
.Card {
  margin: auto;
  margin-top: 1.25rem;
  border-radius: 0.5rem;
  --tw-bg-opacity: 1;
  background-color: rgba(255, 255, 255, var(--tw-bg-opacity));
  --tw-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1),
    0 4px 6px -2px rgba(0, 0, 0, 0.05);
  box-shadow: var(--tw-ring-offset-shadow, 0 0 #0000),
    var(--tw-ring-shadow, 0 0 #0000), var(--tw-shadow);
}
.card-.ant-card-head {
  font-size: 1.125rem;
  line-height: 1.75rem;
  font-weight: 500;
}
.ant-card-body {
  padding: 5px;
  margin: 8px;
  line-height: 15px;
}
.ant-card-head-title {
  font-size: 14px;
  font-weight: bold;
}
</style>
