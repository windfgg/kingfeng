<template>
  <div class="content">
    <Notice class="Card ant-card ant-card-bordered" :course="config.course" />
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
      cookies: "",
      remarks: "",
      config: {
        course: undefined,
        push: undefined,
      }
    };
  },
  mounted () {
    console.log('本项目在 github:https://github.com/QiFengg/kingfeng 进行分发 喜欢的话麻烦给个start 谢谢~')
    console.log('By:qifeng https://github.com/QiFengg')

    document.title = 'KingFeng - 登录页面'

    this.$http.get('api/config').then(response => {
      if (response.data.code === 200) {
        this.config.course = response.data.data.course
        this.config.push = response.data.data.push

        if (this.config.push != localStorage.getItem('push')) {
          localStorage.setItem('push', this.config.push)
        }
      }
    }, (response) => {
      response
      this.$message.error("获取服务端配置失败,请检查配置文件", 2);
    })
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
      //判断wskey格式
      const pin =
        this.cookies.match(/pin=(.*?);/) && this.cookies.match(/pin=(.*?);/)[1];
      pin == decodeURIComponent(pin);
      const wskey =
        this.cookies.match(/wskey=(.*?);/) && this.cookies.match(/wskey=(.*?);/)[1];
      //判断pin格式
      const pt_key =
        this.cookies.match(/pt_key=(.*?);/) && this.cookies.match(/pt_key=(.*?);/)[1];
      pin == decodeURIComponent(pin);
      const pt_pin =
        this.cookies.match(/pt_pin=(.*?);/) && this.cookies.match(/pt_pin=(.*?);/)[1];
      if (pin && wskey) { //判断wskey
        if (this.remarks == '') {
          this.$message.error("备注不能为空", 1.5);
          return
        } else {
          if (this.remarks.length < 3) {
            this.$message.error("备注不能少于三个字", 1.5);
            return
          }
        }
        var WSCK = [
          {
            name: "JD_WSCK",
            value: this.cookies,
            remarks: this.remarks,
          },
        ];
        this.$http.post("api/env", WSCK).then((response) => {
          if (response.data.code === 200) {
            //console.log(response.data.data._id[0]);
            localStorage.setItem("uid", response.data.data._id[0]);
            setTimeout(() => {
              this.$router.push({
                name: "Index", params: { push: this.push }
              });
            }, 1000)
            localStorage.setItem("name", this.remarks);
            this.$message.success("欢迎回来 " + this.remarks, 2);
          } else {
            this.$message.error(response.data.msg, 1.5);
          }
        }, (response) => {

          this.$message.error(response.data.msg, 1.5);
        });
      } else if (pt_key && pt_pin) {//判断是否pinkey
        if (this.remarks.length == '') {
          this.remarks = pt_pin
        }

        var PTCK = [
          {
            name: "JD_COOKIE",
            value: this.cookies,
            remarks: this.remarks,
          },
        ];
        this.$http.post("api/env", PTCK).then((response) => {
          if (response.data.code === 200) {
            localStorage.setItem("uid", response.data.data._id[0]);
            localStorage.setItem("name", this.remarks);

            setTimeout(() => {
              this.$router.push({
                name: "Index", params: { push: this.push }
              });
            }, 1000)

            this.$message.success("欢迎回来 " + this.remarks, 2);
          } else {
            this.$message.error(response.data.msg, 1.5);
          }
        }, (response) => {
          this.$message.error(response.data.msg, 1.5);
        });
      } else { //判断是否为管理员
        this.$http.get("api/admin?key=" + this.cookies).then((response) => {
          if (response.data.code === 200) {
            localStorage.setItem("adminkey", this.cookies);
            setTimeout(() => {
              this.$router.push({
                name: "Admin"
              });
              //延迟时间：3秒
            }, 1000)
            this.$message.success("管理员 欢迎回来 ", 2);
          } else {
            this.$message.error("服务端错误,请检查服务端日志！", 1.5);
          }
        }, (response) => {
          response
          this.$message.error(response.data.msg, 2);
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