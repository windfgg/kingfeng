<template>
  <div class="content">
    <!-- 个人中心 -->
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
        <div>
          <a-input
            v-model="secretkey"
            type="text"
            placeholder="请输入新的SecretKey"
          />
        </div>
        <br />
        <div>
          <a-space size="large">
            <a-button type="primary" shape="round" @click="updateadminkey">
              修改SecretKey
            </a-button>
            <a-button type="danger" shape="round" @click="logout">
              退出登录
            </a-button>
          </a-space>
        </div>
      </div>
    </div>

    <!-- 配置修改 -->
    <div class="Card ant-card ant-card-bordered">
      <div class="ant-card-head">
        <div class="ant-card-head-wrapper">
          <a-icon type="tool" theme="twoTone" />
          <div class="ant-card-head-title">
            配置管理
          </div>
        </div>
      </div>
      <div class="ant-card-body">
        <a-spin :spinning="!isLoding">
          <div v-show="isLoding">
            <div>
              <a-input
                v-model="config.name"
                type="text"
                placeholder="公告来自谁"
              />
            </div>
            <br />
            <div>
              <a-input
                v-model="config.notice"
                type="text"
                placeholder="公告内容"
              />
            </div>
            <br />
            <div>
              <a-input
                v-model="config.push"
                type="text"
                placeholder="推送图片图床地址"
              />
            </div>
            <br />
            <a-space size="large">
              <a-button type="primary" shape="round" @click="savaConfig">
                保存配置
              </a-button>
              <a-button type="primary" shape="round" @click="open(config.push)">
                预览推送图片
              </a-button>
            </a-space>
          </div>
        </a-spin>
      </div>
    </div>

    <!-- 环境变量 -->
    <!-- <div class="Card ant-card ant-card-bordered">
      <div class="ant-card-head">
        <div class="ant-card-head-wrapper">
          <a-icon type="reconciliation" theme="twoTone" />
          <div class="ant-card-head-title">环境变量</div>
        </div>
      </div>
      <div class="ant-card-body">
        <div>
          <a-space size="large">
            <a-button type="primary" shape="round" @click="envsExport"
              >环境变量导出</a-button
            >
            <a-button type="primary" shape="round" @click="envsImport"
              >环境变量恢复</a-button
            >
          </a-space>
        </div>
      </div>
    </div> -->
  </div>
</template>

<script>
//import Vue from 'vue'
export default {
  data() {
    return {
      config: {
        notice: undefined,
        name: undefined,
        push: undefined,
      },
      isLoding: false,
      taskName: "",
      logs: undefined,
      adminkey: "",
      timer: undefined,
      secretkey: "",
    };
  },
  //窗体刚创建没渲染时候
  created() {
    const adminkey = localStorage.getItem("adminkey");
    if (adminkey) {
      this.$http.get("api/admin?key=" + adminkey).then((response) => {
        if (response.data.code === 200) {
          this.$http.get("api/config").then(
            async (response) => {
              if (response.data.code === 200) {
                this.config = response.data.data;
                setTimeout(() => {
                  this.isLoding = !this.isLoding;
                }, 200);
              } else {
                this.$message.error(response.data.msg, 2);
              }
            },
            (response) => {
              response;
              this.$message.error(response.data.msg, 2);
            }
          );
          //this.$message.success("欢迎回来", 1.5);
        } else {
          this.$message.error("SecretKey已被更改,请重新登录", 2);
          localStorage.removeItem("adminkey");
          this.$router.push("/");
        }
      });
    } else {
      this.$router.push("/");
    }
  },
  //渲染过后
  mounted() {
    this.adminkey = localStorage.getItem("adminkey");

    document.title = "KingFeng - 管理员";
  },
  //销毁之前
  beforeDestroy() {
    //clearInterval(this.timer) // 清除定时器
  },
  methods: {
    //wskye任务
    wskeytask() {
      this.taskName = "";
      this.$http
        .put("api/task?taskName=" + "ws" + "&key=" + this.adminkey)
        .then((response) => {
          if (response.data.code === 200) {
            this.$message.success("执行wskey转换成功", 1.5);
            //clearInterval(this.timer) // 清除定时器
            this.timer = setInterval(this.readLog, 1000); // 设置定时器
          } else {
            this.$message.error("错误:" + response.data.msg, 2);
          }
        });
    },
    //退出登录
    logout() {
      localStorage.removeItem("adminkey");
      // clearInterval(this.timer) // 清除定时器
      this.$message.success("退出成功", 1);
      setTimeout(() => {
        this.$router.push("/");
      }, 1000);
    },
    //修改配置
    savaConfig() {
      let body = {
        name: this.config.name,
        push: this.config.push,
        notice: this.config.notice,
      };
      this.$http.post("api/config", body).then((response) => {
        if (response.data.code == 200) {
          this.$message.success("更新配置成功", 1.5);
        }
      });
    },
    open(link) {
      window.open(link, "_blank"); // 新窗口打开外链接
    },
    //修改adminkey
    updateadminkey() {
      this.$http
        .put(
          "api/updateSecretKey?oldkey=" +
            localStorage.getItem("adminkey") +
            "&newkey=" +
            this.secretkey
        )
        .then((response) => {
          if (response.data.code == 200) {
            this.$message.success(response.data.msg + " 请重新登录", 1.5);
            localStorage.removeItem("adminkey");
            this.$router.push("/");
          } else if (response.data.code == 400) {
            this.$message.error(response.data.msg, 1.5);
          }
        });
    },
  },
};
</script>

<style>
.bc {
  background-color: black;
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
  margin: 15px;
  line-height: 15px;
}
.ant-card-head-title {
  margin: 0px 0px 0px 8px;
  font-size: 14px;
  font-weight: bold;
}
</style>
