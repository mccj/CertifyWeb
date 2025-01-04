import { fileURLToPath, URL } from 'node:url';

import { defineConfig, loadEnv, ConfigEnv } from 'vite';
import plugin from '@vitejs/plugin-vue';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers'
import IconsResolver from 'unplugin-icons/resolver'
import Icons from 'unplugin-icons/vite'


const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "certifyweb.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(baseFolder)) {
    fs.mkdirSync(baseFolder, { recursive: true });
}

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (0 !== child_process.spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], { stdio: 'inherit', }).status) {
        throw new Error("Could not create certificate.");
    }
}

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:5032';

// https://vitejs.dev/config/
const viteConfig = defineConfig((mode: ConfigEnv) => {
    const env = loadEnv(mode.mode, process.cwd());
    fs.writeFileSync('./public/config.js', `window.__env__ = ${JSON.stringify(env, null, 2)} `);
    return {
        plugins: [
            plugin(),
            AutoImport({
                // 你可以指定要自动导入的库
                imports: [
                    'vue',
                    'vue-router',
                    // ...其他库
                ],
                resolvers: [ElementPlusResolver(),
                // 自动导入图标组件
                IconsResolver({
                    prefix: 'Icon',
                })
                ],
                dts: 'src/types/auto-imports.d.ts'
            }),
            Components({
                resolvers: [ElementPlusResolver(),
                // 自动注册图标组件
                IconsResolver({
                    enabledCollections: ['ep'],
                })
                ],
                dts: 'src/types/components.d.ts'
            }),
            Icons({
                autoInstall: true,
            }),
        ],
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url))
            }
        },
        server: {
            proxy: {
                '^/api': {
                    target,
                    secure: false,
                    // // target: import.meta.VITE_API_URL,
                    // target: 'http://iot-one-net-yiqing.nbyzwlkj.com/',
                    // changeOrigin: true,
                    // // rewrite: (path) => path.replace(/^\/api/, ''),
                }
            },
            port: 55410,
            https: {
                key: fs.readFileSync(keyFilePath),
                cert: fs.readFileSync(certFilePath),
            }
        }
    };
});

export default viteConfig;
