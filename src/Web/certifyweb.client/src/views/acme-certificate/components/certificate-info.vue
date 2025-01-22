<template>
  <el-dialog v-model="state.isShowDialog" destroy-on-close draggable title="证书下载" width="650">
    <el-tabs>
      <el-tab-pane label="基本信息">
        <!-- {{ state.certInfos }} -->
        <el-descriptions :column="1" v-loading="state.loading">
          <el-descriptions-item label="版本">V{{ state.certInfos?.version }}</el-descriptions-item>
          <el-descriptions-item label="序列号 (SN)">{{ state.certInfos?.serialNumber }}</el-descriptions-item>
          <!-- <el-descriptions-item label="算法">RSA</el-descriptions-item> -->
          <!-- <el-descriptions-item label="私钥长度">2048</el-descriptions-item> -->
          <el-descriptions-item label="证书指纹">{{ state.certInfos?.thumbprint }}</el-descriptions-item>
          <!-- <el-descriptions-item label="公钥SHA2">6F6911A711688BF4EF3EB11338599427C742BAD083E98561C41DF0BD8D0D5052</el-descriptions-item> -->
          <!-- <el-descriptions-item label="签名算法">sha256withrsa</el-descriptions-item> -->
          <el-descriptions-item label="颁发日期">{{ state.certInfos?.effectiveDate }}</el-descriptions-item>
          <el-descriptions-item label="截止日期">{{ state.certInfos?.expirationDate }}</el-descriptions-item>
          <!-- <el-descriptions-item label="SANS值">www.cluster.ink,cluster.ink</el-descriptions-item> -->
          <!-- <el-descriptions-item label="颁发给">Encryption Everywhere DV TLS CA - G1</el-descriptions-item> -->
          <el-descriptions-item label="使用者">{{ state.certInfos?.subject }}</el-descriptions-item>
          <el-descriptions-item label="颁发者">{{ state.certInfos?.issuer }}</el-descriptions-item>
          <!-- <el-descriptions-item label="有效期">2017-11-27 ~ 2027-11-27（剩余 1039 天）</el-descriptions-item> -->
        </el-descriptions>
      </el-tab-pane>
      <el-tab-pane label="证书" name="certificate">
        <el-form label-position="top">
          <el-form-item label="私匙">
            <el-input :model-value="state.form.privateKey" :rows="5" type="textarea" readonly />
          </el-form-item>
          <el-form-item label="证书">
            <el-input :model-value="state.form.certificate" :rows="8" type="textarea" readonly />
          </el-form-item>
        </el-form>
      </el-tab-pane>
      <el-tab-pane label="下载" name="down">
        <el-table
          :data="[{ serverType: 'Nginx', certFormat: 'pem/key' }, { serverType: 'Tomcat', certFormat: 'pfx' }, { serverType: 'Apache', certFormat: 'crt/key' }, { serverType: 'IIS', certFormat: 'pfx' }, /*{ serverType: 'JKS', certFormat: 'jks' }, */{ serverType: '其他', certFormat: 'pem/key' }]"
          style="width: 100%" stripe fit>
          <el-table-column prop="serverType" label="服务器类型" align="center" />
          <el-table-column prop="certFormat" label="证书格式" align="center" />
          <el-table-column prop="address" label="操作" align="center">
            <template #default="scope">
              <el-link
                :href="getUrl(`./api/RequestDomain/DownloadCertFile?id=${state.form.id}&certFormat=${scope.row.certFormat}&serverType=${scope.row.serverType}`)"
                target="_blank">下载</el-link>
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>
    </el-tabs>
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="state.isShowDialog = false">关 闭</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import client, { getUrl } from '@/api';
const state = reactive({
  certInfos: {} as any,
  loading: false,
  isShowDialog: false,
  form: { id: 0, authenticationMethods: '', privateKey: '', certificate: '' }
});

// 打开弹窗
const openDialog = async (row: any | undefined) => {
  state.form = { id: row.id, authenticationMethods: '', privateKey: row.privateKey, certificate: row.certificate };
  state.isShowDialog = true;

  state.certInfos = undefined;
  state.loading = true;
  const res = await client.ApiRequestDomainGetCertInfo(row.id);
  state.certInfos = res ?? {};
  state.loading = false;
};
// 导出对象
defineExpose({ openDialog });

</script>

<style lang="scss" scoped></style>
