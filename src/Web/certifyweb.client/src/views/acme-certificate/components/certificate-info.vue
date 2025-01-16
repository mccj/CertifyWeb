<template>
  <el-dialog v-model="state.isShowDialog" destroy-on-close draggable title="证书下载" width="500">
    <el-table
      :data="[{ serverType: 'Nginx', certFormat: 'pem/key' }, { serverType: 'Tomcat', certFormat: 'pfx' }, { serverType: 'Apache', certFormat: 'crt/key' }, { serverType: 'IIS', certFormat: 'pfx' }, { serverType: 'JKS', certFormat: 'jks' }, { serverType: '其他', certFormat: 'pem/key' }]"
      style="width: 100%">
      <el-table-column prop="serverType" label="服务器类型" />
      <el-table-column prop="certFormat" label="证书格式" />
      <el-table-column prop="address" label="操作">
        <template #default="scope">
          <el-link :href="getUrl(`./api/RequestDomain/DownloadCertFile?id=${state.form.id}&certFormat=${scope.row.certFormat}&serverType=${scope.row.serverType}`)" target="_blank">下载</el-link>
        </template>
      </el-table-column>
    </el-table>
    <template #footer>
      <div class="dialog-footer">
        <!-- <el-button type="info" v-if="state.step > 0" @click="state.step--">上一步</el-button>
        <el-button type="primary" @click="submit">下一步</el-button>
        <el-button type="primary" @click="submit22">获取证书</el-button> -->
        <el-button @click="state.isShowDialog = false">取 消</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
// //import { ElMessageBox } from 'element-plus'
import client, { getUrl } from '@/api';

// // const emits = defineEmits(['handleQuery']);

// const dnsFormRef = ref();

const state = reactive({
  // step: 0,
  // authoritieInfos: [] as any[],
  // acmeList: [] as AcmeInfoDetailOutput[],
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
  // dnsFormRef.value?.resetFields();
  // state.step = 0;
  state.form = { id: row.id, authenticationMethods: '' };
  state.isShowDialog = true;

  state.loading = true;
  // const res = await client.ApiRequestDomainGetDomainOrderAuthorizations(row.id);
  // state.authoritieInfos = res ?? [];
  state.loading = false;
};

// // 关闭弹窗
// const closeDialog = () => {
//   emits('handleQuery');
//   state.isShowDialog = false;
// };

// // 提交
// const submit =async () => {
//   const res = await client.ApiRequestDomainGetDomainOrderAuthValidate({directoryUri:state.authoritieInfos[0].directoryUri,authUrl:state.authoritieInfos[0].authUrl});
//   // dnsFormRef.value.validate(async (valid: boolean) => {
//   //   if (!valid) return;

//   //   if (state.step == 0) {
//   //     state.step = 1;
//   //     // client.ApiAcmeCertificateGetCertificateAuthorities(state.form.id)
//   //     // if (state.form.authenticationMethods == 'autoDns') {
//   //     //   await client.ApiRequestDomainAutoByDns(state.form.id);
//   //     // } else if (state.form.authenticationMethods == 'handDns') {
//   //     //   await client.ApiRequestDomainHandByDns(state.form.id);
//   //     // } else if (state.form.authenticationMethods == 'handHttp') {
//   //     //   await client.ApiRequestDomainHandByHttp(state.form.id);
//   //     // }
//   //   }
//   //   // if (state.form?.id ?? 0 > 0) {
//   //   //   await client.ApiDomainInfoUpdate(state.form);
//   //   // } else {
//   //   //   await client.ApiDomainInfoCreate(state.form);
//   //   // }
//   //   // closeDialog();
//   // });
// };
// const submit22=async()=>{
//   const res = await client.ApiRequestDomainDownloadCertificate(state.form.id);

// }
// 导出对象
defineExpose({ openDialog });

</script>

<style lang="scss" scoped></style>
