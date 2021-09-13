<template>
  <div class="content">
    <!-- 个人中心 -->
    <div class="Card ant-card ant-card-bordered">
      <div class="ant-card-head">
        <div class="ant-card-head-wrapper">
          <a-icon type="crown" theme="twoTone" />
          <div class="ant-card-head-title">个人中心</div>
        </div>
      </div>
      <div class="ant-card-body">
        <div>
          <a-input
            type="text"
            v-model="secretkey"
            placeholder="请输入新的SecretKey"
          />
        </div>
        <br />
        <div>
          <a-space size="large">
            <a-button type="primary" shape="round" @click="updateadminkey"
              >修改SecretKey
            </a-button>
            <a-button type="danger" shape="round" @click="logout">
              退出登录
            </a-button>
          </a-space>
        </div>
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

    <!-- 执行任务 -->
    <!-- <div class="Card ant-card ant-card-bordered">
      <div class="ant-card-head">
        <div class="ant-card-head-wrapper">
          <a-icon type="tool" theme="twoTone" />
          <div class="ant-card-head-title">执行任务</div>
        </div>
      </div>
      <div class="ant-card-body">
        <a-input
          v-model="taskName"
          type="text"
          placeholder="请输入任务名"
          style="width: 80%"
        />

        <div>
          <br />
          <a-space size="large">
            <a-button type="primary" shape="round" @click="wskeytask">
              wskey转换
            </a-button>
          </a-space>
        </div>
      </div>
    </div> -->
  
    <!-- 任务日志 -->
    <!-- <div class="Card ant-card ant-card-bordered">
      <div class="ant-card-head">
        <div class="ant-card-head-wrapper">
          <a-icon type="tool" theme="twoTone" />
          <div class="ant-card-head-title">任务日志</div>
        </div>
      </div>

      <div class="ant-card-body">
        <div>
          <a-textarea id="logs" v-model="logs" rows="10" />
        </div>
        <br />
      </div>
    </div>
  </div> -->
</template>

<script>
//import Vue from 'vue'
export default {
  data () {
    return {
      taskName: "",
      logs: undefined,
      adminkey: "",
      timer: undefined,
      secretkey: ""
    }
  },
  //窗体刚创建没渲染时候
  created () {
    const adminkey = localStorage.getItem('adminkey')
    if (adminkey) {
      this.$http.get("api/admin?key=" + adminkey).then((response) => {
        if (response.data.code === 200) {
          //this.$message.success("欢迎回来", 1.5);
        } else {
          this.$message.error("SecretKey已被更改,请重新登录", 2);
          localStorage.removeItem('adminkey');
          this.$router.push('/')
        }
      })
    } else {
      this.$router.push('/')
    }
  },
  //渲染过后
  mounted () {
    this.adminkey = localStorage.getItem('adminkey')

    document.title = 'KingFeng - 管理员'
  },
  //销毁之前
  beforeDestroy () {
    //clearInterval(this.timer) // 清除定时器
  },
  methods: {
    // //执行任务
    // task () {
    //   // const taksName = ;
    //   // const adminkey = ;
    //   this.$http.put("api/task?taskName=" + this.taskName + "&key=" + this.adminkey).then((response) => {
    //     if (response.data.code === 200) {
    //       this.$message.success(this.taskName + "执行成功", 1.5);
    //       // clearInterval(this.timer) // 清除定时器
    //       // this.timer = setInterval(this.readLog, 1000) // 设置定时器
    //     } else {
    //       this.$message.error("错误:" + response.data.msg, 2);
    //     }
    //   });
    // },
    //wskye任务
    wskeytask () {
      this.taskName = ''
      this.$http.put("api/task?taskName=" + 'ws' + "&key=" + this.adminkey).then((response) => {
        if (response.data.code === 200) {
          this.$message.success("执行wskey转换成功", 1.5);
          //clearInterval(this.timer) // 清除定时器
          this.timer = setInterval(this.readLog, 1000) // 设置定时器
        } else {
          this.$message.error("错误:" + response.data.msg, 2);
        }
      });

    },
    // //读取日志
    // readLog () {
    //   console.log('执行一次')
    //   var task = this.taskName == '' ? 'ws' : this.taskName
    //   this.$http.get("api/log?taskName=" + task + "&key=" + this.adminkey).then((response) => {
    //     if (response.data.code === 200) {
    //       if (response.data.data.indexOf('执行结束') != -1) {
    //         this.logs = response.data.data;
    //         this.taskName = ''
    //         const textarea = document.getElementById('logs');
    //         textarea.scrollTop = textarea.scrollHeight;
    //         clearInterval(this.timer) // 清除定时器 
    //       } else {
    //         //this.logs = response.data.data.replace(/\r|\n/ig, "");
    //         const textarea = document.getElementById('logs');
    //         textarea.scrollTop = textarea.scrollHeight;
    //         this.logs = response.data.data;
    //       }
    //     } else {
    //       this.$message.error("读取日志错误:" + response.data.msg, 2);
    //     }
    //   });
    // },
    //退出登录
    logout () {
      localStorage.removeItem('adminkey');
      // clearInterval(this.timer) // 清除定时器
      this.$message.success("退出成功", 1);
      setTimeout(() => {
        this.$router.push('/')
      }, 1000)
    },
    //修改adminkey
    updateadminkey () {
      this.$http.put('api/updateSecretKey?oldkey=' + localStorage.getItem('adminkey') + '&newkey=' + this.secretkey).then(response => {
        if (response.data.code == 200) {
          this.$message.success(response.data.msg + ' 请重新登录', 1.5)
          localStorage.removeItem('adminkey');
          this.$router.push('/')
        } else if (response.data.code == 400) {
          this.$message.error(response.data.msg, 1.5)
        }
      })
    }
  },
}
</script>

<style>
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