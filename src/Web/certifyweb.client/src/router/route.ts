import { type RouteRecordRaw } from 'vue-router'

/**
 * 建议：路由 path 路径与文件夹名称相同，找文件可浏览器地址找，方便定位文件位置
 *
 * 路由meta对象参数说明
 * meta: {
 *      title:          菜单栏及 tagsView 栏、菜单搜索名称（国际化）
 *      isLink：        是否超链接菜单，开启外链条件，`1、isLink: 链接地址不为空 2、isIframe:false`
 *      isHide：        是否隐藏此路由
 *      isKeepAlive：   是否缓存组件状态
 *      isAffix：       是否固定在 tagsView 栏上
 *      isIframe：      是否内嵌窗口，开启条件，`1、isIframe:true 2、isLink：链接地址不为空`
 *      roles：         当前路由权限标识，取角色管理。控制路由显示、隐藏。超级管理员：admin 普通角色：common
 *      icon：          菜单、tagsView 图标，阿里：加 `iconfont xxx`，fontawesome：加 `fa xxx`
 * }
 */

// 扩展 RouteMeta 接口
declare module 'vue-router' {
  interface RouteMeta {
    title?: string;
    isLink?: string;
    isHide?: boolean;
    isKeepAlive?: boolean;
    isAffix?: boolean;
    isIframe?: boolean;
    roles?: string[];
    icon?: string;
  }
}

export const dynamicRoutes: Array<RouteRecordRaw> = [
  {
    path: '/acme',
    // component: () => import('@/views/home/index.vue'),
    redirect: '/acme/list',
    meta: {
      title: '免费证书',
      icon: 'ele-HomeFilled',
    },
    children: [
      {
        path: '/acme/list',
        component: () => import('@/views/acme-certificate/index.vue'),
        meta: {
          title: '证书列表',
          icon: 'ele-HomeFilled',
        }
      },
      {
        path: '/acme/dns',
        component: () => import('@/views/acme-certificate/dns.vue'),
        meta: {
          title: 'DNS 账户',
          icon: 'ele-Promotion',
        }
      },
      {
        path: '/acme/account',
        component: () => import('@/views/acme-certificate/account.vue'),
        meta: {
          title: 'Acme 账户',
          icon: 'ele-Promotion',
        }
      },
      {
        path: '/acme/authority',
        component: () => import('@/views/acme-certificate/acme-authority.vue'),
        meta: {
          title: '证书颁发机构',
          icon: 'ele-Promotion',
        }
      },
      {
        path: '/acme/challenge-providers',
        component: () => import('@/views/acme-certificate/challenge-providers.vue'),
        meta: {
          title: '验证提供方式',
          icon: 'ele-Promotion',
        }
      },
    ],
  },
  {
    path: '/',
    meta: {
      title: '自签证书',
      icon: 'ele-HomeFilled',
    },
    children: [
      {
        path: '/aaaaa',
        component: () => import('@/views/home/index.vue'),
        meta: {
          title: '证书列表',
          icon: 'ele-HomeFilled',
        }
      }
    ],
  },
  {
    path: '/',
    // component: () => import('@/views/home/index.vue'),
    meta: {
      title: '上传证书',
      icon: 'ele-HomeFilled',
    },
    children: [
      {
        path: '/aaaaa',
        component: () => import('@/views/home/index.vue'),
        meta: {
          title: '证书列表',
          icon: 'ele-HomeFilled',
        }
      }
    ],
  }
];

export const notFoundAndNoPower = [
  // {
  // 	path: '/:path(.*)*',
  // 	name: 'notFound',
  // 	component: () => import('@/views/error/404.vue'),
  // 	meta: {
  // 		title: 'message.staticRoutes.notFound',
  // 		isHide: true,
  // 	},
  // },
  // {
  // 	path: '/401',
  // 	name: 'noPower',
  // 	component: () => import('@/views/error/401.vue'),
  // 	meta: {
  // 		title: 'message.staticRoutes.noPower',
  // 		isHide: true,
  // 	},
  // },
];
