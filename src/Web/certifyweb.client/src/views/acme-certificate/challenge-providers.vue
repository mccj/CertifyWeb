<template>
  <div>
    <el-descriptions title="证书颁发机构" width="100%" style="width: 100%" :column="3">
      <template v-for="item in state.tableData">
        <el-descriptions-item label="名称">{{item.title}}</el-descriptions-item>
        <el-descriptions-item label="验证时长">{{item.propagationDelaySeconds}} 秒</el-descriptions-item>
        <el-descriptions-item label="帮助">
          <el-link :href="item.helpUrl" target="_blank">{{item.helpUrl}}</el-link>
        </el-descriptions-item>
        <el-descriptions-item label="描述" span="3">{{item.description}}</el-descriptions-item>
      </template>
    </el-descriptions>
    <!--<br>
    <el-pagination :page-sizes="[10, 20, 30, 50, 100, 200]" :disabled="false" :background="true"
      layout="total, sizes, prev, pager, next, jumper" :total="state.tableData?.total ?? 0"
      v-model:page-size="state.pagination.pageSize" v-model:current-page="state.pagination.pageNo"
      @change="loadDatas" />-->
  </div>
</template>

<script setup lang="ts">
  import { reactive, ref, onMounted } from 'vue';
  //import moment from 'moment';
  //import { formatDate, fromNow } from '@/utils/format-handle';
  import client, { CertificateAuthority } from '@/api';

  const state = reactive({
    tableData: [] as CertificateAuthority[],
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
    state.tableData = await client.ApiAcmeCertificateGetChallengeProviders(/*Object.assign({}, state.query, { pageNo: state.pagination.pageNo, pageSize: state.pagination.pageSize })*/);
    state.isLoading = false;
  };

</script>

<style lang="scss" scoped></style>
