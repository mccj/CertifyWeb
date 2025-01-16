<template>
  <el-dialog v-model="state.isShowDialog" destroy-on-close draggable title="DNS 账户" width="500">
    <el-form ref="dnsFormRef" label-width="auto" :model="state.form">
      <el-form-item label="备注" required prop="description"
        :rules="[{ required: true, message: '备注 不能为空', trigger: 'blur' }]">
        <el-input v-model="state.form.description" placeholder="请填写 备注" clearable />
      </el-form-item>
      <el-form-item label="验证服务商" required prop="providerType"
        :rules="[{ required: true, message: '验证服务商不能为空', trigger: 'change' }]">
        <el-select v-model="state.form.providerType" placeholder="请选择 验证服务商" clearable>
          <el-option v-for="item in state.dnsList" :key="item.id" :label="item.title" :value="item.id ?? ''" />
        </el-select>
      </el-form-item>
      <el-alert :title="getProviderParameters(state.form?.providerType)?.description" type="success" />
      <el-alert :title="getProviderParameters(state.form?.providerType)?.helpUrl" type="success" />
      <el-form-item v-for="item in getProviderParameters(state.form?.providerType)?.providerParameters" :key="item.key"
        :label="item.name" :required="item.isRequired && item.isCredential" :prop="`parameters.${item.key}`"
        :rules="[{ required: item.isRequired && item.isCredential, message: `${item.name} 不能为空`, trigger: 'change' }]">
        <el-input v-model="(state.form.parameters ?? {})[item.key]" placeholder="请填写 keyId" clearable
          :show-password="item.isPassword" :type="item.isMultiLine ? 'textarea' : 'text'" />
      </el-form-item>
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="state.isShowDialog = false">取 消</el-button>
        <el-button type="primary" @click="submit">确 定</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
//import { ElMessageBox } from 'element-plus'
import client, { CertificateAuthority, DnsInfoUpdateInput } from '@/api';

const emits = defineEmits(['handleQuery']);

const dnsFormRef = ref();

const state = reactive({
  dnsList: [] as CertificateAuthority[],
  loading: false,
  isShowDialog: false,
  form: { parameters: {} } as DnsInfoUpdateInput
  //query: {} as HouseholdGetPageRequestType,
  //pagination: {
  //  pageSize: 10,
  //  pageNo: 1
  //}
});

const getProviderParameters = (id?: string) => {
  const r = state.dnsList?.find(x => x.id == id);
  return r;
}

onMounted(async () => {
  state.loading = true;
  const res = await client.ApiAcmeCertificateGetChallengeProviders(/*Object.assign({}, state.query, { pageNo: state.pagination.pageNo, pageSize: state.pagination.pageSize })*/);
  state.dnsList = res ?? [];
  state.loading = false;
});

// 打开弹窗
const openDialog = async (row: unknown | undefined) => {
  dnsFormRef.value?.resetFields();
  state.form = JSON.parse(JSON.stringify(row ?? {}));
  if (!state.form.parameters)
    state.form.parameters = {};
  state.isShowDialog = true;
};

// 关闭弹窗
const closeDialog = () => {
  emits('handleQuery');
  state.isShowDialog = false;
};

// 提交
const submit = () => {
  dnsFormRef.value.validate(async (valid: boolean) => {
    if (!valid) return;
    state.form.providerTitle = state.dnsList?.find(x => x.id == state.form.providerType)?.title;
    if (state.form?.id ?? 0 > 0) {
      await client.ApiDnsInfoUpdate(state.form);
    } else {
      await client.ApiDnsInfoCreate(state.form);
    }
    closeDialog();
  });
};

// 导出对象
defineExpose({ openDialog });

</script>

<style lang="scss" scoped></style>
