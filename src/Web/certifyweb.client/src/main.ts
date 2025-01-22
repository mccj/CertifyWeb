import './assets/main.css'
import 'element-plus/dist/index.css'

import * as ElementPlusIconsVue from '@element-plus/icons-vue'

// import moment from 'moment';
// import 'moment/dist/locale/zh-cn';
// moment.locale('zh-cn');

import { createApp } from 'vue'
import router from './router/index'
import App from './App.vue'

const app = createApp(App);

for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component('ele-'+key, component)
}

app.use(router).mount('#app')
