//import Vue from "vue";
import Vue from "vue";
import App from "./App.vue";
import VueResource from "vue-resource";
import router from "./router";
import {
  Button,
  Icon,
  Card,
  Input,
  message,
  Space,
  Select,
} from "ant-design-vue"; //引入Ant-Desgin
const components = [
  Button,
  Icon,
  Card,
  Input,
  Input.TextArea,
  Space,
  Select.Option,
];
Vue.use(VueResource);

components.forEach((component) => {
  Vue.component(component.name, component);
});

//import router from "./router";

Vue.config.productionTip = false; //关闭生产提示

//console.log(process.env);
//环境变量;
// console.log(Vue.prototype.$request_url);
Vue.prototype.$message = message; //类似于this.$message这种方法的调用，需要Vue的原型上添加对象方法
//Vue.prototype.$request_url = process.env.VUE_APP_RootUrl; //测试接口地址

new Vue({
  render: (h) => h(App),
  router,
}).$mount("#app");
