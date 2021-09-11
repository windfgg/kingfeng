<template>
  <div class="Card ant-card ant-card-bordered">
    <div class="ant-card-head">
      <div class="ant-card-head-wrapper">
        <a-icon type="calendar" theme="twoTone" />
        <div class="ant-card-head-title">公告</div>
      </div>
    </div>
    <div class="ant-card-body">
      <div>
        <p>{{ p1 }}</p>
        <p>{{ p2 }}</p>
        <p>{{ p3 }}</p>
        <div style="display：inline;">
          本项目在
          <a @click="open('https://github.com/QiFengg/kingfeng')">Github</a>
          和
          <a @click="open('https://github.com/QiFengg/kingfeng')">TG频道</a>
          进行分发✨
        </div>
      </div>
      <br />
      <div>{{ notice }}</div>
      <br />
      <a :src="course" @click="open(course)">手机以及电脑抓取Cookies教程</a>
      <br />
    </div>
  </div>
</template>

<script>
export default {
  data () {
    return {
      p1: "请关闭免密支付以及打开支付验密",
      p2: "建议微信绑定账户以保证提现能到账",
      p3: "需手动抓取Cookies 教程请点击下面链接获取",
      tg: "",
      course: undefined,
      notice: undefined
    };
  },
  mounted () {
    //获取配置文件
    this.$http.get('api/config').then(response => {
      if (response.data.code === 200) {
        this.course = response.data.data.course
        this.notice = response.data.data.notice
        var push = localStorage.getItem('push')
        if (push != response.data.data.push) {
          localStorage.setItem('push', response.data.data.push)
        }
      }
    }, (response) => {
      response
      this.$message.error("获取服务端配置失败,请检查配置文件", 2);
      return
    })
  },
  methods: {
    open (link) {
      window.open(link, '_blank') // 新窗口打开外链接
    }
  },
};
</script>

<style>
.line {
  display: inline-block;
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