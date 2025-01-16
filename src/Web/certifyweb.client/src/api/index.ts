import { trimStart } from 'lodash'

import * as openApi from './OpenApiClient'

export * from './OpenApiClient'

const client = new openApi.OpenApiClient(window.__env__.VITE_API_URL)
export const baseUrl = window.__env__.VITE_API_URL
export const getUrl = (url?: string) => {
    if (!url) return url
    const baseU = baseUrl?.trim()?.replace('\\', '/')
    const urlU = url?.trim()?.replace('\\', '/')
    const u = urlU?.startsWith('//') ? urlU : (baseU + trimStart(urlU, '/'))
    return u.replace('\\', '/').replace(/([^//])\/+/g, (x, x1) => {
        if (x === '://') return x
        return x1 + '/'
    })
}

export default client

// export const merge = (...sources: any[]) => Object.assign({}, ...sources)

// export type bigintType = string | bigint
