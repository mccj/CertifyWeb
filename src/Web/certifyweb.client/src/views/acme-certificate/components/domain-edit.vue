<template>
  <el-dialog v-model="state.isShowDialog" destroy-on-close draggable title="证书" width="500">
    <el-form ref="dnsFormRef" label-width="auto" :model="state.form">
      <el-form-item label="备注" prop="description">
        <el-input v-model="state.form.description" placeholder="请填写 备注" clearable />
      </el-form-item>
      <el-form-item label="主域名" required prop="primaryDomain"
        :rules="[{ required: true, message: '主域名 不能为空', trigger: 'blur' }]">
        <el-input v-model="state.form.primaryDomain" placeholder="请填写 主域名" clearable/>
      </el-form-item>
      <el-form-item label="其他域名" prop="otherDomains">
        <el-input v-model="state.form.otherDomains" placeholder="请填写 其他域名" clearable type="textarea" />
      </el-form-item>
      <el-form-item label="Acme 账户" prop="acmeId">
        <el-select v-model="state.form.acmeId" placeholder="请选择 Acme 账户" clearable>
          <el-option v-for="item in state.acmeList" :key="item.id" :label="item.description" :value="item.id ?? ''" />
        </el-select>
      </el-form-item>
      <!-- <el-form-item label="验证方式" prop="acmeId">
        <el-select v-model="state.form.acmeId" placeholder="请选择 Acme 账户" clearable>
          <el-option v-for="item in state.acmeList" :key="item.id" :label="item.description" :value="item.id ?? ''" />
        </el-select>
      </el-form-item> -->
      密钥算法
      验证方式
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
import client, { AcmeInfoDetailOutput, DomainInfoUpdateInput } from '@/api';

const emits = defineEmits(['handleQuery']);

const dnsFormRef = ref();

const state = reactive({
  acmeList: [] as AcmeInfoDetailOutput[],
  loading: false,
  isShowDialog: false,
  form: { } as DomainInfoUpdateInput
  //query: {} as HouseholdGetPageRequestType,
  //pagination: {
  //  pageSize: 10,
  //  pageNo: 1
  //}
});

onMounted(async () => {
  state.loading = true;
  const res = await client.ApiAcmeInfoGetList({}/*Object.assign({}, state.query, { pageNo: state.pagination.pageNo, pageSize: state.pagination.pageSize })*/);
  state.acmeList = res ?? [];
  state.loading = false;
});

// 打开弹窗
const openDialog = async (row: unknown | undefined) => {
  dnsFormRef.value?.resetFields();
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
  dnsFormRef.value.validate(async (valid: boolean) => {
    if (!valid) return;
    if (state.form?.id ?? 0 > 0) {
      await client.ApiDomainInfoUpdate(state.form);
    } else {
      await client.ApiDomainInfoCreate(state.form);
    }
    closeDialog();
  });
};

// 导出对象
defineExpose({ openDialog });

</script>

<style lang="scss" scoped></style>
