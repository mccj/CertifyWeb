<template>
  <el-dialog v-model="dialogVisible" destroy-on-close draggable title="DNS 账户" width="500">
    {{form}}
    <el-form label-width="auto" :model="form">
      <el-form-item label="验证服务商" required>
        <el-select v-model="form.dnsId" placeholder="请选择 验证服务商" clearable>
          <el-option v-for="item in state.dnsList" :key="item.id" :label="item.title" :value="item.id" />
        </el-select>
      </el-form-item>
      <el-form-item v-for="item in state.dnsList?.find(x=>x.id==form.dnsId)?.providerParameters" :key="item.key" :label="item.name" :required="item.isRequired">
        <el-input v-model="form.parameters[item.key]"  placeholder="请填写 keyId" clearable :show-password="item.isPassword" :type="item.isMultiLine?'textarea':'text'" />
      </el-form-item>
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="dialogVisible = false">保存</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
  import { ref, reactive, onMounted } from 'vue'
  //import { ElMessageBox } from 'element-plus'
  import client, { CertificateAuthority } from '@/api';
  const dialogVisible = ref(true)
  const form = reactive({
    dnsId: '',
    parameters: {}
  })
  const state = reactive({
    acmeList: [] as CertificateAuthority[],
    isLoading: false,
    //query: {} as HouseholdGetPageRequestType,
    //pagination: {
    //  pageSize: 10,
    //  pageNo: 1
    //}
  });

  onMounted(async () => {
    await loadDatas();
  });

  const loadDatas = async () => {
    state.isLoading = true;
    state.dnsList = await client.ApiAcmeCertificateGetChallengeProviders(/*Object.assign({}, state.query, { pageNo: state.pagination.pageNo, pageSize: state.pagination.pageSize })*/);
    state.isLoading = false;
  };
</script>

<style lang="scss" scoped></style>
