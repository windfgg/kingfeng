<template>
  <div class="content">
    <Notice class="Card ant-card ant-card-bordered" />
    <div class="Card ant-card ant-card-bordered">
      <div class="ant-card-head">
        <div class="ant-card-head-wrapper">
          <a-icon type="code" theme="twoTone" />
          <div class="ant-card-head-title">Cookies登录</div>
        </div>
      </div>
      <div class="ant-card-body">
        <div class="text-center">
          <a-input
            v-model="WsKey"
            class="magrin"
            type="text"
            style="text-align: center"
            placeholder="请输入wskey"
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
  data () {
    return {
      WsKey: "",
      remarks: "",
    };
  },
  mounted () {
  },
  created () {
    const uid = localStorage.getItem('uid')
    const adminkey = localStorage.getItem('adminkey')
    if (uid) {
      this.$router.push('/index')
    } else if (adminkey) {
      this.$router.push('/admin')
    }
  },
  methods: {
    CookiesCheck () {
      const pin =
        this.WsKey.match(/pin=(.*?);/) && this.WsKey.match(/pin=(.*?);/)[1];
      pin == decodeURIComponent(pin);
      const wskey =
        this.WsKey.match(/wskey=(.*?);/) && this.WsKey.match(/wskey=(.*?);/)[1];
      if (pin && wskey) {
        if (this.remarks == '') {
          this.$message.error("备注不能为空", 1.5);
          return
        }
        var json = [
          {
            name: "JD_WSCK",
            value: this.WsKey,
            remarks: "r=" + this.remarks + ";",
          },
        ];
        this.$http.post("api/env", json).then((response) => {
          if (response.data.code === 200) {
            //console.log(response.data.data._id[0]);
            localStorage.setItem("uid", response.data.data._id[0]);
            setTimeout(() => {
              this.$router.push({
                name: "Index"
              });
            }, 1000)
            localStorage.setItem("name", this.remarks);
            this.$message.success("欢迎回来 " + this.remarks, 2);
          } else {
            this.$message.error("请求服务器失败，请检查服务端后重试！", 1.5);
          }
        }, (response) => {
          response
          this.$message.error("请求服务器失败，请检查服务端后重试！", 1.5);
        });
        //var data = request.env("JD_WSCK", this.WsKey, "r:" + pin + ";");
        //console.log(data);
      } else {
        //判断是否为管理员
        this.$http.get("api/admin?key=" + this.WsKey).then((response) => {
          if (response.data.code === 200) {
            localStorage.setItem("adminkey", this.WsKey);
            setTimeout(() => {
              this.$router.push({
                name: "Admin"
              });
              //延迟时间：3秒
            }, 1000)
            this.$message.success("管理员 欢迎回来 ", 2);
          } else {
            this.$message.error("wskey 解析失败，请检查格式后重试！", 1.5);
          }
        }, (response) => {
          response
          this.$message.error("请求服务器失败，请检查服务端后重试！", 1.5);
        });
      }
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