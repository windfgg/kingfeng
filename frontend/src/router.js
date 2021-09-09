import VueRouter from "vue-router";
import Vue from "vue";
//引入页面
import Login from "./views/login.vue";
import Index from "./views/index.vue";
import Admin from "./views/admin.vue";
Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Login",
    component: Login,
  },
  {
    path: "/index",
    name: "Index",
    component: Index,
  },
  {
    path: "/admin",
    name: "Admin",
    component: Admin,
  },
];

const router = new VueRouter({
  routes,
});

export default router;
