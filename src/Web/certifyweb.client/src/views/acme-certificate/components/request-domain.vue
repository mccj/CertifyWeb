<template>
  <el-dialog v-model="state.isShowDialog" destroy-on-close draggable title="申请证书" width="500">
    <el-tabs >
    <el-tab-pane label="DNS 验证">
<div v-for="item in state.authoritieInfos" :key="item.id" >
  域名 {{ `${item.resource?.wildcard?'*.':''}${item.resource?.identifier?.value}` }}<br>
  状态 {{ item.resource?.status }}<br>
  dns验证 _acme-challenge.{{ item.resource?.identifier?.value }}
  {{ item.dnsAuthz.dnsTxt }}<br>
  http 访问
  http://{{ item.resource?.identifier?.value }}/.well-known/acme-challenge/{{ item.dnsAuthz.token }}
  {{ item.dnsAuthz.keyAuthz }}<br>
</div>
    </el-tab-pane>
    <el-tab-pane label="Http 验证" name="http">

    </el-tab-pane>
    <el-tab-pane label="Role" name="third">

    </el-tab-pane>
  </el-tabs>
    <el-form ref="dnsFormRef" label-width="auto" :model="state.form">
      <!-- <el-form-item v-if="state.step == 0" label="验证方式" required prop="authenticationMethods"
        :rules="[{ required: true, message: '验证方式 不能为空', trigger: 'blur' }]">
        <el-radio-group v-model="state.form.authenticationMethods">
          <el-radio value="autoDns">自动DNS验证</el-radio>
          <el-radio value="handDns">手动DNS验证</el-radio>
          <el-radio value="handHttp">手动Http</el-radio>
        </el-radio-group>
      </el-form-item> -->
      <!-- <el-form-item label="其他域名" prop="otherDomain">
        <el-input v-model="state.form.otherDomain" placeholder="请填写 其他域名" clearable type="textarea" />
      </el-form-item>
      <el-form-item label="备注" prop="description">
        <el-input v-model="state.form.description" placeholder="请填写 备注" clearable />
      </el-form-item>
      <el-form-item label="Acme 账户" required prop="acmeId"
        :rules="[{ required: true, message: 'Acme 账户不能为空', trigger: 'change' }]">
        <el-select v-model="state.form.acmeId" placeholder="请选择 Acme 账户" clearable>
          <el-option v-for="item in state.acmeList" :key="item.id" :label="item.description" :value="item.id ?? ''" />
        </el-select>
      </el-form-item>
      密钥算法
      验证方式 -->
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button type="info" v-if="state.step > 0" @click="state.step--">上一步</el-button>
        <el-button type="primary" @click="submit">下一步</el-button>
        <el-button type="primary" @click="submit22">获取证书</el-button>
        <el-button @click="state.isShowDialog = false">取 消</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
//import { ElMessageBox } from 'element-plus'
import client, { AcmeInfoDetailOutput } from '@/api';

// const emits = defineEmits(['handleQuery']);

const dnsFormRef = ref();

const state = reactive({
  step: 0,
  authoritieInfos: [] as any[],
  acmeList: [] as AcmeInfoDetailOutput[],
  loading: false,
  isShowDialog: false,
  form: { id: 0, authenticationMethods: '' }
  //query: {} as HouseholdGetPageRequestType,
  //pagination: {
  //  pageSize: 10,
  //  pageNo: 1
  //}
});

// onMounted(async () => {
//   state.loading = true;
//   const res = await client.ApiAcmeInfoGetList({}/*Object.assign({}, state.query, { pageNo: state.pagination.pageNo, pageSize: state.pagination.pageSize })*/);
//   state.acmeList = res ?? [];
//   state.loading = false;
// });

// 打开弹窗
const openDialog = async (row: any | undefined) => {
  dnsFormRef.value?.resetFields();
  state.step = 0;
  state.form = { id: row.id, authenticationMethods: '' };
  state.isShowDialog = true;

  state.loading = true;
  const res = await client.ApiRequestDomainGetDomainOrderAuthorizations(row.id);
  state.authoritieInfos = res ?? [];
  state.loading = false;
};

// // 关闭弹窗
// const closeDialog = () => {
//   emits('handleQuery');
//   state.isShowDialog = false;
// };

// 提交
const submit =async () => {
  const res = await client.ApiRequestDomainGetDomainOrderAuthValidate({directoryUri:state.authoritieInfos[0].directoryUri,authUrl:state.authoritieInfos[0].authUrl});
  // dnsFormRef.value.validate(async (valid: boolean) => {
  //   if (!valid) return;

  //   if (state.step == 0) {
  //     state.step = 1;
  //     // client.ApiAcmeCertificateGetCertificateAuthorities(state.form.id)
  //     // if (state.form.authenticationMethods == 'autoDns') {
  //     //   await client.ApiRequestDomainAutoByDns(state.form.id);
  //     // } else if (state.form.authenticationMethods == 'handDns') {
  //     //   await client.ApiRequestDomainHandByDns(state.form.id);
  //     // } else if (state.form.authenticationMethods == 'handHttp') {
  //     //   await client.ApiRequestDomainHandByHttp(state.form.id);
  //     // }
  //   }
  //   // if (state.form?.id ?? 0 > 0) {
  //   //   await client.ApiDomainInfoUpdate(state.form);
  //   // } else {
  //   //   await client.ApiDomainInfoCreate(state.form);
  //   // }
  //   // closeDialog();
  // });
};
const submit22=async()=>{
  const res = await client.ApiRequestDomainDownloadCertificate(state.form.id);

}
// 导出对象
defineExpose({ openDialog });

</script>

<style lang="scss" scoped></style>
