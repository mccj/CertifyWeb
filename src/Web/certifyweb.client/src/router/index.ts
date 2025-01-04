import { createWebHashHistory, createRouter } from 'vue-router'
import { dynamicRoutes, notFoundAndNoPower } from './route';


const router = createRouter({
    history: createWebHashHistory(),
    routes:[...notFoundAndNoPower, ...dynamicRoutes],
})

router.beforeEach((/*_to, _from*/) => {
  // // 而不是去检查每条路由记录
  // // to.matched.some(record => record.meta.requiresAuth)
  // if (to.meta.requiresAuth && !auth.isLoggedIn()) {
  //   // 此路由需要授权，请检查是否已登录
  //   // 如果没有，则重定向到登录页面
  //   return {
  //     path: '/login',
  //     // 保存我们所在的位置，以便以后再来
  //     query: { redirect: to.fullPath },
  //   }
  // }
})

export default router
