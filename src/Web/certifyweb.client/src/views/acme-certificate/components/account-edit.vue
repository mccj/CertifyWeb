<template>
  <el-dialog v-model="state.isShowDialog" destroy-on-close draggable title="Acme 账户" width="500">
    <el-form label-width="auto" :model="state.form" ref="accountFormRef">
      <el-form-item label="证书颁发机构" required prop="acmeId"
        :rules="[{ required: true, message: '证书颁发机构不能为空', trigger: 'change' }]">
        <el-select v-model="state.form.acmeId" placeholder="请选择 证书颁发机构" clearable>
          <el-option v-for="item in state.acmeList" :key="item.id" :label="`${item.title}【${item.standardExpiryDays}天】`"
            :value="item.id ?? ''" />
        </el-select>
      </el-form-item>
      <el-form-item label="电子邮件" required prop="email"
        v-if="state.acmeList?.find(x => x.id == state.form.acmeId)?.requiresEmailAddress"
        :rules="[{ required: true, message: '电子邮件不能为空', trigger: 'blur' }]">
        <el-input v-model="state.form.email" placeholder="请填写 电子邮件" clearable />
      </el-form-item>
      <template v-if="state.form.acmeId">
        <el-alert :title="state.acmeList?.find(x => x.id == state.form.acmeId)?.description" type="success" />
        <el-alert :title="state.acmeList?.find(x => x.id == state.form.acmeId)?.websiteUrl" type="success" />
        <el-alert :title="state.acmeList?.find(x => x.id == state.form.acmeId)?.privacyPolicyUrl" type="success" />
      </template>
      <template v-if="state.acmeList?.find(x => x.id == state.form.acmeId)?.requiresExternalAccountBinding">
        <el-divider />
        <el-form-item label="Key Id">
          <el-input v-model="state.form.eabKeyId" required placeholder="请填写 eabKeyId" clearable />
        </el-form-item>
        <el-form-item label="Key(HMAC)">
          <el-input v-model="state.form.eabHmacKey" required placeholder="请填写 eabHmacKey" clearable />
        </el-form-item>
        <el-form-item label="Key algorithm">
          <el-input v-model="state.form.eabKeyAlg" placeholder="请填写 eabKeyAlg" clearable />
        </el-form-item>
        <el-alert :title="state.acmeList?.find(x => x.id == state.form.acmeId)?.eabInstructions" type="success" />
      </template>
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
import client, { CertificateAuthority, AcmeInfoUpdateInput } from '@/api';

const emits = defineEmits(['handleQuery']);

const accountFormRef = ref();
const state = reactive({
  acmeList: [] as CertificateAuthority[],
  loading: false,
  isShowDialog: false,
  form: {} as AcmeInfoUpdateInput
});

onMounted(async () => {
  state.loading = true;
  const res = await client.ApiAcmeCertificateGetCertificateAuthorities(/*Object.assign({}, state.query, { pageNo: state.pagination.pageNo, pageSize: state.pagination.pageSize })*/);
  state.acmeList = res ?? [];
  state.loading = false;
});


// 打开弹窗
const openDialog = async (row: unknown | undefined) => {
  accountFormRef.value?.resetFields();
  state.form = JSON.parse(JSON.stringify(row ?? {}));
  state.isShowDialog = true;
};

// 关闭弹窗
const closeDialog = () => {
  emits('handleQuery');
  state.isShowDialog = false;
};

// 提交
const submit = () => {
  accountFormRef.value.validate(async (valid: boolean) => {
    if (!valid) return;
    state.form.directoryUri = state.acmeList?.find(x => x.id == state.form.acmeId)?.productionAPIEndpoint;
    if (state.form?.id ?? 0 > 0) {
      await client.ApiAcmeInfoUpdate(state.form);
    } else {
      await client.ApiAcmeInfoCreate(state.form);
    }
    closeDialog();
  });
};

// 导出对象
defineExpose({ openDialog });

</script>

<style lang="scss" scoped></style>
