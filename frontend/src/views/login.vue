<template>
  <div class="content">
    <!--作者公告-->
    <Notice class="Card ant-card ant-card-bordered" />
    <!--用户提醒-->
    <div v-if="lodings.noticIsShow">
      <a-spin :spinning="!lodings.noticeIsLoding">
        <div
          v-show="lodings.noticeIsLoding"
          class="Card ant-card ant-card-bordered"
        >
          <div class="ant-card-head">
            <div class="ant-card-head-wrapper">
              <a-icon type="calendar" theme="twoTone" />
              <div class="ant-card-head-title">{{ config.name }}温馨提醒您</div>
            </div>
          </div>
          <div class="ant-card-body">
            {{ config.notice }}
          </div>
        </div>
      </a-spin>
    </div>
    <!--节点选择-->
    <a-spin :spinning="!lodings.serverIsLoding">
      <div
        v-show="lodings.serverIsLoding"
        class="Card ant-card ant-card-bordered"
      >
        <div class="ant-card-head">
          <div class="ant-card-head-wrapper">
            <a-icon type="radar-chart" style="color: #2c99ff" />
            <div class="ant-card-head-title">
              节点选择
            </div>
          </div>
        </div>
        <div class="ant-card-body">
          <div class="text-center">
            <div>
              <a-select
                v-if="servers"
                :default-active-first-option="false"
                style="width: 100%"
                @change="nodeChange"
              >
                <a-select-option
                  v-for="item in servers"
                  :key="item.id"
                  :disabled="item.maxCount - item.currentCount <= 0 && 1"
                >
                  {{ item.name }}
                </a-select-option>
              </a-select>
            </div>
          </div>
        </div>
      </div>
    </a-spin>
    <!--登录-->
    <div v-if="selecItem" class="Card ant-card ant-card-bordered">
      <div class="ant-card-head">
        <div class="ant-card-head-wrapper">
          <a-icon type="code" theme="twoTone" />
          <div class="ant-card-head-title">
            Cookies登录
          </div>
        </div>
      </div>
      <div class="ant-card-body">
        <div class="text-center">
          <a-input
            v-model="cookies"
            class="magrin"
            type="text"
            style="text-align: center"
            placeholder="请输入cookies"
          />
          <br />
          <a-input
            v-model="remarks"
            class="magrin"
            type="text"
            placeholder="请输入备注"
            style="width: 60%; text-align: center"
          />
          <br />
          <a-button
            class="magrin"
            type="primary"
            shape="round"
            :loading="isLogin.loading"
            @click="CookiesCheck"
          >
            登录
          </a-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Notice from "../components/Notice.vue";
export default {
  components: {
    Notice,
  },
  // props: ["servers"],
  data() {
    return {
      servers: undefined,
      lodings: {
        serverIsLoding: true,
        noticeIsLoding: true,
        noticIsShow: false,
      },
      isLogin: {
        loading: false,
      },
      cookies: "",
      remarks: "",
      config: {
        notice: undefined,
        name: undefined,
      },
      selecItem: undefined,
    };
  },
  mounted() {
    console.log(
      "本项目在 github:https://github.com/QiFengg/kingfeng 进行分发 喜欢的话麻烦给个start 谢谢~"
    );
    console.log("By:qifeng https://github.com/QiFengg");

    this.getServer();
  },
  created() {
    document.title = "KingFeng - 登录页面";

    const uid = localStorage.getItem("uid");
    const adminkey = localStorage.getItem("adminkey");
    if (uid) {
      this.$router.push("/index");
    } else if (adminkey) {
      this.$router.push("/admin");
    }

    this.getConfig();
  },
  methods: {
    CookiesCheck() {
      this.$set(this.isLogin, "loading", true);
      const ql_url = this.selecItem.address;
      //判断wskey格式
      const pin =
        this.cookies.match(/pin=(.*?);/) && this.cookies.match(/pin=(.*?);/)[1];
      pin == decodeURIComponent(pin);
      const wskey =
        this.cookies.match(/wskey=(.*?);/) &&
        this.cookies.match(/wskey=(.*?);/)[1];
      //判断pin格式
      const pt_key =
        this.cookies.match(/pt_key=(.*?);/) &&
        this.cookies.match(/pt_key=(.*?);/)[1];
      pin == decodeURIComponent(pin);
      const pt_pin =
        this.cookies.match(/pt_pin=(.*?);/) &&
        this.cookies.match(/pt_pin=(.*?);/)[1];
      if (pin && wskey) {
        //判断wskey
        if (this.remarks == "") {
          this.$message.error("备注不能为空", 1.5);
          this.$set(this.isLogin, "loading", false);
          return;
        } else {
          if (this.remarks.length < 3) {
            this.$message.error("备注不能少于三个字", 1.5);
            this.$set(this.isLogin, "loading", false);
            return;
          }
        }
        var WSCK = [
          {
            name: "JD_WSCK",
            value: this.cookies,
            remarks: this.remarks,
          },
        ];
        this.$http.post("api/env?ql_url=" + ql_url, WSCK).then(
          (response) => {
            if (response.data.code === 200) {
              //console.log(response.data.data._id[0]);
              localStorage.setItem("uid", response.data.data._id[0]);
              setTimeout(() => {
                this.$router.push({
                  name: "Index",
                  params: { push: this.push },
                });
              }, 1000);
              localStorage.setItem("name", this.remarks);
              localStorage.setItem("address", ql_url);
              this.$message.success("欢迎回来 " + this.remarks, 2);
            } else {
              this.$message.error(response.data.msg, 1.5);
              this.$set(this.isLogin, "loading", false);
              return;
            }
          },
          (response) => {
            this.$message.error(response.data.msg, 1.5);
            this.$set(this.isLogin, "loading", false);
            return;
          }
        );
      } else if (pt_key && pt_pin) {
        //判断是否pinkey

        this.$http
          .get("api/CheeckPinCk?pinck=" + this.cookies)
          .then((response) => {
            if (response.data.code != 200) {
              this.$message.error("请检查pinck 是否过期以及是否正确", 1.5);
              this.$set(this.isLogin, "loading", false);
              return;
            } else {
              if (this.remarks.length == "") {
                this.remarks = pt_pin;
              }
              var PTCK = [
                {
                  name: "JD_COOKIE",
                  value: this.cookies,
                  remarks: this.remarks,
                },
              ];
              this.$http.post("api/env?ql_url=" + ql_url, PTCK).then(
                (response) => {
                  if (response.data.code === 200) {
                    localStorage.setItem("uid", response.data.data._id[0]);
                    localStorage.setItem("name", this.remarks);

                    setTimeout(() => {
                      this.$router.push({
                        name: "Index",
                        params: { push: this.push },
                      });
                    }, 1000);
                    localStorage.setItem("address", ql_url);
                    this.$message.success("欢迎回来 " + this.remarks, 2);
                    this.$set(this.isLogin, "loading", false);
                    return;
                  } else {
                    this.$message.error(response.data.msg, 1.5);
                    this.$set(this.isLogin, "loading", false);
                    return;
                  }
                },
                (response) => {
                  this.$message.error(response.data.msg, 1.5);
                }
              );
            }
          });
      } else {
        //判断是否为管理员
        if (this.cookies == "") {
          this.$message.error("输入框不能为空", 1.5);
          this.$set(this.isLogin, "loading", false);
          return;
        }
        this.$http.get("api/admin?key=" + this.cookies).then(
          (response) => {
            if (response.data.code === 200) {
              localStorage.setItem("adminkey", this.cookies);
              this.$message.success("管理员 欢迎回来 ", 2);
              setTimeout(() => {}, 200);
              setTimeout(() => {
                this.$router.push({
                  name: "Admin",
                });
                //延迟时间：3秒
              }, 1000);
              this.$set(this.isLogin, "loading", false);
              return;
            } else {
              this.$message.error("登录失败,请检查是否输入正确", 1.5);
              this.$set(this.isLogin, "loading", false);
              return;
            }
          },
          (response) => {
            this.$message.error(response.data.msg, 2);
            this.$set(this.isLogin, "loading", false);
            return;
          }
        );
      }
    },
    getServer() {
      this.lodings.serverIsLoding = !this.lodings.serverIsLoding;
      this.$http.get("api/servers").then(
        async (response) => {
          if (response.data.code === 200) {
            this.servers = response.data.data;
          } else {
            this.$message.error(response.data.msg, 1.5);
          }
          setTimeout(() => {
            this.lodings.serverIsLoding = !this.lodings.serverIsLoding;
          }, 200);
        },
        (response) => {
          response;
          this.$message.error(response.data.msg, 3);
        }
      );
    },
    getConfig() {
      this.lodings.noticeIsLoding = !this.lodings.noticeIsLoding;
      this.$http.get("api/config").then(
        async (response) => {
          if (response.data.code === 200) {
            if (
              response.data.data.name != null &&
              response.data.data.notice != null
            ) {
              this.config = response.data.data;
              console.log(this.lodings.noticIsShow);
              this.$set(this.lodings, "noticIsShow", !this.lodings.noticIsShow);
              console.log(this.lodings.noticIsShow);
              setTimeout(() => {
                this.lodings.noticeIsLoding = !this.lodings.noticeIsLoding;
              }, 200);
            } else {
            }
          } else {
            this.$message.error(response.data.msg, 2);
          }
        },
        (response) => {
          response;
          this.$message.error(response.data.msg, 2);
        }
      );
    },
    nodeChange(value) {
      this.selecItem = this.servers[value - 1];
    },
  },
};
</script>

<style>
.content {
  margin: auto;
  width: 91.666667%;
  max-width: 64rem;
}
.text-center {
  margin: 5px;
  text-align: center;
}
.magrin {
  margin-bottom: 8px;
  margin-top: 8px;
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
  margin: 0px 0px 0px 8px;
  font-size: 14px;
  font-weight: bold;
}
</style>
