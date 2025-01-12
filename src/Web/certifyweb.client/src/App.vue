<template>
  <el-config-provider :locale="zhCn">
    <el-container class="layout-container">
      <el-header height="50px" v-if="false">
        <div><!--黑马程序员：<strong>小帅鹏</strong>--></div>
        <el-dropdown placement="bottom-end">
          <!-- 展示给用户，默认看到的 -->
          <span class="el-dropdown__box">
            <!--<el-avatar :src="avatar" />-->
            <el-icon><i-ep-caret-bottom /></el-icon>
          </span>
          <!-- 折叠的下拉部分 -->
          <template #dropdown>
            <el-dropdown-menu>
              <!--<el-dropdown-item command="profile" :icon="User">基本资料</el-dropdown-item>
              <el-dropdown-item command="avatar" :icon="Crop">更换头像</el-dropdown-item>
              <el-dropdown-item command="password" :icon="EditPen">重置密码</el-dropdown-item>
              <el-dropdown-item command="logout" :icon="SwitchButton">退出登录</el-dropdown-item>-->
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </el-header>
      <el-container>
        <el-aside width="200px">
          <!--
          el-menu 整个菜单组件
          active-text-color：点亮的时候文字应该是什么颜色
          :default-active="$route.path"  配置默认高亮的菜单项，即当前$route.path作为当前高亮的标识，这个值一旦跟下面配置的index是相等的，它就会高亮
          router  router选项开启，el-menu-item 的 index 就是点击跳转的路径

          el-menu-item 菜单项
          index="/article/channel" 配置的是访问的跳转路径，配合default-active的值，实现高亮
          -->
          <el-menu router :default-active="$route.path">
            <template v-for="val in menuLists">
              <el-sub-menu :index="val.path" v-if="val.children && val.children.length > 0"
                           :key="val.path">
                <template #title>
                  <SvgIcon :name="val.meta?.icon" />
                  <span>{{ val.meta?.title }}</span>
                </template>
                <!-- <SubItem :chil="val.children" /> -->
                <el-menu-item v-for="val2 in val.children" :index="val2.path" :key="val2.path">
                  <SvgIcon :name="val2?.meta?.icon" />
                  <template #title
                            v-if="!val2?.meta?.isLink || (val2?.meta?.isLink && val2?.meta?.isIframe)">
                    <span>{{ val2?.meta?.title }}</span>
                  </template>
                  <template #title v-else>
                    <!-- <a class="w100" @click.prevent="onALinkClick(val)">{{ $t(val.meta.title) }}</a> -->
                    <a class="w100">{{ val2?.meta?.title }}</a>
                  </template>
                </el-menu-item>
              </el-sub-menu>
              <template v-else>
                <el-menu-item :index="val.path" :key="val.path">
                  <SvgIcon :name="val?.meta?.icon" />
                  <template #title
                            v-if="!val?.meta?.isLink || (val?.meta?.isLink && val?.meta?.isIframe)">
                    <span>{{ val?.meta?.title }}</span>
                  </template>
                  <template #title v-else>
                    <!-- <a class="w100" @click.prevent="onALinkClick(val)">{{ $t(val.meta.title) }}</a> -->
                    <a class="w100">{{ val?.meta?.title }}</a>
                  </template>
                </el-menu-item>
              </template>
            </template>
            <!--<el-sub-menu index="/user">
                <template #title>
                    <el-icon><i-ep-UserFilled /></el-icon>
                    <span>个人中心</span>
                </template>
                <el-menu-item index="/user/profile">
                    <el-icon><i-ep-User /></el-icon>
                    <span>基本资料</span>
                </el-menu-item>
                <el-menu-item index="/user/avatar">
                    <el-icon><i-ep-Crop /></el-icon>
                    <span>更换头像</span>
                </el-menu-item>
                <el-menu-item index="/user/password">
                    <el-icon><i-ep-EditPen /></el-icon>
                    <span>重置密码</span>
                </el-menu-item>
            </el-sub-menu>-->
          </el-menu>
        </el-aside>
        <el-main><router-view /></el-main>
      </el-container>
    </el-container>
  </el-config-provider>
</template>

<script setup lang="ts">
  import { ElConfigProvider } from 'element-plus'
  import zhCn from 'element-plus/es/locale/lang/zh-cn'
  import { dynamicRoutes } from '@/router/route';
  const menuLists = dynamicRoutes.filter(f => f?.meta?.title);
</script>

<style lang="scss" scoped>
  .layout-container {
    height: 100vh;

    .el-aside {
      /*        background-color: #e5e5e5;

          &__logo {
              height: 120px;
              background: url('@/assets/logo.png') no-repeat center / 120px auto;
          }*/

      .el-menu {
        /*border-right: none;*/
        height: 100%;
      }
    }

    .el-header {
      background-color: #e5e5e5;
      display: flex;
      align-items: center;
      justify-content: space-between;

      .el-dropdown__box {
        display: flex;
        align-items: center;

        .el-icon {
          color: #999;
          margin-left: 10px;
        }

        &:active,
        &:focus {
          outline: none;
        }
      }
    }
    /*        .el-footer {
          display: flex;
          align-items: center;
          justify-content: center;
          font-size: 14px;
          color: #666;
      }*/
  }
</style>
